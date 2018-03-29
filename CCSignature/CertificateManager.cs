using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace CCSignature
{
    public class CertificateManager
    {
        private const string SIGANTURE_ALGORITHM        = "SHA1";
        private const string CERTIFICATE_OID_FILTER     = "1.3.6.1.4.1.25070.1.1.1.1.0.1.2";
        private const string CERTIFICATE_OID_FILTER2    = "2.16.620.1.1.1.2.10";

#region private methods

		private X509Certificate2 GetCertificate(string certThumbPrint) 
        {
            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            X509Certificate2Collection certCollection;
            try
            {
                store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

                // Find the certificate that matches the thumbprint.
                certCollection = store.Certificates.Find(X509FindType.FindByThumbprint, certThumbPrint, false);
            } 
            finally
            {
                store.Close();
            }

            // Check to see if our certificate was added to the collection. If no, throw an error, if yes, create a certificate using it
            if (certCollection.Count != 1)
            {
                throw new ArgumentException("No certificate found containing thumbprint: " + certThumbPrint);
            }
            return certCollection[0];
        }

#endregion


#region public methods

		public string Sign(string certThumbPrint, string stringToSign)
        {
            var certificateWithPKey   = GetCertificate(certThumbPrint);
            var rsaFormatter          = new RSAPKCS1SignatureFormatter(certificateWithPKey.PrivateKey);
            rsaFormatter.SetHashAlgorithm(SIGANTURE_ALGORITHM);

            var hash            = new SHA1Managed().ComputeHash(Encoding.UTF8.GetBytes(stringToSign));
            var signedHashValue = rsaFormatter.CreateSignature(hash);
            return Convert.ToBase64String(signedHashValue);
        }

        public List<Certificate> GetStoreCertificates()
        {
            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

            var validCertscollection        = store.Certificates.Find(X509FindType.FindByTimeValid, DateTime.Now, true);
            var signatureCertscollection    = validCertscollection.Find(X509FindType.FindByCertificatePolicy, CERTIFICATE_OID_FILTER, true);
            signatureCertscollection.AddRange(validCertscollection.Find(X509FindType.FindByCertificatePolicy, CERTIFICATE_OID_FILTER2, true));

            var storeCertificates = new List<Certificate>();
            foreach (var certificate in signatureCertscollection)
            {
                if (!certificate.HasPrivateKey) continue;
                
                var friendlyName = !string.IsNullOrEmpty(certificate.FriendlyName) 
					? certificate.FriendlyName 
					: string.Empty;

                storeCertificates.Add(new Certificate(certificate.Thumbprint, certificate.GetNameInfo(X509NameType.SimpleName,false) + friendlyName));
            }
            store.Close();

            return storeCertificates;
        }

		public X509Certificate2 GetIssuer(X509Certificate2 leafCert)
        {
            if (leafCert.Subject == leafCert.Issuer)
            {
                return leafCert;
            }

            var chain = new X509Chain
            {
                ChainPolicy = {RevocationMode = X509RevocationMode.NoCheck}
            };

            chain.Build(leafCert);
            X509Certificate2 issuer = null;
            if (chain.ChainElements.Count > 1)
            {
                issuer = chain.ChainElements[1].Certificate;
            }
            chain.Reset();
            return issuer;
        }
#endregion
		       
	}
}
