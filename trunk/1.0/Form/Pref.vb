' ��������������������������������������������������������������������������������������������������������������
' |                                                                                                            |
' |    ZGuideTV.NET: An Electronic Program Guide (EPG) - i.e. an "electronic TV magazine"                      |
' |    - which makes the viewing of today's and next week's TV listings possible. It can be customized to      |
' |    pick up the TV listings you only want to have a look at. The application also enables you to carry out  |
' |    a search or even plan to record something later through the K!TV software.                              |
' |                                                                                                            |
' |    Copyright (C) 2004-2012 ZGuideTV.NET Team <http://zguidetv.codeplex.com/>                               |
' |                                                                                                            |
' |    Project administrator : Pascal Hubert (neojudgment@hotmail.com)                                         |
' |                                                                                                            |
' |    This program is free software: you can redistribute it and/or modify                                    |
' |    it under the terms of the GNU General Public License as published by                                    |
' |    the Free Software Foundation, either version 2 of the License, or                                       |
' |    (at your option) any later version.                                                                     |
' |                                                                                                            |
' |    This program is distributed in the hope that it will be useful,                                         |
' |    but WITHOUT ANY WARRANTY; without even the implied warranty of                                          |
' |    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the                                           |
' |    GNU General Public License for more details.                                                            |
' |                                                                                                            |
' |    You should have received a copy of the GNU General Public License                                       |
' |    along with this program.  If not, see <http://www.gnu.org/licenses/>.                                   |
' |                                                                                                            |
' ��������������������������������������������������������������������������������������������������������������
Imports System.IO
Imports System.Globalization
Imports System.Net
Imports Microsoft.DirectX
Imports Microsoft.DirectX.AudioVideoPlayback

Public Class Pref

#Region "Property"

    Private _updateIn As String = "jour(s)"

    Public Property UpdateIn As String
        Get
            Return _updateIn
        End Get
        Set(ByVal Value As String)
            _updateIn = Value
        End Set
    End Property

    Private _checkIn As String = "jour(s)"

    Public Property CheckIn As String
        Get
            Return _checkIn
        End Get
        Set(ByVal Value As String)
            _checkIn = Value
        End Set
    End Property

    Private Property _messageBoxEPGDataTitre As String

    Public Property messageBoxEPGDataTitre As String
        Get
            Return _messageBoxEPGDataTitre
        End Get
        Set(ByVal Value As String)
            _messageBoxEPGDataTitre = Value
        End Set
    End Property

    Private Property _messageBoxEPGData As String

    Public Property messageBoxEPGData As String
        Get
            Return _messageBoxEPGData
        End Get
        Set(ByVal Value As String)
            _messageBoxEPGData = Value
        End Set
    End Property

    Private Property _messageBoxEPGData1 As String

    Public Property messageBoxEPGData1 As String
        Get
            Return _messageBoxEPGData1
        End Get
        Set(ByVal Value As String)
            _messageBoxEPGData1 = Value
        End Set
    End Property

