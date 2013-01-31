using System;
using Dimmi.Controllers;
using Dimmi.DataInterfaces;
using Dimmi.DataInterfaces.Fakes;
using Dimmi.Encryption;
using Dimmi.Encryption.Fakes;
using Dimmi.Models.Domain;
using Dimmi.Models.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using Moq;

namespace DimmiUnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestAuthentication()
        {

            //arrange
            string inOuathId = "100004880199806";
            string inEmail = "updziwb_greeneescuwitzskysonsensteinbergman_1355800025@tfbnw.net";
            Guid userId = Guid.Parse("a09241d2-9678-4c21-9e6e-77e57720fffa");
            string toEncrypt = inOuathId + "#" + inEmail;


            var pup = new StubPathProvider
            {
                GetPublicPath01 = () => @"C:\Users\CMD-LTL-DVL\Documents\Visual Studio 2012\Projects\Dimmi\Dimmi\Encryption\public"
            };
            var prp = new StubPathProvider
            {
                GetPrivatePath01 = () => @"C:\Users\CMD-LTL-DVL\Documents\Visual Studio 2012\Projects\Dimmi\Dimmi\Encryption"
            };

            string[] encryptedParts = Crypto.Encrypter(toEncrypt, pup);

            //string request = Crypto.Decrypt(new string[] { encryptedParts[0], encryptedParts[1] }, prp);
            //string[] parts = request.Split(new char[] { Char.Parse("#") });

            var user = new Mock<User>();
            user.SetupGet(u => u.id).Returns(userId);        //.UserData { id=Guid.Parse("a09241d2-9678-4c21-9e6e-77e57720fffa") };
            user.SetupGet(u => u.emailAddress).Returns(inEmail);

            var userRepository = new Mock<IUserRepository>();
            var controller = new Mock<UsersController>();
            controller.Setup(c => c.Get(encryptedParts[0], encryptedParts[1])).Returns(user.Object);



            var userReturned = controller.Object.Get(encryptedParts[0], encryptedParts[1]);
            Assert.AreEqual(userReturned.id, userId);
            Assert.AreEqual(userReturned.oauthId, inOuathId);
           
            //var repo = new StubIUserRepository
            //{
            //    GetStringString = (oathId, emailAddress) => user
            //};

            //var controller = new UsersController(repo);
            ////act
            //var result = controller.Get(parts[0], parts[1]);

            ////assert
            //Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreSame(result.emailAddress, inEmail);
            //Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreSame(result.oauthId, inOuathId);
            //Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreSame(result.id, userId);
            //Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result.sessionToken);
            //Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result.expires);

        }
    }
}
