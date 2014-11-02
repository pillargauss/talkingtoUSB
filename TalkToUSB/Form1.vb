Imports HIDLibrary
Imports System.Management
Imports System.Threading

Public Class Form1
    Inherits System.Windows.Forms.Form
    Private Declare Function FT_Open Lib "FTD2XX.dll" (ByVal intDeviceNumber As Short, ByRef lnghandle As Integer) As Integer
    Const FT_Ok As Short = 0
    Dim buf(100) As Byte
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim sendBytes, readBytes As Byte()
        Dim BytesWritten As Integer

        sendBytes = System.Text.Encoding.Default.GetBytes(TextBox2.Text)
 
        ' Purge buffers
        FT_Status = FT_Purge(FT_Handle, FT_PURGE_RX Or FT_PURGE_TX)
        If FT_Status <> FT_Ok Then
            Exit Sub
        End If


        ' Set Baud Rate
        FT_Status = FT_SetBaudRate(FT_Handle, 115200)
        If FT_Status <> FT_Ok Then
            Exit Sub
        End If


        ' Set parameters
        FT_Status = FT_SetDataCharacteristics(FT_Handle, FT_DATA_BITS_8, FT_STOP_BITS_1, FT_PARITY_NONE)
        If FT_Status <> FT_Ok Then
            Exit Sub
        End If


        ' Set Flow Control
        FT_Status = FT_SetFlowControl(FT_Handle, FT_FLOW_RTS_CTS, 0, 0)
        If FT_Status <> FT_Ok Then
            Exit Sub
        End If


        ' Set RTS
        FT_Status = FT_SetRts(FT_Handle)
        If FT_Status <> FT_Ok Then
            Exit Sub
        End If


        ' Set DTR
        FT_Status = FT_SetDtr(FT_Handle)
        If FT_Status <> FT_Ok Then
            Exit Sub
        End If


        ' Write string data to device
        FT_Status = FT_Write_String(FT_Handle, TextBox2.Text, Len(TextBox2.Text), BytesWritten)
        If FT_Status <> FT_Ok Then
            Exit Sub
        End If


        ' Wait
        Sleep(100)

        ' Get number of bytes waiting to be read
        FT_Status = FT_GetQueueStatus(FT_Handle, FT_RxQ_Bytes)
        If FT_Status <> FT_Ok Then
            Exit Sub
        End If


        ' Read number of bytes waiting
        ' Allocate string to recieve data
        Dim TempStringData As String
        Dim BytesRead As Integer

        TempStringData = Space(FT_RxQ_Bytes + 1)
        FT_Status = FT_Read_String(FT_Handle, TempStringData, FT_RxQ_Bytes, BytesRead)
        If FT_Status <> FT_Ok Then
            Exit Sub
        End If
        ' Display string on form
        TextBox1.Text = Trim(TempStringData)

    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        'myUSB.Open("Vid_0403&Pid_6001")
        'Dim USBClass As New System.Management.ManagementClass("Win32_USBHub")
        'Dim USBCollection As System.Management.ManagementObjectCollection = USBClass.GetInstances()
        'Dim USB As System.Management.ManagementObject
        'Dim temp As String = "USB\VID_0403&PID_6001\A101EOSM"

        'For Each USB In USBCollection
        '    Me.ListBox2.Items.Add("Description = " & USB("Name").ToString())
        '    Me.ListBox2.Items.Add("Device ID = " & USB("deviceid").ToString())
        '    Me.ListBox2.Items.Add("PNP Device ID = " & USB("PNPDeviceID").ToString())
        '    'temp = USB.Properties("VID").Value.ToString()
        '    If StrComp(temp, USB("deviceid").ToString()) = 0 Then
        '        TextBox1.AppendText(USB("deviceid").ToString() & Environment.NewLine)
        '    End If

        'Next USB

        'Dim microChipUSB As HIDLibrary.HidDevice()
        'Dim myUSB As HidDevice
        'microChipUSB = HidDevices.Enumerate(USB("deviceid"))
        'myUSB = microChipUSB(0)
        Dim lnghandle As Object
        Dim ftdiDeviceCount As Int16
        Dim TempManufacturer As String
        Dim TempManufacturerID As String
        Dim TempDescription As String
        Dim TempSerialNumber As String

        Dim DeviceIndex As Integer
        Dim TempDevString As String
        Dim lngFT_Type, ID As Integer
        Dim VidPid As String


        ' Create empty strings
        TempManufacturer = Space(32)
        TempManufacturerID = Space(16)
        TempDescription = Space(64)
        TempSerialNumber = Space(16)

        'Get the number of device connected. 
        'FT_ListDevices - Get information concerning the devices currently connected.  This function can return such information as the 
        'number of devices connected, and device strings such as serial number and product description.
        FT_Status = FT_GetNumberOfDevices(ftdiDeviceCount, vbNullChar, FT_LIST_NUMBER_ONLY)
        If FT_Status <> FT_Ok Then
            Exit Sub
        End If

        'Display the device number found
        Label2.Text = ftdiDeviceCount.ToString + "FTDI device found"

        TempDevString = Space(16)
        FT_Status = FT_GetDeviceString(DeviceIndex, TempDevString, FT_LIST_BY_INDEX Or FT_OPEN_BY_SERIAL_NUMBER)
        If FT_Status <> FT_Ok Then
            Exit Sub
        End If

        FT_Status = FT_OpenByIndex(DeviceIndex, FT_Handle)

        If FT_Status <> FT_Ok Then
            Exit Sub
        End If

        FT_Status = FT_GetDeviceInfo(FT_Handle, lngFT_Type, ID, TempSerialNumber, TempDescription, 0)
        If FT_Status <> FT_Ok Then
            Exit Sub
        End If

        VidPid = Hex(ID)
        If Len(VidPid) < 8 Then
            VidPid = "0" + VidPid
        End If

        Label3.Text = "Vendor ID:" + Microsoft.VisualBasic.Left(VidPid, 4)
        Label4.Text = "Product ID:" + Microsoft.VisualBasic.Right(VidPid, 4)

       
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        If Not bluetooth.IsOpen Then
            bluetooth.Open()
            AddHandler bluetooth.DataReceived, AddressOf bluetooth_DataReceived
        End If
    End Sub

    Private Sub bluetooth_DataReceived(ByVal sender As Object, ByVal e As System.IO.Ports.SerialDataReceivedEventArgs) Handles bluetooth.DataReceived
        If bluetooth.IsOpen Then
            Try
                ThreadProcSafe(0)
            Catch ex As Exception

            End Try
        End If
    End Sub
    Private Sub ThreadProcSafe(ByVal x As String)
        Me.SetText("Text set safely " & x)
    End Sub
    Delegate Sub SetTextCallback(ByVal [text] As String)
    Private Sub SetText(ByVal [text] As String)
        'TextBox3.AppendText(bluetooth.ReadLine())

        Dim bufSize As Integer = bluetooth.BytesToRead
        If bufSize > 0 Then
            bluetooth.Read(buf, 0, bufSize)
        End If
        TextBox3.AppendText(buf.ToString)
    End Sub

    'Private Sub usb_DataReceived(ByVal sender As Object, ByVal e As System.IO.Ports.SerialDataReceivedEventArgs) Handles usb.DataReceived
    '    Try
    '        ThreadProcSafe_usb(0)
    '    Catch ex As Exception

    '    End Try
    'End Sub
    'Private Sub ThreadProcSafe_usb(ByVal x As String)
    '    Me.settext1("Text set safely" & x)
    'End Sub
    'Private Sub settext1(ByVal [text] As String)
    '    TextBox3.AppendText(usb.ReadLine() & "from usb" & Environment.NewLine)
    'End Sub
End Class
