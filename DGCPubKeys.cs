using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QRBarcode.DigitalKeys;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace QRBarcode
{
    public class DGCPubKeys
    {

        public List<CertJson> LstCerts { get; set; }

        public async Task GetCertificates()
        {
            try
            {
                WebClient wc = new WebClient();
                string json = wc.DownloadString("https://raw.githubusercontent.com/lovasoa/sanipasse/master/src/assets/Digital_Green_Certificate_Signing_Keys.json");
                LstCerts = new List<CertJson>();
                if (!string.IsNullOrEmpty(json))
                {
                    JObject jo = JObject.Parse(json);
                    JsonReader jr = jo.CreateReader();
                    CertJson cj = null;
                    while (jr.Read())
                    {
                        if (jr.Value != null)
                        {
                            
                            if(jr.TokenType == JsonToken.PropertyName && jr.Value.ToString().EndsWith("="))
                            {
                                cj = new CertJson (){ KID = jr.Value.ToString() };
                            }
                            else
                            {
                                if (cj!=null && !string.IsNullOrEmpty(cj.KID))
                                {
                                    if (jr.Path == cj.KID + ".serialNumber")
                                    {
                                        cj.SerialNumber = jr.Value.ToString();
                                    }
                                    else if (jr.Path == cj.KID + ".subject")
                                    {
                                        cj.Subject = jr.Value.ToString();
                                    }
                                    else if (jr.Path == cj.KID + ".issuer")
                                    {
                                        cj.Issuer = jr.Value.ToString();
                                    }
                                    else if (jr.Path == cj.KID + ".notBefore")
                                    {
                                        cj.NotBefore = jr.Value.ToString();
                                    }
                                    else if (jr.Path == cj.KID + ".notAfter")
                                    {
                                        cj.NotAfter = jr.Value.ToString();
                                    }
                                    else if (jr.Path == cj.KID + ".signatureAlgorithm")
                                    {
                                        cj.SignatureAlgorithm = jr.Value.ToString();
                                    }
                                    else if (jr.Path == cj.KID + ".fingerprint")
                                    {
                                        cj.Fingerprint = jr.Value.ToString();
                                    }
                                    else if (jr.Path == cj.KID + ".publicKeyAlgorithm.hash.name")
                                    {
                                        cj.HashAlgorithm = jr.Value.ToString();
                                    }
                                    else if (jr.Path == cj.KID + ".publicKeyAlgorithm.name")
                                    {
                                        cj.KeyAlgorithm = jr.Value.ToString();
                                    }
                                    else if (jr.Path == cj.KID + ".publicKeyPem" && jr.Value.ToString()!="publicKeyPem")
                                    {
                                        cj.Base64PubKey = jr.Value.ToString();

                                        byte[] cert = Convert.FromBase64String(cj.Base64PubKey);
                                       
                                        
                                        if(cj.KeyAlgorithm=="ECDSA")
                                        {
                                            //wszystkie są p-256
                                            CngKey klucz = ImportECDsa256PublicKey(cj.Base64PubKey);
                                           
                                            ECDsaCng eCDsaCng = new ECDsaCng(klucz);

                                            cj.PubKey = eCDsaCng;
                                        }
                                        else
                                        {
                                            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();

                                            //Create a new instance of RSAParameters.
                                            RSAParameters RSAKeyInfo = new RSAParameters();

                                            //Set RSAKeyInfo to the public key values. 
                                            RSAKeyInfo.Modulus = cert;
                                            RSAKeyInfo.Exponent = new byte[]{ 1,0,1};
                                            RSA.ImportParameters(RSAKeyInfo);

                                            cj.PubKey = RSA;
                                        }


                                        LstCerts.Add(cj);
                                        cj = null;
                                    }
                                }
                            }
                        }
                    }
                                       

                }
                else
                {
                    throw new Exception(Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("ErrorEmptyCertificates"));
                }
            }
            catch(Exception ex)
            {
                MessageDialog md = new MessageDialog(Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("Error") + ex.Message);
                await md.ShowAsync();
            }
            finally
            {
                await Task.CompletedTask;
            }
            
        }

        //https://stackoverflow.com/questions/44502331/c-sharp-get-cngkey-object-from-public-key-in-text-file

        private static readonly byte[] s_secp256r1Prefix =
    Convert.FromBase64String("MFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAE");

        // For ECDH instead of ECDSA, change 0x53 to 0x4B.
        private static readonly byte[] s_cngBlobPrefix = { 0x45, 0x43, 0x53, 0x31, 0x20, 0, 0, 0 };

        private static CngKey ImportECDsa256PublicKey(string base64)
        {
            byte[] subjectPublicKeyInfo = Convert.FromBase64String(base64);

            if (subjectPublicKeyInfo.Length != 91)
                throw new InvalidOperationException();

            byte[] prefix = s_secp256r1Prefix;

            if (!subjectPublicKeyInfo.Take(prefix.Length).SequenceEqual(prefix))
                throw new InvalidOperationException();

            byte[] cngBlob = new byte[s_cngBlobPrefix.Length + 64];
            Buffer.BlockCopy(s_cngBlobPrefix, 0, cngBlob, 0, s_cngBlobPrefix.Length);

            Buffer.BlockCopy(
                subjectPublicKeyInfo,
                s_secp256r1Prefix.Length,
                cngBlob,
                s_cngBlobPrefix.Length,
                64);

            return CngKey.Import(cngBlob, CngKeyBlobFormat.EccPublicBlob);
        }

    }
}
