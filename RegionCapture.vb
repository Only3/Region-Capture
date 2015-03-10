
Class RegionCapture
    Inherits Form

    Public SaveName As String = Environ("UserProfile") & "\Desktop\Img.png"

    Sub New()
        DoubleBuffered = True
        WindowState = FormWindowState.Minimized
        Cursor = Cursors.Cross
        Show()
        Opacity = 0.4
        TopMost = True
        FormBorderStyle = FormBorderStyle.None
        WindowState = FormWindowState.Maximized
    End Sub

    Private SX, SY, CX, CY As Single
    Private Rect As Rectangle, Draw As Boolean = False
    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        If Draw Then
            Rect = New Rectangle(Math.Min(SX, CX), Math.Min(SY, CY), Math.Abs(SX - CX), Math.Abs(SY - CY))
            e.Graphics.DrawRectangle(New Pen(Brushes.Black, 2) With {.DashStyle = Drawing2D.DashStyle.Dash}, Rect)
            e.Graphics.DrawString(Rect.Width & " - " & Rect.Height, Font, Brushes.Blue, New Point(Rect.X + Rect.Width - 60, Rect.Y + Rect.Height + 10))
        End If
        MyBase.OnPaint(e)
    End Sub
    Protected Overrides Sub OnMouseDown(e As MouseEventArgs)
        Draw = True
        SX = e.X
        SY = e.Y
        MyBase.OnMouseDown(e)
    End Sub
    Protected Overrides Sub OnMouseMove(e As MouseEventArgs)
        If Draw Then
            CX = e.X
            CY = e.Y
            Invalidate()
        End If
        MyBase.OnMouseMove(e)
    End Sub
    Protected Overrides Sub OnMouseUp(e As MouseEventArgs)
        Cursor = Cursors.Default
        Draw = False
        Opacity = 0
        Dim Bm As New Bitmap(Rect.Width, Rect.Height) ', Imaging.PixelFormat.Format32bppArgb)
        Dim G As Graphics = Graphics.FromImage(Bm)
        G.CopyFromScreen(Rect.X, Rect.Y, 0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy)
        Bm.Save(SaveName, Imaging.ImageFormat.Png)
        Close()
        MyBase.OnMouseUp(e)
    End Sub
    Protected Overrides Sub OnKeyDown(e As KeyEventArgs)
        If e.KeyCode = Keys.Escape Then Close()
        MyBase.OnKeyDown(e)
    End Sub
End Class
