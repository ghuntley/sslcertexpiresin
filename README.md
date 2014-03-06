# Overview
A command line client that can be used in devops/monitoring style situation
which checks the validity of a SSL certificate and returns the amount of
days until the certificate has expired by setting the environment return code.

* If a certificate will expire in 300 days then the RC=$? or %ERRORLEVEL% will be set to 300.
* If a certificate expired 100 days ago then the RC=$? or %ERRORLEVEL% will be set to -100.
* If a certificate will expire today then the RC=$? or %ERRORLEVEL% will be set to 0.
* If a error occurs whilst retrieving the certificate then the RC=$? or %ERRORLEVEL% will be set to -1.


# Usage

    > SSLCertExpiresIn -h
    
    Copyright (C) 2014 Geoffrey Huntley
    1.0.0


    Usage: SSLCertExpiresIn -s www.freebsg.org [-p 443]
    Usage: SSLCertExpiresIn -s 127.0.0.1 -p 8443

      -s, --server    Required. WebServer Hostname or IP Address.
      -p, --port      WebServer Port
      --help          Display this help screen.

# Output when certificate is Valid

    > SSLCertExpiresIn -s freebsd.org
    2014-03-06 11:12:29.3665|TRACE|SSLCertExpiresIn.Program|Results of parsing command line arguments: Server=freebsd.org, Port=
    2014-03-06 11:12:29.3832|INFO|SSLCertExpiresIn.Program|Retrieving SSL Certificate details for: https://freebsd.org:443
    2014-03-06 11:12:31.8688|INFO|SSLCertExpiresIn.Program|Successfully retrieved SSL certificate.
    2014-03-06 11:12:31.8834|DEBUG|SSLCertExpiresIn.Program|Certificate will expire at: 10/01/2015 10:59:59 AM
    2014-03-06 11:12:31.8834|DEBUG|SSLCertExpiresIn.Program|Certificate has 309 days remaining until the certificate expires.

    > echo %ERRORLEVEL%
    309

![success](https://raw.github.com/ghuntley/sslcertexpiresin/master/assets/sslexpiresin-success.png)


# Output when certificate is Invalid

    > SSLCertExpiresIn -s  127.0.0.1 -p 8443
    2014-03-06 11:15:24.2679|TRACE|SSLCertExpiresIn.Program|Results of parsing command line arguments: Server=127.0.0.1, Port=8443
    2014-03-06 11:15:24.2838|INFO|SSLCertExpiresIn.Program|Retrieving SSL Certificate details for: https://127.0.0.1:8443
    2014-03-06 11:15:25.3325|INFO|SSLCertExpiresIn.Program|Failed to retrieve SSL certificate, will try again shortly.
    2014-03-06 11:15:25.3325|ERROR|SSLCertExpiresIn.Program|System.Net.WebException: Unable to connect to the remote server ---> System.Net.Sockets.SocketException: No connection could be made because the target machine actively refused it 127.0.0.1:8443 at System.Net.Sockets.Socket.DoConnect(EndPoint endPointSnapshot, SocketAddress socketAddress) at System.Net.ServicePoint.ConnectSocketInternal(Boolean connectFailure, Socket s4, Socket s6, Socket& socket, IPAddress& address, ConnectSocketState state, IAsyncResult asyncR sult, Exception& exception)
       --- End of inner exception stack trace ---
       at System.Net.HttpWebRequest.GetResponse()
       at SSLCertExpiresIn.Program.<>c__DisplayClass2.<GetX509Certificate>b__0() in d:\Dropbox\Projects\sslcertexpiresin\src\SSLCertExpiresIn\Program.cs:line 91

    2014-03-06 11:15:28.5514|FATAL|SSLCertExpiresIn.Program|System.Exception: Non-recoverable failure occurred whilst retrieving SSL certificate.
       at SSLCertExpiresIn.Program.GetX509Certificate(String url) in d:\Dropbox\Projects\sslcertexpiresin\src\SSLCertExpiresIn\Program.cs:line 110
       at SSLCertExpiresIn.Program.Main(String[] args) in d:\Dropbox\Projects\sslcertexpiresin\src\SSLCertExpiresIn\Program.cs:line 133
       
    > echo %ERRORLEVEL%
    -1

![failure](https://raw.github.com/ghuntley/sslcertexpiresin/master/assets/sslexpiresin-failure.png)

