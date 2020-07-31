Imports System.Runtime.InteropServices
Imports System.Threading
Imports GMap.NET
Imports GMap.NET.MapProviders
Imports GMap.NET.WindowsForms
Imports System.Drawing
Imports System.Drawing.Drawing2D

Public Class Form1
    Dim takeoffLat As Double
    Dim takeoffLon As Double
    Dim takeoffAlt As Double

    Dim homeLat As Double
    Dim homeLon As Double


    Dim currentAlt As Double
    Dim currentLat As Double
    Dim currentLon As Double
    Dim heading As Double
    Dim rroll As Integer
    Dim rpitch As Integer

    Dim lastLat As Double
    Dim lastLon As Double
    Dim lastAlt As Double
    Dim distanceTraveled As Double


    Dim targetLat() As Double
    Dim targetLon() As Double
    Dim targetAlt() As Double
    Dim lightselector As Integer
    Dim autoNavState As Boolean

    ''glider defs
    Dim gliderMode As Boolean
    Dim releasedBalloon As Boolean
    Dim deployedParachute As Boolean
    Dim errorFlash As Boolean = False



    Dim rawRecvStr As String = ""
    Dim com As String
    Dim comThread As System.Threading.Thread
    Dim validString As Boolean
    Dim firstGPS As Boolean = False
    Dim overlayOne As New GMapOverlay("GMapControl1")
    Dim overlayTwo As New GMapOverlay("GMapControl1")
    Dim line_layer As New GMapRoute("GMapControl1")

    Dim dropOverlay As New GMapOverlay("GMapControl1")
    Dim homeoverlay As New GMapOverlay("GMapControl1")


    Public Structure waypoint
        Public lat As Double
        Public lon As Double
        Public alt As Double
    End Structure
    Dim waypoint1, waypoint2, waypoint3, waypoint4, waypoint5 As waypoint
    Dim com1 As IO.Ports.SerialPort = Nothing





    Private Sub Button14_Click(sender As Object, e As EventArgs) Handles Button14.Click
        'tail lights
        Button14.BackColor = Color.White
        Button12.BackColor = Color.Blue
        Button13.BackColor = Color.Blue
        Button15.BackColor = Color.Blue
        lightselector = 0
        TextBox5.Text = "255"
        TextBox6.Text = "255"
        TextBox7.Text = "255"
        TextBox8.Text = "255"
        TextBox9.Text = "255"
        TextBox10.Text = "255"
        TextBox11.Text = "255"
        TextBox12.Text = "255"
        TextBox13.Text = "255"

    End Sub

    Private Sub Button12_Click(sender As Object, e As EventArgs) Handles Button12.Click
        'r wing lights
        Button14.BackColor = Color.Blue
        Button12.BackColor = Color.White
        Button13.BackColor = Color.Blue
        Button15.BackColor = Color.Blue
        lightselector = 1
        TextBox5.Text = "255"
        TextBox6.Text = "255"
        TextBox7.Text = "255"
        TextBox8.Text = "255"
        TextBox9.Text = "255"
        TextBox10.Text = "255"
        TextBox11.Text = "255"
        TextBox12.Text = "255"
        TextBox13.Text = "255"

    End Sub

    Private Sub Button13_Click(sender As Object, e As EventArgs) Handles Button13.Click
        'l wing ligts
        Button14.BackColor = Color.Blue
        Button12.BackColor = Color.Blue
        Button13.BackColor = Color.White
        Button15.BackColor = Color.Blue
        lightselector = 2
        TextBox5.Text = "255"
        TextBox6.Text = "255"
        TextBox7.Text = "255"
        TextBox8.Text = "255"
        TextBox9.Text = "255"
        TextBox10.Text = "255"
        TextBox11.Text = "255"
        TextBox12.Text = "255"
        TextBox13.Text = "255"
    End Sub

    Private Sub Button15_Click(sender As Object, e As EventArgs) Handles Button15.Click
        'nose lights
        Button14.BackColor = Color.Blue
        Button12.BackColor = Color.Blue
        Button13.BackColor = Color.Blue
        Button15.BackColor = Color.White
        lightselector = 3
        TextBox5.Text = "255"
        TextBox6.Text = "255"
        TextBox7.Text = "255"
        TextBox8.Text = "255"
        TextBox9.Text = "255"
        TextBox10.Text = "255"
        TextBox11.Text = "255"
        TextBox12.Text = "255"
        TextBox13.Text = "255"

    End Sub

    Private Sub Button11_Click(sender As Object, e As EventArgs) Handles Button11.Click
        'set lights
        Select Case lightselector
            Case 0
                'tail lights

            Case 1
                'r wing
            Case 2
                'l wing
            Case 3
                'nose
        End Select

    End Sub

    Function getDistanceToWaypoints() As String

        If waypoint1.alt > 0 Then
            TextBox14.Text = Math.Round(distance(currentLat, currentLon, waypoint1.lat, waypoint1.lon, "M") * 1609, 2, MidpointRounding.AwayFromZero) 'to meters

        End If
        If waypoint2.alt > 0 Then
            TextBox15.Text = Math.Round(distance(currentLat, currentLon, waypoint2.lat, waypoint2.lon, "M") * 1609, 2, MidpointRounding.AwayFromZero) 'to meters

        End If
        If waypoint3.alt > 0 Then
            TextBox16.Text = Math.Round(distance(currentLat, currentLon, waypoint3.lat, waypoint3.lon, "M") * 1609, 2, MidpointRounding.AwayFromZero) 'to meters

        End If
        If waypoint4.alt > 0 Then
            TextBox17.Text = Math.Round(distance(currentLat, currentLon, waypoint4.lat, waypoint4.lon, "M") * 1609, 2, MidpointRounding.AwayFromZero) 'to meters

        End If
        If waypoint5.alt > 0 Then
            TextBox19.Text = Math.Round(distance(currentLat, currentLon, waypoint5.lat, waypoint5.lon, "M") * 1609, 2, MidpointRounding.AwayFromZero) 'to meters

        End If
        Return 1

    End Function



    Function displayGPS() As String
        overlayOne.Clear()

        overlayOne.Markers.Add(New GMap.NET.WindowsForms.Markers.GMarkerGoogle(New PointLatLng(currentLat, currentLon), Markers.GMarkerGoogleType.blue_dot))
        'GMapControl1.Overlays.Add(overlayOne)
        GMapControl1.Overlays.Add(overlayOne)

        'line_layer = New GMapRoute("single_line")
        'line_layer.Stroke = New Pen(Brushes.Black, 2)


        'line_layer.Points.Add(New PointLatLng(currentLat, currentLon))

        'overlayTwo.Routes.Add(line_layer)
        'GMapControl1.Overlays.Add(overlayTwo)

        'GMapControl1.RoutesEnabled = True
        'GMapControl1.UpdateRouteLocalPosition(line_layer)
        'GMapControl1.Overlays.Add(overlayTwo)

        Dim marker_ As GMapMarker
        marker_ = New GMap.NET.WindowsForms.Markers.GMarkerGoogle(New PointLatLng(currentLat, currentLon), Markers.GMarkerGoogleType.blue_dot)
        'overlayTwo.Markers.Add(marker_)
        getDistanceToWaypoints()

        Return 1



    End Function

    Function ParseReceivedData(raw As String) As String

        Dim dataSplitRaw() As String
        dataSplitRaw = raw.Split(" ")

        Dim GPSdata As String
        Dim NAVdata As String
        lastLat = currentLat
        lastLon = currentLon

        If lastAlt > currentAlt Then
            'up
            PictureBox11.Visible = False
            PictureBox12.Visible = True
        ElseIf lastAlt < currentAlt Then
            'down
            PictureBox11.Visible = True
            PictureBox12.Visible = False
        ElseIf currentAlt = 0 Then
            PictureBox11.Visible = False
            PictureBox12.Visible = False
        End If
        lastAlt = currentAlt



        'gps proc
        GPSdata = dataSplitRaw(1)
        Dim gpssplit() As String
        gpssplit = GPSdata.Split(",")
        If firstGPS = False Then
            takeoffAlt = gpssplit(2)
            takeoffLat = gpssplit(0)
            takeoffAlt = gpssplit(1)
            firstGPS = True
        End If
        currentAlt = gpssplit(2)
        currentLat = gpssplit(0)
        currentLon = gpssplit(1)

        TextBox1.Text = (String.Format("{0:0.0}", currentAlt - takeoffAlt)).ToString + "m"
        TextBox2.Text = (String.Format("{0:0.0}", currentAlt)).ToString + "m"
        TextBox18.Text = gpssplit(3)

        'nav proc
        NAVdata = dataSplitRaw(3)
        Dim navsplit() As String
        navsplit = NAVdata.Split(",")
        heading = navsplit(0)
        rroll = navsplit(1)
        rpitch = navsplit(2)

        If (navsplit(3) = "111") Then
            'GLIDER MODE
            GroupBox8.Visible = True 'Glider state
            PictureBox6.Visible = True


            GroupBox5.Visible = False
            GroupBox2.Visible = False
            GroupBox6.Visible = False
            If navsplit(4) = 1 Then
                'released balloon
                TextBox25.Text = "Balloon Released"
                PictureBox6.Visible = False
                PictureBox7.Visible = True
                PictureBox8.Visible = True
                PictureBox10.Visible = False


            End If
            If navsplit(5) = 1 Then
                'deployed parachute
                TextBox26.Text = "Parachute Deployed"
                PictureBox7.Visible = False
                PictureBox8.Visible = False
                PictureBox10.Visible = True
            End If




        ElseIf (navsplit(3) = "000") Then
            GroupBox5.Visible = True
            GroupBox2.Visible = True
            GroupBox6.Visible = True
            GroupBox8.Visible = False 'Glider state
        Else
            'GroupBox5.Visible = False
            'GroupBox2.Visible = False
            'GroupBox6.Visible = False
            'GroupBox8.Visible = False 'Glider state

        End If




        TextBox4.Text = navsplit(0)
        TextBox23.Text = navsplit(1)
        TextBox24.Text = navsplit(2)
        'rotate uav symbol 
        PictureBox2.Invalidate()
        PictureBox4.Invalidate()
        PictureBox5.Invalidate()

        distanceTraveled = distanceTraveled + distance(currentLat, currentLon, lastLat, lastLon, "M")

        If homeLat <> 0 Then
            TextBox21.Text = Math.Round(distance(currentLat, currentLon, homeLat, homeLon, "M") * 1609, 2, MidpointRounding.AwayFromZero)

        End If






        Return 1

    End Function

    ''rotate plane image for nav


    Function findWaypoints() As String
        Dim point1 As String
        Dim point2 As String
        Dim point3 As String
        Dim point4 As String
        Dim point5 As String

        dropOverlay.Clear()

        point1 = wptbox1.Text
        point2 = wptbox2.Text
        point3 = wptbox3.Text
        point4 = wptbox4.Text
        point5 = wptbox5.Text

        Dim datasplit() As String
        Dim tempLat As Double
        Dim tempLon As Double
        Dim tempAlt As Double


        If wptbox1.Text Is "" Then
        Else


            datasplit = Split(point1, ",")

            If Double.TryParse(datasplit(0), tempLat) Then

            End If

            If Double.TryParse(datasplit(1), tempLon) Then

            End If

            If Double.TryParse(datasplit(2), tempAlt) Then

            End If
            waypoint1.lat = tempLat
            waypoint1.lon = tempLon
            waypoint1.alt = tempAlt

        End If
        If wptbox2.Text Is "" Then
        Else


            datasplit = Split(point2, ",")

            If Double.TryParse(datasplit(0), tempLat) Then

            End If

            If Double.TryParse(datasplit(1), tempLon) Then

            End If

            If Double.TryParse(datasplit(2), tempAlt) Then

            End If
            waypoint2.lat = tempLat
            waypoint2.lon = tempLon
            waypoint2.alt = tempAlt

        End If
        If wptbox3.Text Is "" Then
        Else

            datasplit = Split(point3, ",")

            If Double.TryParse(datasplit(0), tempLat) Then

            End If

            If Double.TryParse(datasplit(1), tempLon) Then

            End If

            If Double.TryParse(datasplit(2), tempAlt) Then

            End If
            waypoint3.lat = tempLat
            waypoint3.lon = tempLon
            waypoint3.alt = tempAlt
        End If

        If wptbox4.Text Is "" Then
        Else

            datasplit = Split(point4, ",")

            If Double.TryParse(datasplit(0), tempLat) Then

            End If

            If Double.TryParse(datasplit(1), tempLon) Then

            End If

            If Double.TryParse(datasplit(2), tempAlt) Then

            End If
            waypoint4.lat = tempLat
            waypoint4.lon = tempLon
            waypoint4.alt = tempAlt
        End If
        If wptbox5.Text Is "" Then
        Else

            datasplit = Split(point5, ",")

            If Double.TryParse(datasplit(0), tempLat) Then

            End If

            If Double.TryParse(datasplit(1), tempLon) Then

            End If

            If Double.TryParse(datasplit(2), tempAlt) Then

            End If
            waypoint5.lat = tempLat
            waypoint5.lon = tempLon
            waypoint5.alt = tempAlt


        End If

        dropOverlay.Markers.Add(New GMap.NET.WindowsForms.Markers.GMarkerGoogle(New PointLatLng(waypoint1.lat, waypoint1.lon), Markers.GMarkerGoogleType.orange_dot))
        dropOverlay.Markers.Add(New GMap.NET.WindowsForms.Markers.GMarkerGoogle(New PointLatLng(waypoint2.lat, waypoint2.lon), Markers.GMarkerGoogleType.orange_dot))
        dropOverlay.Markers.Add(New GMap.NET.WindowsForms.Markers.GMarkerGoogle(New PointLatLng(waypoint3.lat, waypoint3.lon), Markers.GMarkerGoogleType.orange_dot))
        dropOverlay.Markers.Add(New GMap.NET.WindowsForms.Markers.GMarkerGoogle(New PointLatLng(waypoint4.lat, waypoint4.lon), Markers.GMarkerGoogleType.orange_dot))
        dropOverlay.Markers.Add(New GMap.NET.WindowsForms.Markers.GMarkerGoogle(New PointLatLng(waypoint5.lat, waypoint5.lon), Markers.GMarkerGoogleType.orange_dot))

        GMapControl1.Overlays.Add(dropOverlay)

        Return 1




    End Function

    Function ReceiveSerialData() As String
        ' Receive strings from a serial port.
        Dim returnStr As String = ""
        Dim strSplit() As String


        Try
            com1 = My.Computer.Ports.OpenSerialPort(com)
            com1.BaudRate = 57600
            com1.ReadTimeout = 10000
            Do
                Dim Incoming As String = com1.ReadLine()

                If Incoming.StartsWith("ERR") Then
                    Label16.Text = Incoming

                    GroupBox8.BackColor = Color.Red
                    strSplit = Incoming.Split(":")
                    If strSplit(1) = "100" Then
                        'balloon fail to release
                        PictureBox6.Visible = False
                        PictureBox9.Visible = True
                    End If
                    If strSplit(1) = "110" Then
                        'parachute fail to deploy
                    End If

                ElseIf Incoming.StartsWith("GPS") Then
                    TextBox3.Text = Incoming


                    strSplit = Incoming.Split(" ")

                    If strSplit(0) = "GPS:" Then
                        validString = True
                        TextBox3.BackColor = Color.LimeGreen
                        ParseReceivedData(Incoming)
                        displayGPS()

                    Else
                        validString = False
                        TextBox3.BackColor = Color.Red

                    End If
                ElseIf Incoming.StartsWith("INF") Then
                    Label16.Text = Incoming
                Else
                    validString = False
                    TextBox3.BackColor = Color.Red

                End If

                If Incoming Is Nothing Then
                    Exit Do
                End If
            Loop
        Catch ex As TimeoutException
            returnStr = "Error: Serial Port read timed out."
        Finally
            If com1 IsNot Nothing Then com1.Close()
        End Try


        Return returnStr

    End Function


    Public Function distance(ByVal lat1 As Double, ByVal lon1 As Double, ByVal lat2 As Double, ByVal lon2 As Double, ByVal unit As Char) As Double
        If lat1 = lat2 And lon1 = lon2 Then
            Return 0
        Else
            Dim theta As Double = lon1 - lon2
            Dim dist As Double = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta))
            dist = Math.Acos(dist)
            dist = rad2deg(dist)
            dist = dist * 60 * 1.1515

            Return dist
        End If
    End Function

    Private Function deg2rad(ByVal deg As Double) As Double
        Return (deg * Math.PI / 180.0)
    End Function

    Private Function rad2deg(ByVal rad As Double) As Double
        Return rad / Math.PI * 180.0
    End Function



    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        PictureBox2.Invalidate()



        PictureBox1.Image = UAV_Mapper_and_control.My.Resources.uavlights1
        ''PictureBox2.Image = UAV_Mapper_and_control.My.Resources.plane
        PictureBox3.Image = UAV_Mapper_and_control.My.Resources.out_plane

        PictureBox6.Visible = False
        PictureBox7.Visible = False
        PictureBox8.Visible = False
        PictureBox9.Visible = False
        PictureBox10.Visible = False
        Label17.Visible = False
        Button20.Visible = False
        Button21.Visible = False




        'color lighting buttons
        Button14.BackColor = Color.Blue
        Button12.BackColor = Color.Blue
        Button13.BackColor = Color.Blue
        Button15.BackColor = Color.Blue
        Me.CheckForIllegalCrossThreadCalls = False
        TextBox3.Text = "COM3"
        Timer2.Interval = 1000




    End Sub


    Private Sub waypointincrement()
        If autoNavState = True And waypoint1.lat <> 0 Then
            com1.WriteLine("WPT:1," + waypoint1.lat.ToString + waypoint1.lon.ToString + waypoint1.alt.ToString)
        ElseIf autoNavState = False Then
            com1.WriteLine("WPT:0")

        End If
    End Sub


    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
        com = TextBox3.Text
        comThread = New System.Threading.Thread(AddressOf ReceiveSerialData)
        comThread.IsBackground = True
        comThread.Start()
        Timer1.Interval = 1000
        Timer1.Enabled = True
    End Sub

    Private Sub GMapControl1_Load(sender As Object, e As EventArgs) Handles GMapControl1.Load
        GMapControl1.DragButton = Windows.Forms.MouseButtons.Left
        GMapControl1.CanDragMap = True


        GMapControl1.MapProvider = GMap.NET.MapProviders.GoogleHybridMapProvider.Instance
        GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerAndCache
        GMapControl1.ShowCenter = False
        GMapControl1.Position = New GMap.NET.PointLatLng(39.615719, -103.180253)




    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Dim calcSpeed As Integer
        calcSpeed = distanceTraveled * 3600

        TextBox20.Text = calcSpeed
        distanceTraveled = 0
        Call waypointincrement()
    End Sub
    Private Sub wptbox1_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles wptbox1.KeyDown
        If e.KeyCode = Keys.Enter Then
            wptbox2.Focus()
            findWaypoints()

        End If
    End Sub


    Private Sub Button18_Click(sender As Object, e As EventArgs) Handles Button18.Click

        Select Case autoNavState
            Case True
                Button18.BackColor = Color.Red

                autoNavState = False
                com1.WriteLine("WPT:0")
                Return



            Case False
                Button18.BackColor = Color.Green
                autoNavState = True

                Return

        End Select


    End Sub


    Private Sub Button17_Click(sender As Object, e As EventArgs) Handles Button17.Click
        takeoffAlt = currentAlt
    End Sub

    Private Sub wptbox2_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles wptbox2.KeyDown
        If e.KeyCode = Keys.Enter Then
            wptbox3.Focus()
            findWaypoints()

        End If
    End Sub

    Private Sub TextBox22_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles TextBox22.KeyDown
        If e.KeyCode = Keys.Enter Then
            Dim home() As String
            home = Split(TextBox22.Text, ",")
            homeoverlay.Clear()
            homeLat = home(0)
            homeLon = home(1)
            homeoverlay.Markers.Add(New GMap.NET.WindowsForms.Markers.GMarkerGoogle(New PointLatLng(home(0), home(1)), Markers.GMarkerGoogleType.green))
            GMapControl1.Overlays.Add(homeoverlay)

        End If
    End Sub



    Private Sub wptbox3_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles wptbox3.KeyDown
        If e.KeyCode = Keys.Enter Then
            wptbox4.Focus()
            findWaypoints()

        End If
    End Sub
    Private Sub wptbox4_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles wptbox4.KeyDown
        If e.KeyCode = Keys.Enter Then
            wptbox5.Focus()
            findWaypoints()

        End If
    End Sub

    Private Sub Timer2_Tick(sender As Object, e As EventArgs) Handles Timer2.Tick
        GroupBox8.BackColor = Color.Green

        If errorFlash = True Then
            GroupBox8.BackColor = Color.Green
            errorFlash = False
            Return
        End If
        If errorFlash = False Then
            GroupBox8.BackColor = Color.Red
            errorFlash = True
            Return
        End If

    End Sub

    Private Sub Button19_Click(sender As Object, e As EventArgs) Handles Button19.Click
        Label17.Visible = True
        Button20.Visible = True
        Button21.Visible = True

    End Sub

    Private Sub Button21_Click(sender As Object, e As EventArgs) Handles Button21.Click
        Label17.Visible = False
        Button20.Visible = False
        Button21.Visible = False
    End Sub

    Private Sub Button20_Click(sender As Object, e As EventArgs) Handles Button20.Click
        com1.WriteLine("CTL:1")
        Label17.Visible = False
        Button20.Visible = False
        Button21.Visible = False
    End Sub

    Private Sub wptbox5_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles wptbox5.KeyDown
        If e.KeyCode = Keys.Enter Then
            findWaypoints()

        End If
    End Sub
    Private Sub Rotation(sender As Object, e As PaintEventArgs) Handles PictureBox2.Paint
        Dim dgr As Double
        If dgr = 360 Then dgr = 0
        dgr += Val(heading)

        With e.Graphics
            .TranslateTransform(PictureBox2.Width \ 2, PictureBox2.Height \ 2)
            .RotateTransform(CSng(dgr))
            Dim img As New Bitmap(UAV_Mapper_and_control.My.Resources.plane)
            img = New Bitmap(img, PictureBox2.Width, PictureBox2.Height)
            .DrawImage(img, -img.Width \ 2, -img.Height \ 2)
        End With
    End Sub


    Private Sub Roll(sender As Object, e As PaintEventArgs) Handles PictureBox4.Paint
        Dim dgr As Double
        If dgr = 180 Then dgr = 0
        dgr += Val(rroll)
        dgr -= 90

        With e.Graphics
            .TranslateTransform(PictureBox4.Width \ 2, PictureBox4.Height \ 2)
            .RotateTransform(CSng(dgr))
            Dim img As New Bitmap(UAV_Mapper_and_control.My.Resources.planeRoll)
            img = New Bitmap(img, PictureBox4.Width, PictureBox4.Height)
            .DrawImage(img, -img.Width \ 2, -img.Height \ 2)
        End With
        PictureBox4.BackgroundImageLayout = ImageLayout.Zoom

    End Sub

    Private Sub Pitch(sender As Object, e As PaintEventArgs) Handles PictureBox5.Paint
        Dim dgr As Double
        If dgr = 180 Then dgr = 0
        dgr += Val(rpitch)
        dgr -= 90


        With e.Graphics
            .TranslateTransform(PictureBox5.Width \ 2, PictureBox5.Height \ 2)
            .RotateTransform(CSng(dgr))
            Dim img As New Bitmap(UAV_Mapper_and_control.My.Resources.planePitch)
            img = New Bitmap(img, PictureBox5.Width, PictureBox5.Height)
            .DrawImage(img, -img.Width \ 2, -img.Height \ 2)
        End With
        PictureBox5.BackgroundImageLayout = ImageLayout.Zoom

    End Sub


End Class
