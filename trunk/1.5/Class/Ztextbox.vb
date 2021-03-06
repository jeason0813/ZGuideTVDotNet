﻿' ReSharper disable once CheckNamespace
Public Class Ztextbox
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()
        ' Add any initialization after the InitializeComponent() call.
        SetStyle( _
             ControlStyles.UserPaint Or ControlStyles.AllPaintingInWmPaint Or _
             ControlStyles.OptimizedDoubleBuffer, True)
        AutoSize = False
        AutoScaleMode = Windows.Forms.AutoScaleMode.None
    End Sub
    Private _text As String = String.Empty
    Public Overrides Property Text As String
        Set(value As String)
            _text = value
            Refresh()
        End Set
        Get
            Return _text
        End Get
    End Property

    Protected Overrides Sub OnPaint(e As Windows.Forms.PaintEventArgs)
        Try
            Dim drawingArea As Graphics = e.Graphics
            With drawingArea
                .DrawLine(New Pen(Color.Black), New Point(0, CInt(Height / 1.5)), New Point(0, Height - 1))
                .DrawLine(New Pen(Color.Black), New Point(0, Height - 1), New Point(Width, Height - 1))
                Dim f As New StringFormat
                Dim r As New Rectangle(0, 0, Width, Height)
                f.Alignment = StringAlignment.Near
                f.LineAlignment = StringAlignment.Far
                Dim fo As New Font(Font.FontFamily, 8.5)
                .DrawString(" " & _text, fo, Brushes.Black, r, f)
                f.Dispose()
                fo.Dispose()
            End With
        Catch ex As Exception

        End Try
    End Sub
    Protected Overrides Sub OnResize(e As EventArgs)
        MyBase.OnResize(e)
        Refresh()
    End Sub
End Class