#End Region

    Private Sub TailleBdd()
        Dim FichierInfo As New FileInfo(BDDPath & "db_progs.db3")
        If My.Computer.FileSystem.FileExists(BDDPath & "db_progs.db3") Then
            Dim TailleFichier As Integer = CInt(FichierInfo.Length)
            TailleFichier = CInt(TailleFichier/1024)
            If TailleFichier < 4 Then
                Me.Dispose()
                Miseajour.ShowDialog()
            End If
        End If
    End Sub

    Friend Sub frmPref_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        ' On regarde quel langue utiliser 22/08/2008
        LanguageCheck()
        Trace.WriteLine(DateTime.Now & " On vient d'entrer dans les Pr�f�rences")
    End Sub

    Private Sub BT_Cancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ButtonCancel.Click

        With My.Settings
            If .firststart Then
                .firststart = False
                .Save()
                Me.Close()
            End If
        End With
        TailleBdd()
        Trace.WriteLine(DateTime.Now & " On vient de quitterles Pr�f�rences via le bouton Cancel")

        'N�o 18/08/2010
        With Mainform.Timer_minute
            .Start()
            .Enabled = True
        End With

        Me.Close()
    End Sub

    Private Sub BT_Ok_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ButtonOK.Click

        ' On va �crire les sons wav s�lectionn�s
        My.Settings.RSSSound = ComboBoxRSS.Text
        My.Settings.MessagesSound = ComboBoxMessages.Text
        My.Settings.ReminderSound = ComboBoxReminder.Text

        ' On va �crire le volume du flux RSS
        My.Settings.RSSVolume = - 3000 + (TrackBarRSS.Value*300)

        ' On va �crire le volume des messages
        My.Settings.MessagesVolume = - 3000 + (TrackBarMessages.Value*300)

        ' On va �crire le volume des rappels
        My.Settings.ReminderVolume = - 3000 + (TrackBarReminder.Value*300)

        If CheckBoxAudioOn.Checked = True Then
            My.Settings.AudioOn = False
            Mainform.ToolStripStatusLabelAudio.Image = My.Resources.soundmute
        Else
            My.Settings.AudioOn = True
            Mainform.ToolStripStatusLabelAudio.Image = My.Resources.sound
        End If

        With My.Settings
            If .firststart Then
                .firststart = False
                .Save()
                Me.Close()
            End If
        End With

        ' dans frmpref on a cliqu� sur OK pour accepter le choix des preferences
        Trace.WriteLine(DateTime.Now.ToString & "  passage dans bouton ok de frmpref")

        'gestion des sources
        If RadioButtonEPGData.Checked Then
            If tbPin.Text.Trim.Length = 0 Then
                Dim BoxEPGData As DialogResult
                BoxEPGData =
                    MessageBox.Show(
                        messageBoxEPGData & Chr(13) & messageBoxEPGData1, messageBoxEPGDataTitre, MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation)
                Return

            Else

                With My.Settings
                    .Epgdata = True
                    .PinEpgdata = tbPin.Text.Trim
                    .EpgdataDownload = NumericUpDownDownloadEpgdata.Value
                    .Save()
                    .Lienmiseajour = "http://www.epgdata.com/"
                    .Save()
                End With
            End If
        Else
            My.Settings.Epgdata = False
            My.Settings.Save()
        End If

        TailleBdd()

        ' on va �crire les options du proxy
        My.Settings.loginproxy = Login.Text

        ' login du proxy
        My.Settings.passproxy = Pass.Text

        ' mot de pas du proxy
        My.Settings.Proxy = ProxyHost.Text

        ' url du proxy
        If (Not ProxyPort.Text Is Nothing AndAlso String.IsNullOrEmpty(ProxyPort.Text)) Then
            ProxyPort.Text = "1"
        End If

        ' le port du proxy ne peut pas �tre � null sinon
        My.Settings.ProxyPort = ProxyPort.Text

        ' port du proxy
        My.Settings.tracelog = fichier_log_activ�

        ' on va �crire si l'option 'FadeOut' est activ�e ou non
        With My.Settings
            If CheckBoxFadeOut.Checked Then
                .FadeOut = 1
            Else
                .FadeOut = 0
            End If
        End With

        ' on va �crire si l'option 'InfoBulles' est activ�e ou non
        With My.Settings
            If CheckBoxToolTipsBallon.Checked Then
                .ToolTipsBallon = True
            Else
                .ToolTipsBallon = False
            End If
        End With

        ' on va �crire si l'option 'les dimensions et la position
        ' de la fen�tre principale() ' auto est activ�e ou non
        With CheckBoxConfigWindow
            If .Checked Then
                My.Settings.WindowState = 1
            Else
                My.Settings.WindowState = 0
            End If
        End With

        ' on va �crire si l'option 'sauvegarde de la position
        ' des buttons est activ�e ou non
        With CheckBoxbuttonssave
            If .Checked Then
                My.Settings.buttonssave = 1
            Else
                My.Settings.buttonssave = 0
            End If
        End With

        ' on va �crire si l'option 'Cacher automatiquement
        ' les filtres est activ�e ou non
        With CheckBoxFilters
            If .Checked Then
                My.Settings.Cacher_Filtre = True
            Else
                My.Settings.Cacher_Filtre = False
            End If
        End With

        ' On va �crire la configuration mat�rielle dans le user.config
        With My.Settings
            If RadioButtonConfNormal.Checked Then
                .ConfHardware = 1
            End If

            If RadioButtonConfAverage.Checked Then
                .ConfHardware = 2
            End If

            If RadioButtonConfMinimum.Checked Then
                .ConfHardware = 3
            End If
        End With

        ' On va �crire si
        ' L utilisateur autorise le scroll horizontal quand in demande d afficher
        ' moins de chaines que son ecran ne le permet?
        ' Autorisation dans les frmpref (checkbox)
        With CheckBoxaccordscrollhorizontal
            If .Checked Then
                My.Settings.accordscrollhorizontal = True
                Trace.WriteLine(DateTime.Now & " " & "accord scroll horizontal = true")
            Else
                My.Settings.accordscrollhorizontal = False
                Trace.WriteLine(DateTime.Now & " " & "accord scroll horizontal = false")
            End If
        End With

        ' on va �crire si l'option 'UrlUpdate auto' auto est activ�e ou non
        With CheckBoxUrlUpdate
            If .Checked Then
                My.Settings.UrlUpdate = True
                Trace.WriteLine(DateTime.Now & " " & "UrlUpdate = true")
            Else
                My.Settings.UrlUpdate = False
                Trace.WriteLine(DateTime.Now & " " & "UrlUpdate = false")
            End If
        End With

        ' !!!!!!!!!!!!!!! � modifier (� mettre dans un module ???) N�o
        Trace.WriteLine(DateTime.Now.ToString & "  passage dans bouton ok de frmpref")
        Trace.WriteLine(DateTime.Now & " " & "nb_max chaine ecran = " & Mainform.NbMaxChaineEcran.ToString)
        Trace.WriteLine(
            DateTime.Now & " " & "nb de chaines differentes selectionn�es � la mise a jour =" &
            nombre_de_chaines_differentes.ToString)
        If My.Settings.nbchainesdiff < Mainform.NbMaxChaineEcran AndAlso My.Settings.accordscrollhorizontal Then
            Mainform.ScrollHorizontal = True
            My.Settings.scrollhorizontal = Mainform.ScrollHorizontal
            Trace.WriteLine(DateTime.Now & " " & "scroll_horizontal � true")
        End If

        If Not My.Settings.nbchainesdiff < Mainform.NbMaxChaineEcran Then
            Mainform.ScrollHorizontal = False
            My.Settings.scrollhorizontal = Mainform.ScrollHorizontal
            Trace.WriteLine(DateTime.Now & " " & "scroll_horizontal � false")
        End If

        ' on va �crire si l'option mise � jour auto est activ�e ou non
        With My.Settings
            If CheckBoxAutoUpdate.Checked Then
                .AutoUpdate = 1
            Else
                .AutoUpdate = 0
            End If
        End With

        ' On va �crire si l'option reprise automatique est activ�e ou non
        With My.Settings
            If CheckBoxResume.Checked Then
                .ResumeChecked = 1
            Else
                .ResumeChecked = 0
            End If
        End With

        ' On va �crire le nombre de minute avant la reprise automatique
        With My.Settings
            If RadioButtonResume5.Checked Then
                .ResumeBefore = 5
            End If
            If RadioButtonResume10.Checked Then
                .ResumeBefore = 10
            End If
            If RadioButtonResume15.Checked Then
                .ResumeBefore = 15
            End If
            If RadioButtonResume20.Checked Then
                .ResumeBefore = 20
            End If
            If RadioButtonResume30.Checked Then
                .ResumeBefore = 30
            End If
        End With

        ' On va �crire si l'option v�rification auto est activ�e ou non
        With My.Settings
            If CheckBoxAutoverif.Checked Then
                .Autoverif = 1
            Else
                .Autoverif = 0
            End If
        End With

        ' 03/05/2008 on va �crire si l option fichier_log_activ� est activ�e
        With My.Settings
            If CheckBoxtracelog.Checked Then
                .tracelog = True
            Else
                .tracelog = False
            End If
        End With

        ' on va �crire si l'option purge auto est activ�e ou non
        With My.Settings
            If CheckBoxPurge.Checked Then
                .Purgefichiers = 1
            Else
                .Purgefichiers = 0

            End If
        End With

        ' On va �crire l'intervalle de mise � jour dans le user.config
        With My.Settings
            If RadioButtonDay1.Checked Then
                .intervalmiseajour = 1
            End If

            If RadioButtonDay2.Checked Then
                .intervalmiseajour = 2
            End If

            If RadioButtonDay3.Checked Then
                .intervalmiseajour = 3
            End If

            If RadioButtonDay4.Checked Then
                .intervalmiseajour = 4
            End If

            If RadioButtonDay5.Checked Then
                .intervalmiseajour = 5
            End If
        End With

        ' On va �crire l'intervalle de v�rification dans le user.config
        With My.Settings
            If RadioButtonAllDays.Checked Then
                .intervalverif = 1
            End If

            If RadioButtonAllWeeks.Checked Then
                .intervalverif = 7
            End If

            If RadioButtonAllMonths.Checked Then
                .intervalverif = 30
            End If
        End With

        ' on va �crire si l'option Slide est activ�e ou non
        With CheckBoxSlide
            Select Case .Checked
                Case True
                    My.Settings.Slide = 1
                Case Else
                    My.Settings.Slide = 0
            End Select
        End With

        ' On va �crire la dur�e minimum de l'affichage
        ' dans Ce soir dans le user.config
        Dim tempsminimumcesoir As SByte
        With My.Settings
            If RadioButtonCesoirAll.Checked Then
                tempsminimumcesoir = 0
            End If

            If RadioButtonCesoir45.Checked Then
                tempsminimumcesoir = 45
            End If

            If RadioButtonCesoir60.Checked Then
                tempsminimumcesoir = 60
            End If

            If RadioButtonCesoir75.Checked Then
                tempsminimumcesoir = 75
            End If

            If RadioButtonCesoir90.Checked Then
                tempsminimumcesoir = 90
            End If

            If RadioButtonCesoir105.Checked Then
                tempsminimumcesoir = 105
            End If

            If RadioButtonCesoir120.Checked Then
                tempsminimumcesoir = 120
            End If
        End With

        ' On va �crire la dur�e minimum de l'affichage
        ' dans Maintenant dans le user.config
        Dim tempsminimummaintenant As SByte
        With My.Settings
            If RadioButtonMaintenantAll.Checked Then
                tempsminimummaintenant = 5
            End If

            If RadioButtonMaintenant45.Checked Then
                tempsminimummaintenant = 45
            End If

            If RadioButtonMaintenant60.Checked Then
                tempsminimummaintenant = 60
            End If

            If RadioButtonMaintenant75.Checked Then
                tempsminimummaintenant = 75
            End If

            If RadioButtonMaintenant90.Checked Then
                tempsminimummaintenant = 90
            End If

            If RadioButtonMaintenant105.Checked Then
                tempsminimummaintenant = 105
            End If

            If RadioButtonMaintenant120.Checked Then
                tempsminimummaintenant = 120
            End If
        End With

        ' on va �crire si l'option Ancienne interface est activ�e
        ' ou non et si la dur�e de 'Ce soir' ou 'Maintenant' �
        ' �t� modifi�e alors on red�marre ZG
        With CheckBoxoldUI

            If .Checked AndAlso My.Settings.oldUI = 0 Then
                ' Si on a activ�  l'�tat 3D alors qu il ne l �tait pas

                With My.Settings
                    .dureecesoir = tempsminimumcesoir
                    .dureemaintenant = tempsminimummaintenant
                    .oldUI = 1
                    .Save()
                End With

                Mainform.tl.Close()
                Me.Close()
                Trace.WriteLine(DateTime.Now.ToString & " On va faire un restart dans les pr�f�rences")
                Mainform.AppliRestart = True
            Else
                If .Checked = False AndAlso My.Settings.oldUI = 1 Then
                    ' si on a descativ� l'etat 3D alors qu il etait activ�
                    With My.Settings
                        .dureecesoir = tempsminimumcesoir
                        .dureemaintenant = tempsminimummaintenant
                        .oldUI = 0
                        .Save()
                    End With
                    Mainform.tl.Close()
                    Me.Close()
                    Trace.WriteLine(DateTime.Now.ToString & " On va faire un restart dans les pr�f�rences")
                    Mainform.AppliRestart = True
                End If
            End If
        End With

        ' Si la dur�e de 'Ce soir' ou 'Maintenant' seule � �t� modifi�e
        ' on red�marre ZG
        With My.Settings

            ' Si on vient de changer les durees pour ce_soir ou maintenant
            If tempsminimumcesoir <> .dureecesoir OrElse tempsminimummaintenant <> .dureemaintenant Then
                .dureecesoir = tempsminimumcesoir
                .dureemaintenant = tempsminimummaintenant
                .Save()

                Trace.WriteLine(DateTime.Now.ToString & " On va faire un restart dans les pr�f�rences")
                Mainform.tl.Close()
                Application.DoEvents()
                Mainform.AppliRestart = True
            End If
        End With

        Application.DoEvents()
        Trace.WriteLine(DateTime.Now.ToString & " On va sortir des pr�f�rences via le button OK")
        Me.Close()

        ' on y passe toujors qq soit le cas de figure meme pour un application.restart
        Me.Dispose() ' BB 040810 peut on faire cela ?? on execute encore du code dans frmpref!
        If Mainform.AppliRestart = True Then
            Mainform.Timer_wait.Enabled = True
            My.Settings.ModifPrefRestart = 1

        Else
            'N�o 18/08/2010
            With Mainform.Timer_minute
                .Start()
                .Enabled = True
            End With
        End If
    End Sub

    Public Sub New()
        InitializeComponent()

        ' intervalle de mise � jour en jour(s)
        Dim intervalmaj As Integer

        ' intervalle de v�rification en jour(s)
        Dim verifinterval As Integer

        ' diff�rence de date entre la derni�re mise � jour et maintenant
        Dim diffjourmaj As Long

        ' diff�rence de date entre la derni�re v�rification et maintenant
        Dim diffjourverif As Long

        ' nombre de jours restant avant la prochaine mise � jour
        Dim jourrestantmaj As String

        ' nombre de jours restant avant la prochaine v�rification
        Dim jourrestantverif As String

        ' purge
        Dim Purge As Integer

        ' mise � jour auto
        Dim AutoUpdate As Integer

        ' v�rification auto
        Dim Autoverif As Integer

        ' date de la derni�re mise � jour
        Dim datemaj As String
        Dim datemajverif As String
        Dim datesave As Date

        ' date de la derni�re v�rification
        Dim datesaveverif As Date

        ' Slide activ� dans central screen
        Dim Slide As Integer

        ' On va lire dans le .config
        ' url du proxy
        ProxyHost.Text = My.Settings.Proxy

        ' port du proxy
        ProxyPort.Text = My.Settings.ProxyPort

        ' login du proxy
        Login.Text = My.Settings.loginproxy

        ' mot de passe du proxy
        Pass.Text = My.Settings.passproxy

        ' purge des fichiers
        Purge = My.Settings.Purgefichiers

        ' mise � jour auto
        AutoUpdate = My.Settings.AutoUpdate

        ' interval de mise � jour
        intervalmaj = My.Settings.intervalmiseajour
        verifinterval = My.Settings.intervalverif

        ' date derni�re mise � jour
        datemaj = My.Settings.datemajmiseajour

        ' affichage effet Slide
        Slide = My.Settings.Slide

        ' v�rification(auto)
        Autoverif = My.Settings.Autoverif

        ' date derni�re v�rification
        datemajverif = My.Settings.datemajverif

        If Not (Not datemaj Is Nothing AndAlso String.IsNullOrEmpty(datemaj)) Then
            ' On affiche la date de la derni�re mise � jour dans les pr�f�rences
            datemaj = My.Settings.datemajmiseajour
            datesave = CDate(datemaj)
            TextBoxUpdateLast.Text = (datesave.ToLongDateString).ToString
        End If

        If Not (Not datemajverif Is Nothing AndAlso String.IsNullOrEmpty(datemajverif)) Then
            ' On affiche la date de la derni�re v�rification dans les pr�f�rences
            datemajverif = My.Settings.datemajverif
            datesaveverif = CDate(datemajverif)
            TextBoxLastUpdate.Text = (datesaveverif.ToLongDateString).ToString
        End If

        ' On va lire l'intervalle de mise � jour dans le user.config et on affiche dans les pr�f�rences
        With My.Settings
            If .intervalmiseajour = 1 Then
                RadioButtonDay1.Checked = True
            End If

            If .intervalmiseajour = 2 Then
                RadioButtonDay2.Checked = True
            End If

            If .intervalmiseajour = 3 Then
                RadioButtonDay3.Checked = True
            End If

            If .intervalmiseajour = 4 Then
                RadioButtonDay4.Checked = True
            End If

            If .intervalmiseajour = 5 Then
                RadioButtonDay5.Checked = True
            End If
        End With

        ' On va lire si la sauvegarde de la position des buttons est activ�e dans le user.config
        ' et on affiche dans les pr�f�rences
        With My.Settings
            If .buttonssave = 1 Then

                CheckBoxbuttonssave.Checked = True
            Else
                CheckBoxbuttonssave.Checked = False

            End If
        End With

        ' On va lire si les InfoBulles sont activ�es ou non
        ' et on affiche dans les pr�f�rences
        With My.Settings
            If .ToolTipsBallon = True Then

                CheckBoxToolTipsBallon.Checked = True
            Else
                CheckBoxToolTipsBallon.Checked = False

            End If
        End With

        ' On va lire l'intervalle de v�rification dans le user.config et on affiche dans les pr�f�rences
        With My.Settings
            If .intervalverif = 1 Then
                RadioButtonAllDays.Checked = True
            End If

            If .intervalverif = 7 Then
                RadioButtonAllWeeks.Checked = True
            End If

            If .intervalverif = 30 Then
                RadioButtonAllMonths.Checked = True
            End If
        End With

        ' On va lire le temps minimum d'affichage dans ce soir
        ' user.config et on affiche dans les pr�f�rences
        With My.Settings
            If .dureecesoir = 0 Then
                RadioButtonCesoirAll.Checked = True
            End If

            If .dureecesoir = 45 Then
                RadioButtonCesoir45.Checked = True
            End If

            If .dureecesoir = 60 Then
                RadioButtonCesoir60.Checked = True
            End If

            If .dureecesoir = 75 Then
                RadioButtonCesoir75.Checked = True
            End If

            If .dureecesoir = 90 Then
                RadioButtonCesoir90.Checked = True
            End If

            If .dureecesoir = 105 Then
                RadioButtonCesoir105.Checked = True
            End If

            If .dureecesoir = 120 Then
                RadioButtonCesoir120.Checked = True
            End If
        End With

        ' On va lire le temps minimum d'affichage dans maintenant
        ' user.config et on affiche dans les pr�f�rences
        With My.Settings
            If .dureemaintenant = 5 Then
                RadioButtonMaintenantAll.Checked = True
            End If

            If .dureemaintenant = 45 Then
                RadioButtonMaintenant45.Checked = True
            End If

            If .dureemaintenant = 60 Then
                RadioButtonMaintenant60.Checked = True
            End If

            If .dureemaintenant = 75 Then
                RadioButtonMaintenant75.Checked = True
            End If

            If .dureemaintenant = 90 Then
                RadioButtonMaintenant90.Checked = True
            End If

            If .dureemaintenant = 105 Then
                RadioButtonMaintenant105.Checked = True
            End If

            If .dureemaintenant = 120 Then
                RadioButtonMaintenant120.Checked = True
            End If
        End With

        ' On va lire si l'option effets Slide est coch�e
        With CheckBoxSlide
            Select Case My.Settings.Slide
                Case 1
                    .Checked = True
                Case Else
                    .Checked = False
            End Select
        End With

        ' On va lire si l'option 'Cacher automatiquement
        ' est coch�e
        Select Case My.Settings.Cacher_Filtre
            Case True
                CheckBoxFilters.Checked = True
            Case Else
                CheckBoxFilters.Checked = False
        End Select

        ' On va lire si l'option ancienne interface est coch�e
        With CheckBoxoldUI
            Select Case My.Settings.oldUI
                Case 1
                    .Checked = True
                Case Else
                    .Checked = False
            End Select
        End With

        ' On va lire la configuration mat�riel dans le user.config et on affiche dans les pr�f�rences
        With My.Settings
            If .ConfHardware = 1 Then ' pour afficher 36 heures
                RadioButtonConfNormal.Checked = True
            End If

            If .ConfHardware = 2 Then ' pour afficher 30 heures
                RadioButtonConfAverage.Checked = True
            End If

            If .ConfHardware = 3 Then ' pour afficher 24 heures
                RadioButtonConfMinimum.Checked = True
            End If
        End With

        ' On va lire si
        ' L utilisateur autorise le scroll horizontal quand in demande d afficher
        ' moins de chaines que son ecran ne le permet?
        ' Autorisation dans les frmpref (checkbox) et on grise la box si nbchainesdiff >
        If My.Settings.nbchainesdiff < Mainform.NbMaxChaineEcran Then
            GroupBoxMouse.Enabled = True
        Else
            GroupBoxMouse.Enabled = False
        End If

        With CheckBoxaccordscrollhorizontal
            Select Case My.Settings.accordscrollhorizontal
                Case True
                    .Checked = True
                    Trace.WriteLine(DateTime.Now & " " & "accord scroll horizontal = vrai")
                Case Else
                    .Checked = False
                    Trace.WriteLine(DateTime.Now & " " & "accord scroll horizontal = false")
            End Select
        End With

        ' On va lire si l'option UrlUpdate auto est coch�e
        With CheckBoxUrlUpdate
            Select Case My.Settings.UrlUpdate
                Case True
                    .Checked = True
                    Trace.WriteLine(DateTime.Now & " " & "UrlUpdate = true")
                Case Else
                    .Checked = False
                    Trace.WriteLine(DateTime.Now & " " & "UrlUpdate = false")
            End Select
        End With

        ' On va lire si l'option mise � jour auto est coch�e
        With CheckBoxAutoUpdate
            Select Case My.Settings.AutoUpdate
                Case 1
                    .Checked = True
                Case Else
                    .Checked = False
            End Select
        End With

        ' On va lire si l'option reprise automatique est coch�e
        With CheckBoxResume
            Select Case My.Settings.ResumeChecked
                Case 1
                    .Checked = True
                    GroupBoxResumeBefore.Enabled = True
                Case Else
                    .Checked = False
                    GroupBoxResumeBefore.Enabled = False
            End Select
        End With

        ' On va lire le temps avant la reprise automatique
        With My.Settings
            Select Case .ResumeBefore
                Case 5
                    RadioButtonResume5.Checked = True
                Case 10
                    RadioButtonResume10.Checked = True
                Case 15
                    RadioButtonResume15.Checked = True
                Case 20
                    RadioButtonResume20.Checked = True
                Case 30
                    RadioButtonResume30.Checked = True
            End Select
        End With

        ' On va lire si l'option v�rification auto est coch�e
        With CheckBoxAutoverif
            Select Case My.Settings.Autoverif
                Case 1
                    .Checked = True
                Case Else
                    .Checked = False
            End Select
        End With

        ' On va lire si l'option purge auto est coch�e
        With CheckBoxPurge
            Select Case My.Settings.Purgefichiers
                Case 1
                    .Checked = True
                Case Else
                    .Checked = False
            End Select
        End With

        ' On va lire si l'option FadeOut est coch�e
        With CheckBoxFadeOut
            Select Case My.Settings.FadeOut
                Case 1
                    .Checked = True
                Case Else
                    .Checked = False
            End Select
        End With

        ' on va lire si l option fichier_log_activ� est activ�e
        With CheckBoxtracelog
            Select Case My.Settings.tracelog
                Case True
                    .Checked = True
                    fichier_log_activ� = True
                Case Else
                    .Checked = False
                    fichier_log_activ� = False
            End Select
        End With

        ' On va lire si l'option des dimensions et de la position
        ' de la fen�tre principale est coch�e
        With CheckBoxConfigWindow
            Select Case My.Settings.WindowState
                Case 1
                    .Checked = True
                Case Else
                    .Checked = False
            End Select
        End With

        ' Mise � jour auto
        If Not (Not datemaj Is Nothing AndAlso String.IsNullOrEmpty(datemaj)) Then
            datesave = CDate(datemaj)
            diffjourmaj = DateDiff(DateInterval.Day, datesave, Date.Now)
            ' diff�rence en jours
            jourrestantmaj = Str(intervalmaj - diffjourmaj)
            ' calcul du nombre de jours restant

            ' Si on a d�pass� la date de mise � jour --> jourrestant = 0
            If Val(jourrestantmaj) <= 0 Then
                jourrestantmaj = Str(0)
            End If

            ' On affiche le nombre de jours restant dans les pr�f�rences

            LabelUpdateIn.Text = jourrestantmaj & " " & lngUpdateIn
        End If

        If Not (Not datemaj Is Nothing AndAlso String.IsNullOrEmpty(datemaj)) Then

            ' On affiche la date de la prochaine mise � jour dans les pr�f�rences
            Dim jourplusmaj As Integer
            jourplusmaj = CInt(intervalmaj - diffjourmaj)
            Dim dmaj As Date = Date.Now.AddDays(jourplusmaj)
            TextBoxUpdateNext.Text = (dmaj.ToLongDateString).ToString

        End If

        ' v�rification auto
        If Not (Not datemajverif Is Nothing AndAlso String.IsNullOrEmpty(datemajverif)) Then
            datesaveverif = CDate(datemajverif)
            diffjourverif = DateDiff(DateInterval.Day, datesaveverif, Date.Now)

            ' diff�rence en jours
            jourrestantverif = Str(verifinterval - diffjourverif)

            ' calcul du nombre de jours restant
            ' Si on a d�pass� la date de mise � jour --> jourrestant = 0
            If Val(jourrestantverif) <= 0 Then
                jourrestantverif = Str(0)
            End If

            ' On affiche le nombre de jours restant dans les pr�f�rences
            If My.Settings.Language <> "Fran�ais" Then
                LabelCheckIn.Text = jourrestantverif & " " & lngCheckIn
            Else
                LabelCheckIn.Text = jourrestantverif & " " & CheckIn
            End If
        End If

        If Not (Not datemajverif Is Nothing AndAlso String.IsNullOrEmpty(datemajverif)) Then

            ' On affiche la date de la prochaine v�rification dans les pr�f�rences
            Dim jourplusverif As Integer
            jourplusverif = CInt(verifinterval - diffjourverif)
            Dim dverif As Date = Date.Now.AddDays(jourplusverif)
            TextBoxNextUpdate.Text = (dverif.ToLongDateString).ToString

        End If
        'on lire la source 
        If My.Settings.Epgdata Then
            RadioButtonEPGData.Checked = True
            RadioButtonXMLTV.Checked = False
            tbPin.Text = My.Settings.PinEpgdata
            NumericUpDownDownloadEpgdata.Value = My.Settings.EpgdataDownload
            GroupBoxEpgdata.Enabled = True
        Else
            RadioButtonEPGData.Checked = False
            RadioButtonXMLTV.Checked = True
            GroupBoxEpgdata.Enabled = False
        End If

        ' On affiche la taille de la bdd dans les pr�f�rences
        Dim FichierInfo As New FileInfo(BDDPath & "db_progs.db3")
        Dim TailleFichier As Integer = CInt(FichierInfo.Length)
        TailleFichier = CInt(TailleFichier/1024)
        TextBoxTailleBdd.Text = TailleFichier.ToString("##,###", CultureInfo.CurrentCulture) & " Ko"

        ' On va lire le volume de ZGuideTV.NET dans le mixer de Windows
        Dim VolumeLevel As Integer = AudioMixerHelper.GetVolume
        TrackBarMasterVolume.Value = CInt(VolumeLevel/6553.5)

        ' Combobox Gestion du Son
        ComboBoxRSS.Items.Clear()
        ComboBoxMessages.Items.Clear()
        ComboBoxReminder.Items.Clear()

        ' On scanne le r�pertoire 'Media' et on peuple dans les combobox
        Try
            Dim DirInfo As New DirectoryInfo(MediaPath)

            For Each FileInfo As FileInfo In DirInfo.GetFiles
                If FileInfo.Extension = ".wav" OrElse FileInfo.Extension = ".mid" OrElse FileInfo.Extension = ".mp3" Then
                    ' On affiche les fichiers wav, mid et mp3 trouv�s dans le r�pertoire 'Media' 
                    ComboBoxRSS.Items.Add(FileInfo.Name)
                    ComboBoxMessages.Items.Add(FileInfo.Name)
                    ComboBoxReminder.Items.Add(FileInfo.Name)
                End If
            Next
        Catch ex As IOException
            Trace.WriteLine(DateTime.Now & " " & ex.ToString)
        End Try

        ' On affiche les sons wav s�lectionn�s
        ComboBoxRSS.Text = My.Settings.RSSSound
        ComboBoxMessages.Text = My.Settings.MessagesSound
        ComboBoxReminder.Text = My.Settings.ReminderSound

        ' On va lire le volume du flux RSS
        TrackBarRSS.Value = CInt((My.Settings.RSSVolume + 3000)/300)

        ' On va lire le volume des messages
        TrackBarMessages.Value = CInt((My.Settings.MessagesVolume + 3000)/300)

        ' On va lire le volume des rappels des �missions
        TrackBarReminder.Value = CInt((My.Settings.ReminderVolume + 3000)/300)

        ' On va lire si le son est sur muet
        Select Case My.Settings.AudioOn
            Case True
                CheckBoxAudioOn.Checked = False
            Case Else
                CheckBoxAudioOn.Checked = True
        End Select
    End Sub

    Private Sub FrmPref_FormClosed(ByVal sender As Object, ByVal e As FormClosedEventArgs) Handles Me.FormClosed

        Dim FichierInfo As New FileInfo(BDDPath & "db_progs.db3")
        If My.Computer.FileSystem.FileExists(BDDPath & "db_progs.db3") Then
            Dim TailleFichier As Integer = CInt(FichierInfo.Length)
            TailleFichier = CInt(TailleFichier/1024)
            If TailleFichier < 4 Then
                Application.Exit()
            End If
        End If
        Me.Dispose()
    End Sub

    Private Sub CheckBoxAutoUpdate_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) _
        Handles CheckBoxAutoUpdate.CheckedChanged

        ' Si Autoupdate est coch� on active les autres groupbox
        Select Case CheckBoxAutoUpdate.Checked
            Case False
                GroupBoxIntervalle.Enabled = False
                GroupBoxUpdateIn.Enabled = False
                GroupBoxUpdateNext.Enabled = False
            Case Else
                GroupBoxIntervalle.Enabled = True
                GroupBoxUpdateIn.Enabled = True
                GroupBoxUpdateNext.Enabled = True
        End Select
    End Sub

    Private Sub CheckBoxResume_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) _
        Handles CheckBoxResume.CheckedChanged

        ' Si Autoupdate est coch� on active les autres groupbox et on met � jour l'image dans la StatusStrip
        Select Case CheckBoxResume.Checked
            Case False
                GroupBoxResumeBefore.Enabled = False
                Mainform.ToolStripStatusLabelWakeUp.Image = My.Resources.asterisk
            Case Else
                GroupBoxResumeBefore.Enabled = True
                Mainform.ToolStripStatusLabelWakeUp.Image = My.Resources.asteriskrouge
        End Select
    End Sub

    Private Sub CheckBoxAutoverif_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) _
        Handles CheckBoxAutoverif.CheckedChanged

        ' Si Autoverif est coch� on active les autres groupbox
        Select Case CheckBoxAutoverif.Checked
            Case False
                GroupBoxIntervalles.Enabled = False
                GroupBoxNextCheck.Enabled = False
                GroupBoxCheckIn.Enabled = False
            Case Else
                GroupBoxIntervalles.Enabled = True
                GroupBoxNextCheck.Enabled = True
                GroupBoxCheckIn.Enabled = True
        End Select
    End Sub

    Public Function ConnectionAvailable(ByVal strServer As String) As Boolean

        'rvs75 : 27/10/2010 : renvoie teste isOffline avant tout !!!!
        'evite de figer zg
        If Not (isOffline) Then
            Try
                Dim reqFP As HttpWebRequest = CType(HttpWebRequest.Create(strServer), HttpWebRequest)
                Dim rspFP As HttpWebResponse = CType(reqFP.GetResponse(), HttpWebResponse)

                If HttpStatusCode.OK = rspFP.StatusCode Then
                    ' HTTP = 200 - Connection Internet ou serveur disponible
                    reqFP.Abort()
                    rspFP.Close()
                    Return True
                Else
                    ' Autres status - Connection Internet ou serveur indisponible
                    reqFP.Abort()
                    rspFP.Close()
                    Return False
                End If

            Catch e1 As WebException
                ' Connection non disponible
                Return False
            End Try
        End If
    End Function

    Private Sub btSAbonner_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BoutonSAbonner.Click
        EPGDataSubscription.ShowDialog()
    End Sub

    Private Sub RadioButtonXMLTV_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles RadioButtonXMLTV.CheckedChanged
        GroupBoxEpgdata.Enabled = False
    End Sub

    Private Sub RadioButtonEPGData_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles RadioButtonEPGData.CheckedChanged
        GroupBoxEpgdata.Enabled = True
    End Sub

    Private Sub ButtonRSSSound_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ButtonRSSSound.Click
        Try
            ' le volume va de 0 (max) � -3000 (min)
            Dim AudioPlay As Audio
            AudioPlay = New Audio(MediaPath & ComboBoxRSS.Text(), True)
            AudioPlay.Volume = - 3000 + (TrackBarRSS.Value*300)
            AudioPlay.Play()
        Catch ex As DirectXException
            Trace.WriteLine(DateTime.Now & " " & ex.ToString)
        End Try
    End Sub

    Private Sub ButtonMessagesSound_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ButtonMessagesSound.Click
        Try
            ' le volume va de 0 (max) � -3000 (min)
            Dim AudioPlay As Audio
            AudioPlay = New Audio(MediaPath & ComboBoxMessages.Text(), True)
            AudioPlay.Volume = - 3000 + (TrackBarMessages.Value*300)
            AudioPlay.Play()
        Catch ex As DirectXException
            Trace.WriteLine(DateTime.Now & " " & ex.ToString)
        End Try
    End Sub

    Private Sub ButtonReminderSound_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ButtonReminderSound.Click
        Try
            ' le volume va de 0 (max) � -3000 (min)
            Dim AudioPlay As Audio
            AudioPlay = New Audio(MediaPath & ComboBoxReminder.Text(), True)
            AudioPlay.Volume = - 3000 + (TrackBarReminder.Value*300)
            AudioPlay.Play()
        Catch ex As DirectXException
            Trace.WriteLine(DateTime.Now & " " & ex.ToString)
        End Try
    End Sub

    Private Sub TrackBarMasterVolume_MouseUp(ByVal sender As Object, ByVal e As EventArgs) Handles TrackBarMasterVolume.MouseUp

        Try
            ' On va �crire le volume de ZGuideTV.NET
            AudioMixerHelper.SetVolume(CInt(TrackBarMasterVolume.Value*6553.5))
        Catch ex As Exception
            'On �vite que cela plante dans AudioMixerHelper !!!
        End Try

        Try
            ' le volume va de 0 (max) � -3000 (min)
            Dim AudioPlay As Audio
            AudioPlay = New Audio(MediaPath & "sound3.mp3", True)
            AudioPlay.Volume = 0
            AudioPlay.Play()
        Catch ex As DirectXException
            Trace.WriteLine(DateTime.Now & " " & ex.ToString)
        End Try
    End Sub
End Class
