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
        public static string Decrypt(string[] data, PathProvider p)
        {
            
            WebBase64 sessionMaterial = (WebBase64)data[0];
            WebBase64 cipherText = (WebBase64)data[1];
            string output;

            //PathProvider pathProvider = new PathProvider();
            string path1 = p.GetPrivatePath();

            //string path1 = HostingEnvironment.ApplicationPhysicalPath + "encryption";

            using (var crypter = new Crypter(path1))
            using (var sessionCrypter = new SessionCrypter(crypter, sessionMaterial))
            {
                output = sessionCrypter.Decrypt(cipherText);

            }
            return output;
        }

        public static string[] Encrypter(string textToEncrypt, PathProvider p)
        {
            WebBase64 sessionMaterial;
            WebBase64 cipherText;
            string[] data;
            string path = p.GetPublicPath();


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

    public class PathProvider
    {
        public virtual string GetPublicPath()
        {
            return HostingEnvironment.ApplicationPhysicalPath + "encryption\\public";
        }

        public virtual string GetPrivatePath()
        {
            return HostingEnvironment.ApplicationPhysicalPath + "encryption";
        }
    }
}