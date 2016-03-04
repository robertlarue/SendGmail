Imports System.IO
Imports System.Net.Mail
Imports Google.Apis.Gmail.v1
Imports Google.Apis.Gmail.v1.Data
Imports Google.Apis.Auth.OAuth2
Imports Google.Apis.Util.Store
Imports System.Text
Imports System.Threading
Imports CommandLine
Imports Newtonsoft.Json
Class Options
    <[Option]("s"c, "subject", Required:=False, DefaultValue:="", HelpText:="Subject of message")>
    Public Property Subject As String

    <[Option]("f"c, "from", Required:=True, HelpText:="Sender address")>
    Public Property Sender As String

    <[OptionList]("t"c, "to", Required:=True, Separator:=";", HelpText:="Recipient addresses, semicolon separated")>
    Public Property Recipients As IList(Of String)

    <[Option]("m"c, "message", Required:=False, DefaultValue:="", HelpText:="Mail message")>
    Public Property Message As String

    <[Option]("b"c, "bodyfile", Required:=False, HelpText:="File to use as message body")>
    Public Property BodyFile As String

    <HelpOption>
    Public Function GetUsage() As String
        ' this without using CommandLine.Text
        '  or using HelpText.AutoBuild
        Dim usage = New StringBuilder()
        usage.AppendLine("SendGmail 1.0")
        usage.AppendLine("Read user manual for usage instructions...")
        Return usage.ToString()
    End Function
End Class
Module SendGmail
    Dim Scopes As String() = {GmailService.Scope.GmailSend}
    Sub Main(args As String())
        Dim o As New Options()
        If CommandLine.Parser.Default.ParseArguments(args, o) Then
            Dim credential As UserCredential
            If File.Exists("client_id.json") Then
                Using stream = New FileStream("client_id.json", FileMode.Open, FileAccess.Read)
                    Dim credPath As String = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal)
                    credPath = Path.Combine(credPath, ".credentials/gmail-creds.json")
                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.Load(stream).Secrets,
                Scopes,
                "user",
                CancellationToken.None,
                New Google.Apis.Util.Store.FileDataStore(credPath, True)).Result
                End Using
                Dim BodyText As String = ""
                If Not String.IsNullOrEmpty(o.BodyFile) Then
                    If File.Exists(o.BodyFile) Then
                        BodyText = File.ReadAllText(o.BodyFile)
                    Else
                        Console.WriteLine("ERROR: Cannot read message body file")
                    End If
                Else
                    BodyText = o.Message
                End If
                Dim msg = New System.Net.Mail.MailMessage() _
                With {
                .Subject = o.Subject,
                .Body = BodyText,
                .From = New MailAddress(o.Sender)
                }
                For Each recipient In o.Recipients
                    msg.To.Add(New MailAddress(recipient))
                Next
                Dim ApplicationName As String = ""
                Dim jsonreader = New JsonTextReader(New StreamReader("client_id.json"))
                While jsonreader.Read()
                    If jsonreader.TokenType = JsonToken.PropertyName And jsonreader.Value = "project_id" Then
                        jsonreader.Read()
                        ApplicationName = jsonreader.Value
                        Exit While
                    End If
                End While
                Dim gmail As New GmailService(New Google.Apis.Services.BaseClientService.Initializer() _
                With {
                .HttpClientInitializer = credential,
                .ApplicationName = ApplicationName
                })
                Dim AlarmTriggered As Boolean = False

                Try
                    Dim msgStr = New Stream
                    Dim client As SmtpClient = New SmtpClient("localhost")
                    client.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory
                    client.PickupDirectoryLocation = "."
                    client.Send(msg)
                    Dim filepath = Directory.GetFiles(".", "*.eml", SearchOption.TopDirectoryOnly).Single()
                    Using fs As New FileStream(filepath, FileMode.Open)
                        fs.CopyTo(msgstr)
                    End Using
                    Dim result = gmail.Users.Messages.Send(New Message With {.Raw = Base64UrlEncode(msgStr.ToString())}, "me").Execute()
                Catch e As Exception
                    Console.WriteLine(e.Message & vbNewLine & e.StackTrace & vbNewLine & e.Data.ToString)
                    End Try
                Else
                    Console.Error.WriteLine("ERROR: client_id.json file missing")
            End If
        Else
            Console.WriteLine(o.GetUsage())
        End If
    End Sub
    Public Function Base64UrlEncode(arg As String) As String
        Dim inputBytes = System.Text.Encoding.UTF8.GetBytes(arg)
        Dim s As String = Convert.ToBase64String(inputBytes).Replace("+", "-").Replace("/", "_").Replace("=", "")
        Return s
    End Function
End Module
