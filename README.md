#SendGmail
Windows command line tool for sending Gmail messages using the [Gmail API Client Library for .NET](https://developers.google.com/api-client-library/dotnet/apis/gmail/v1) targeting .NET 4.0 for compatibility with Windows XP.

An important distinction of this tool compared to other command line tools like [blat](http://www.blat.net/) is that it communicates over https rather than [SMTP](https://en.wikipedia.org/wiki/Simple_Mail_Transfer_Protocol) so that it does not typically require changes to antivirus settings (or get flagged as a mass mailing worm).

##Download
You can download the latest release from the [Github Releases](https://github.com/robertlarue/SendGmail/releases) page

##Google Setup
1. Sign into the google account you with to send mail from.
2. Open the [Google Developers Console](https://console.developers.google.com).
3. Create a new project
4. Open the [API Manager](https://console.developers.google.com/apis) and enable the Gmail API.
5. Open the [Credentials](https://console.developers.google.com/apis/credentials) page and create a new OAuth2 client ID for your app.
6. Click the Download JSON icon to the right of the Client ID.
7. Rename the downloaded file to client_id.json and move it to the same directory as SendGmail.exe
8. The first time you run SendGmail, it will launch your default browser and have you sign in and grant Gmail access to the app.
9. The OAuth2 credentials are saved in your User folder (C:\Users\username\.credentials\ on Windows 7 and later, and C:\Documents and Settings\username\My Documents\.credentials\ on Windows XP)

##Usage

    SendGmail -t <recipent> -f <sender> [-s <subject>] [-m <message> | -b <text file>]
    
    -t --to    recipient address (required)
    -f --from  sender address (required but defaults to address used with OAuth)
    -s --subject   subject of message (optional)
    -m --message   single line message (optional, mutually exclusive with bodyfile)
    -b --bodyfile  text file to used as message body (optional)

##Example
###Single line message
    SendGmail -t eric@gmail.com -f tim@apple.com -s "Self Driving Cars" -m "Nice!"
###Text file as body of message
    SendGmail -t frank@dji.com -f nick@gopro.com -s "Camera Drone" -b proposal.txt


