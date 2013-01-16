using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using Keyczar;

namespace Dimmi.Encryption
{
    public static class Crypto
    {
        public static string Decrypt(string[] data)
        {
            
            WebBase64 sessionMaterial = (WebBase64)data[0];
            WebBase64 cipherText = (WebBase64)data[1];
            string output;

            string path1 = HostingEnvironment.ApplicationPhysicalPath + "encryption";

            using (var crypter = new Crypter(path1))
            using (var sessionCrypter = new SessionCrypter(crypter, sessionMaterial))
            {
                output = sessionCrypter.Decrypt(cipherText);

            }
            return output;
        }

        public static string[] Encrypter(string textToEncrypt)
        {
            WebBase64 sessionMaterial;
            WebBase64 cipherText;
            string[] data;
            string path = HostingEnvironment.ApplicationPhysicalPath + "encryption\\public";

            using (var encrypter = new Encrypter(path))
            using (var sessionCrypter = new SessionCrypter(encrypter))
            {
                sessionMaterial = sessionCrypter.SessionMaterial;
                cipherText = sessionCrypter.Encrypt(textToEncrypt);
                data = new string[] { sessionMaterial.ToString(), cipherText.ToString() };
                //data.Add("sessionmaterial", sessionMaterial);
                //data.Add("data", cipherText);
                
            }
            return data;
        }
    }
}