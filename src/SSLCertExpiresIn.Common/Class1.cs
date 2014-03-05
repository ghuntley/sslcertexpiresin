using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSLCertExpiresIn.Common
{
    public class Class1
    {
        public DateTime GetExpirationDateTime(X509Certificate certificate)
        {
            X509Certificate2 cert2 = new X509Certificate2(certificate);

            string cn = cert2.GetIssuerName();
            string cedate = cert2.GetExpirationDateString();

            string cpub = cert2.GetPublicKeyString();

            //display the cert dialog box
            X509Certificate2UI.DisplayCertificate(cert2);
        }
    }
}
