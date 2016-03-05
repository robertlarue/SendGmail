#SendGmail
Windows command line tool for sending Gmail messages using the [Gmail API Client Library for .NET](https://developers.google.com/api-client-library/dotnet/apis/gmail/v1) targeting .NET 4.0 for compatibility with Windows XP.

An important distinction of this tool compared to other command line tools like [blat](http://www.blat.net/) is that it communicates over https rather than [SMTP](https://en.wikipedia.org/wiki/Simple_Mail_Transfer_Protocol) so that it does not typically require changes to antivirus settings (or get flagged as a mass mailing worm).

##Download
You can download the latest release from the Github Releases page

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


