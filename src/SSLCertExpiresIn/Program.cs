using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using CommandLine;
using NLog;
using SSLCertExpiresIn.Helpers;

namespace SSLCertExpiresIn
{
    public class Program
    {
        public const int DefaultWebserverPort = 443;
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public static string GetCertificateDistinguishedName(X509Certificate certificate)
        {
            Contract.Requires(certificate != null);

            string distinguishedname = certificate.Subject;

            Log.Debug("Certificate distinguished name is: {0}", distinguishedname);
            return distinguishedname;
        }

        public static DateTimeOffset GetCertificateExpirationDate(X509Certificate certificate)
        {
            Contract.Requires(certificate != null);

            DateTime expirationdate = DateTime.Parse(certificate.GetExpirationDateString());

            Log.Debug("Certificate will expire at: {0}", expirationdate);

            return expirationdate;
        }

        public static string GetCertificateIssuerName(X509Certificate certificate)
        {
            Contract.Requires(certificate != null);

            string issuer = certificate.Issuer;
            Log.Debug("Certificate was issued by: {0}", issuer);

            return certificate.Issuer;
        }

        public static string GetCertificatePublicKey(X509Certificate certificate)
        {
            Contract.Requires(certificate != null);

            string publickey = certificate.GetPublicKeyString();

            Log.Debug("Certificate public key is: {0}", publickey);
            return publickey;
        }

        public static int GetDaysRemaining(DateTimeOffset expirydate)
        {
            DateTime currentdatetime = DateTime.Now;

            TimeSpan expiresin = expirydate - currentdatetime;

            Log.Debug("Certificate has {0} days remaining until the certificate expires.", expiresin.Days);

            return expiresin.Days;
        }

        public static X509Certificate GetX509Certificate(string url)
        {
            Contract.Requires(!String.IsNullOrWhiteSpace(url));

            try
            {
                var uri = new Uri(url);
            }
            catch (Exception ex)
            {
                throw new UriFormatException(ex.Message);
            }

            try
            {
                X509Certificate certificate = Retry.Do(() =>
                {
                    Log.Info("Retrieving SSL Certificate details for: {0}", url);
                    try
                    {
                        var request = (HttpWebRequest) WebRequest.Create(url);
                        request.Timeout = 15*1000;
                        var response = (HttpWebResponse) request.GetResponse();
                        response.Close();

                        Log.Info("Successfully retrieved SSL certificate.");

                        return request.ServicePoint.Certificate;
                    }
                    catch (Exception ex)
                    {
                        Log.Info("Failed to retrieve SSL certificate, will try again shortly.");
                        Log.Error(ex);
                        throw ex;
                    }
                }, TimeSpan.FromSeconds(0), 4);

                return certificate;
            }
            catch
            {
                throw new Exception(
                    "Non-recoverable failure occurred whilst retrieving SSL certificate.");
            }
        }

        private static void Main(string[] args)
        {
            var options = new CommandLineOptions();

            // allow app to be debugged in visual studio.
            if (Debugger.IsAttached)
            {
                args = "-s www.freebsd.org -p 443 ".Split(' ');
            }

            if (Parser.Default.ParseArgumentsStrict(args, options))
            {
                try
                {
                    Log.Trace("Results of parsing command line arguments: Server={0}, Port={1}", options.Server,
                        options.Port);

                    string url = String.Format("https://{0}:{1}", options.Server, options.Port ?? DefaultWebserverPort);
                    X509Certificate certificate = GetX509Certificate(url);

                    DateTimeOffset expirationdate = GetCertificateExpirationDate(certificate);

                    int daysremaining = GetDaysRemaining(expirationdate);

                    Environment.Exit(daysremaining);
                }
                catch (Exception ex)
                {
                    Log.Fatal(ex);

                    Environment.Exit(-1);
                }
            }
        }
    }
}