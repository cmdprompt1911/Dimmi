using System;
using System.Net;
using System.Net.Http;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dimmi.Controllers;
using Dimmi.Encryption;
using Dimmi.Models.UI;

namespace DimmiUnitTest
{
    [TestClass]
    public class UserControllerTests
    {
        [TestMethod]
        public void TestAuthentication()
        {
            //String authId = "100004880199806";
            //String email = "updziwb_greeneescuwitzskysonsensteinbergman_1355800025@tfbnw.net";
            //String toEncrypt = authId + "#" + email;

            //string[] encryptedStringAndMat = Crypto.Encrypter(toEncrypt);

            //Dimmi.Controllers.UsersController uc = new Dimmi.Controllers.UsersController();
            //User user = uc.Get(encryptedStringAndMat[1], encryptedStringAndMat[0]);
            //Assert.IsNotNull(user);
            //Assert.IsNotNull(user.sessionToken);
            //Assert.AreEqual(user.emailAddress, email);
            //Assert.AreEqual(user.oauthId, authId);
            //DateTime svrDate = new DateTime(user.lastLogin.Year, user.lastLogin.Month, user.lastLogin.Day, user.lastLogin.Hour, user.lastLogin.Minute, user.lastLogin.Second);
            //DateTime currentDate = DateTime.UtcNow;
            //currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, currentDate.Hour, currentDate.Minute, currentDate.Second);
            //long diff = currentDate.Ticks - svrDate.Ticks;

            //Assert.IsTrue(diff <= 100);
        }
    }
}
