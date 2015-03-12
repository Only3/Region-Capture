
    Class RegionCapture
        Inherits Form

        Private UpLoad As Boolean
        Public SaveName As String = Environ("UserProfile") & "\Desktop\Img.png"

        Sub New(Optional Up As Boolean = False)
            UpLoad = Up
            DoubleBuffered = True
            WindowState = FormWindowState.Minimized
            Cursor = Cursors.Cross
            Show()
            Opacity = 0.4
            TopMost = True
            FormBorderStyle = FormBorderStyle.None
            WindowState = FormWindowState.Maximized
            RegisterHotKey(Handle, 1, Nothing, Keys.Escape)
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
            Draw = False
            Opacity = 0
            Cursor = Cursors.Default
            WindowState = FormWindowState.Normal
            Dim Bm As New Bitmap(Rect.Width, Rect.Height)
            Dim G As Graphics = Graphics.FromImage(Bm)
            G.CopyFromScreen(Rect.X, Rect.Y, 0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy)
            If Not UpLoad Then
                Bm.Save(SaveName, Imaging.ImageFormat.Png)
            Else
                Dim boundary As String = DateTime.Now.Ticks.ToString("x")
                Dim Byt() As Byte = System.Text.Encoding.ASCII.GetBytes(String.Format("---------{1}{0}Content-Type: image/png{0}Content-Disposition: form-data; name=fileupload; filename=img.png{0}{0}", vbCrLf, boundary))
                Dim Ms As New IO.MemoryStream
                Ms.Write(Byt, 0, Byt.Length)
                Bm.Save(Ms, Imaging.ImageFormat.Png)
                Byt = System.Text.Encoding.ASCII.GetBytes(String.Format("{0}---------{1}{0}Content-Disposition: form-data; name=key{0}{0}AOLZTMKP15a6c425b9ece5467e6f6357eb53dae4{0}---------{1}", vbCrLf, boundary))
                Ms.Write(Byt, 0, Byt.Length)
                Dim Req As Net.HttpWebRequest = Net.HttpWebRequest.Create("http://imageshack.us/upload_api.php")
                Req.ContentType = "multipart/form-data; boundary=-------" + boundary
                Req.Method = "POST"
                Req.ContentLength = Ms.Length
                Dim Stream As IO.Stream = Req.GetRequestStream()
                Ms.Seek(0, IO.SeekOrigin.Begin)
                Ms.WriteTo(Stream)
                Stream.Close()
                Ms.Close()
                Process.Start(Split(Split(New IO.StreamReader(Req.GetResponse.GetResponseStream).ReadToEnd, "<image_link>")(1), "</")(0))
            End If
            Close()
            MyBase.OnMouseUp(e)
        End Sub
        Protected Overrides Sub OnClosed(e As EventArgs)
            UnregisterHotKey(Handle, 1)
            MyBase.OnClosed(e)
        End Sub
        Protected Overrides Sub WndProc(ByRef m As Message)
            If m.Msg = &H312 Then
                Select Case m.WParam
                    Case 1
                        Close()
                End Select
            End If
            MyBase.WndProc(m)
        End Sub
        Private Declare Function RegisterHotKey Lib "user32" (ByVal hwnd As IntPtr, ByVal id As Integer, ByVal fsModifiers As Integer, ByVal vk As Integer) As Integer
        Private Declare Function UnregisterHotKey Lib "user32" (ByVal hwnd As IntPtr, ByVal id As Integer) As Integer
    End Class
