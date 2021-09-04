using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace QRBarcode.DigitalKeys
{
    public class CertJson
    {
        public string KID { get; set; }
        public string SerialNumber { get; set; }

        public string Subject { get; set; }

        public string Issuer { get; set; }
        public string NotBefore { get; set; }
        public string NotAfter { get; set; }

        public string SignatureAlgorithm { get; set; }

        public string Fingerprint { get; set; }
        public string HashAlgorithm { get; set; }
        public string KeyAlgorithm { get; set; }
        public string Base64PubKey { get; set; }

        public AsymmetricAlgorithm PubKey {get;set;}
    }
}
