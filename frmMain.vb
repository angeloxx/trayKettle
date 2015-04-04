Imports System
Imports System.Net
Imports System.Net.Sockets
Imports System.Threading
Imports System.Text

Public Class frmMain

    ' ManualResetEvent instances signal completion.
    Private Shared connectDone As New ManualResetEvent(False)
    Private Shared sendDone As New ManualResetEvent(False)
    Private Shared receiveDone As New ManualResetEvent(False)

    ' The response from the remote device.
    Private Shared response As String = String.Empty
    Private ip As String = "127.0.0.1"
    Dim client As New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
    Private lastTemperature As String
    Private lastIsReady As Boolean

    Private Sub frmMain_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Me.Hide()
        Me.ShowInTaskbar = False
        notifyIcon.Visible = True

        Dim cmdLine As New TCommandLineParser
        With cmdLine
            .AddString("ip", False)
        End With

        Try
            cmdLine.AddItems(My.Application.CommandLineArgs.ToArray)
            If Not cmdLine.Validate() Then
                notifyIcon.ShowBalloonTip(30000, "Teapot", "Command line error", ToolTipIcon.Error)
            End If
            If cmdLine.FindValue("ip") Then ip = cmdLine.GetString("ip")
        Catch exp As Exception
            notifyIcon.ShowBalloonTip(30000, "Teapot", "Command line big error:" + vbCrLf + exp.ToString, ToolTipIcon.Error)
            ip = ""
        End Try

        If ip <> "" Then
            client.BeginConnect(New IPEndPoint(IPAddress.Parse(ip), 2000), New AsyncCallback(AddressOf ConnectCallback), client)
            connectDone.WaitOne()

            Receive(client)
            Send(client, "get sys status" + vbCr)
        End If
    End Sub

    Private Sub ConnectCallback(ByVal ar As IAsyncResult)
        ' Retrieve the socket from the state object.
        Dim client As Socket = CType(ar.AsyncState, Socket)

        ' Complete the connection.
        Try
            client.EndConnect(ar)
            connectDone.Set()
        Catch x As SocketException
            notifyIcon.ShowBalloonTip(30000, "Teapot", "Unable to connect to " + ip + ", program will die\nSpecify the ip address with /ip=xx.xx.xx.xx command line", ToolTipIcon.Error)
        End Try

        ' Signal that the connection has been made.
    End Sub 'ConnectCallback

    Private Sub Receive(ByVal client As Socket)

        ' Create the state object.
        Dim state As New StateObject
        state.workSocket = client

        ' Begin receiving the data from the remote device.
        client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, New AsyncCallback(AddressOf ReceiveCallback), state)
    End Sub 'Receive


    Private Sub ReceiveCallback(ByVal ar As IAsyncResult)

        ' Retrieve the state object and the client socket 
        ' from the asynchronous state object.
        Dim state As StateObject = CType(ar.AsyncState, StateObject)
        Dim client As Socket = state.workSocket

        ' Read data from the remote device.
        Dim bytesRead As Integer = client.EndReceive(ar)

        If bytesRead > 0 Then
            ' There might be more data, so store the data received so far.
            ' state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead))
            Evaluate(Encoding.ASCII.GetString(state.buffer, 0, bytesRead))

            ' Get the rest of the data.
            client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, New AsyncCallback(AddressOf ReceiveCallback), state)
        Else
            ' All the data has arrived; put it in response.
            If state.sb.Length > 1 Then
                response = state.sb.ToString()
            End If
            ' Signal that all bytes have been received.
            receiveDone.Set()
        End If
    End Sub 'ReceiveCallback

    Private Sub updateStatus(_message As String, _timeout_s As Integer, tipIcon As ToolTipIcon, _lastTemperature As Integer, _lastIsReady As Integer)


        REM Ignore _lastIsReady = -1
        If _lastIsReady = 1 Then lastIsReady = True
        If _lastIsReady = 0 Then lastIsReady = False

        If _lastTemperature > 0 Then
            lastTemperature = _lastTemperature & " C"
        End If


        notifyIcon.Text = "trayKettle - " & _message
        notifyIcon.ShowBalloonTip(_timeout_s * 1000, "Teapot", _message, tipIcon)
    End Sub

    Private Sub Evaluate(_in As String)
        Dim value As Integer

        REM Console.WriteLine(_in)
        If _in.StartsWith("sys status key=") Then
            value = Asc(_in.Substring(15, 1))
            Select Case value
                Case 3
                    updateStatus("On and warming", 30, ToolTipIcon.Info, -1, -1)
                Case 5
                    updateStatus("On at 65 C", 30, ToolTipIcon.Info, 65, 0)
                Case 9
                    updateStatus("On at 80 C", 30, ToolTipIcon.Info, 80, 0)
                Case 17
                    updateStatus("On at 95 C", 30, ToolTipIcon.Info, 95, 0)
                Case 33
                    updateStatus("On at 100 C", 30, ToolTipIcon.Info, 100, 0)
                Case Else
                    updateStatus("Idle", 30, ToolTipIcon.Info, -1, -1)
            End Select
            Return
        End If

        Console.WriteLine(Trim(_in.Substring(11)))
        If _in.StartsWith("sys status") Then
            Select Case Trim(_in.Substring(11, _in.Length - 12))
                Case "0x5"
                    updateStatus("Powered on", 30, ToolTipIcon.Info, -1, -1)
                Case "0x0"
                    If lastIsReady Then
                        updateStatus("Powered off and ready at " & lastTemperature, 30, ToolTipIcon.Warning, -1, -1)
                        My.Computer.Audio.PlaySystemSound(System.Media.SystemSounds.Asterisk)
                    Else
                        updateStatus("Powered off", 30, ToolTipIcon.Info, -1, -1)
                    End If
                Case "0x100"
                    updateStatus("Powered on at 100 C", 30, ToolTipIcon.Info, 100, 0)
                Case "0x95"
                    updateStatus("Powered on at 95 C", 30, ToolTipIcon.Info, 95, 0)
                Case "0x80"
                    updateStatus("Powered on at 80 C", 30, ToolTipIcon.Info, 80, 0)
                Case "0x65"
                    updateStatus("Powered on at 65 C", 30, ToolTipIcon.Info, 65, 0)
                Case "0x11"
                    updateStatus("Powered on and warming", 30, ToolTipIcon.Info, -1, -1)
                Case "0x8005"
                    updateStatus("Powered on and Warming for 5 minutes", 30, ToolTipIcon.Info, -1, -1)
                Case "0x8010"
                    updateStatus("Powered on and Warming for 10 minutes", 30, ToolTipIcon.Info, -1, -1)
                Case "0x8020"
                    updateStatus("Powered on and Warming for 20 minutes", 30, ToolTipIcon.Info, -1, -1)
                Case "0x10"
                    updateStatus("Powered on and Warmed", 30, ToolTipIcon.Info, -1, -1)
                Case "0x3"
                    updateStatus("Ready", 30, ToolTipIcon.Info, -1, 1)
                Case "0x2"
                    updateStatus("Problems", 30, ToolTipIcon.Error, -1, -1)
                Case "0x1"
                    updateStatus("Kettle is removed", 30, ToolTipIcon.Warning, -1, -1)
            End Select

            Return
        End If
    End Sub

    Private Shared Sub Send(ByVal client As Socket, ByVal data As String)
        ' Convert the string data to byte data using ASCII encoding.
        Dim byteData As Byte() = Encoding.ASCII.GetBytes(data)

        ' Begin sending the data to the remote device.
        client.BeginSend(byteData, 0, byteData.Length, 0, New AsyncCallback(AddressOf SendCallback), client)
    End Sub 'Send


    Private Shared Sub SendCallback(ByVal ar As IAsyncResult)
        ' Retrieve the socket from the state object.
        Dim client As Socket = CType(ar.AsyncState, Socket)

        ' Complete sending the data to the remote device.
        Dim bytesSent As Integer = client.EndSend(ar)
        Console.WriteLine("Sent {0} bytes to server.", bytesSent)

        ' Signal that all bytes have been sent.
        sendDone.Set()
    End Sub

    Private Sub notifyIcon_BalloonTipClosed(sender As Object, e As System.EventArgs) Handles notifyIcon.BalloonTipClosed
        If Not client.Connected Then
            End
        End If
    End Sub

    Private Sub notifyIcon_MouseClick(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles notifyIcon.MouseClick
        If e.Button = Windows.Forms.MouseButtons.Right Then 'Checks if the pressed button is the Right Mouse
            menu.Show(Cursor.Position)
        End If
    End Sub 'SendCallback

    Private Sub StartTo100CToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles StartTo100CToolStripMenuItem.Click
        Send(client, "set sys output 0x4" + vbCr)
        Send(client, "set sys output 0x80" + vbCr)
    End Sub

    Private Sub StartTo95CToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles StartTo95CToolStripMenuItem.Click
        Send(client, "set sys output 0x4" + vbCr)
        Send(client, "set sys output 0x2" + vbCr)
    End Sub

    Private Sub StartTo80CToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles StartTo80CToolStripMenuItem.Click
        Send(client, "set sys output 0x4" + vbCr)
        Send(client, "set sys output 0x4000" + vbCr)
    End Sub

    Private Sub StartTo65CToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles StartTo65CToolStripMenuItem.Click
        Send(client, "set sys output 0x4" + vbCr)
        Send(client, "set sys output 0x200" + vbCr)
    End Sub

    Private Sub StopToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles StopToolStripMenuItem.Click
        Send(client, "set sys output 0x0" + vbCr)
    End Sub

    Private Sub menu_LostFocus(sender As Object, e As System.EventArgs) Handles menu.LostFocus
        menu.Hide()
    End Sub
    Private Sub menu_Opening(sender As System.Object, e As System.ComponentModel.CancelEventArgs) Handles menu.Opening

    End Sub

    Private Sub ExitToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles ExitToolStripMenuItem.Click
        notifyIcon.Visible = False
        End
    End Sub
End Class

Public Class StateObject
    ' Client socket.
    Public workSocket As Socket = Nothing
    ' Size of receive buffer.
    Public Const BufferSize As Integer = 256
    ' Receive buffer.
    Public buffer(BufferSize) As Byte
    ' Received data string.
    Public sb As New StringBuilder
End Class 'StateObject
