using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Dimmi.Data;
using Dimmi.Models.UI;
using Dimmi.Models.Domain;
using Dimmi.DataInterfaces;
using System.Collections;
using Dimmi.Encryption;
using Keyczar;


namespace Dimmi.Controllers
{
    public class UsersController : ApiController
    {
        static readonly IUserRepository repository = new UserRepository();
        static readonly IReviewStatisticsRepository stat_repository = new ReviewStatisticsRepository();

        public User Get(string sessionRequest, string sessionMaterial)
        {
            //decrypt the session request
            //request should be in the form of uid:emailaddress -- the UID is from facebook.  If the user exists, the UID will be stored on their account.  can compare the UID and Email to get a valid user
            // then generate a access token based upon the user's UID, email and the LAST timestamp for when the user logged on (update from this method.
            // they will be requred to pass this access token with every request.  We can then compare it to the three values stored in the db (UID, email, lastaccessed) to determine if this is a valid user.
            string request = Crypto.Decrypt(new string[] { sessionMaterial, sessionRequest });
            string[] parts = request.Split(new char[] { Char.Parse("#") });

            UserData checkUser = repository.Get(parts[0], parts[1]);
            if (checkUser == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            //we have a valid user. 
            //update the lastLogin to NOW
            DateTime dt = DateTime.UtcNow;
            //now encrypt the three pieces (UID:email:lastLogin)
            string toEncrypt = checkUser.oauthId + "#" + checkUser.emailAddress + "#" + checkUser.id + "#" + dt.ToString();
            string[] output = Crypto.Encrypter(toEncrypt);
            //now store the sessionMaterial and the lastlogin
            checkUser.lastLogin = dt;
            checkUser.sessionMaterial = output[0];
            checkUser.sessionToken = output[1];
            checkUser = repository.Update(checkUser);

            //now map to UI model...
            User retx = AutoMapper.Mapper.Map<UserData, User>(checkUser);
            //populate the sessionToken and return;
            retx.sessionToken = output[1];

            return retx;
        }

        public bool IsUserValid(Guid userId, string sessionToken)
        {
            UserData testUser = repository.GetByUserId(userId);
            string[] vectors = new string[] { testUser.sessionMaterial, sessionToken };
            sessionToken = Crypto.Decrypt(vectors);
            //oauthId:emailAddress:uid:timestamp
            string[] parts = sessionToken.Split(new char[] { Char.Parse("#") });
            DateTime checkDate = DateTime.Parse(parts[3]);
            if (!checkDate.ToString().Equals(testUser.lastLogin.ToString())) //login timestamps don't match...
                return false;
            if (checkDate < DateTime.UtcNow.AddHours(-2)) //session has expired
                return false;
            if (!parts[0].Equals(testUser.oauthId))
                return false;
            if (!parts[1].Equals(testUser.emailAddress))
                return false;
            if (!Guid.Parse(parts[2]).Equals(testUser.id))
                return false;

            return true;
        }

        private bool IsUserValidToUpdateUser(User userObj, Guid userId, string sessionToken)
        {
            UserData testUser = repository.GetByUserId(userId);
            if (testUser == null)
                return false;
            string[] vectors = new string[] { testUser.sessionMaterial, sessionToken };
            sessionToken = Crypto.Decrypt(vectors);
            //oauthId:emailAddress:uid:timestamp
            string[] parts = sessionToken.Split(new char[] { Char.Parse("#") });
            DateTime checkDate = DateTime.Parse(parts[3]);
            if (!checkDate.ToString().Equals(testUser.lastLogin.ToString())) //login timestamps don't match...
                return false;
            if (checkDate < DateTime.UtcNow.AddHours(-2)) //session has expired
                return false;
            if (!userObj.id.Equals(testUser.id)) // the user is trying to update a user object other than their own...
                return false;
            if (!parts[0].Equals(testUser.oauthId))
                return false;
            if (!parts[1].Equals(testUser.emailAddress))
                return false;
            if (!Guid.Parse(parts[2]).Equals(testUser.id))
                return false;

            return true;
        }


        public HttpResponseMessage PostUser(User user)
        {
            if (user == null)
            {
                throw new HttpResponseException(HttpStatusCode.MethodNotAllowed);
            }

            UserData check = repository.Get(user.emailAddress);
            if (check != null)
            {
                throw new HttpResponseException(HttpStatusCode.MethodNotAllowed);
            }


            DateTime dt = DateTime.UtcNow;
            //now encrypt the three pieces (UID:email:lastLogin)
            string toEncrypt = user.oauthId + "#" + user.emailAddress + "#" + user.id + "#" + dt.ToString();
            string[] output = Crypto.Encrypter(toEncrypt);
            //now store the sessionMaterial and the lastlogin
            user.lastLogin = dt;
            user.sessionToken = output[1];
            UserData ret = AutoMapper.Mapper.Map<User, UserData>(user);
            ret.sessionMaterial = output[0];
            ret = repository.Add(ret);

            User retx = AutoMapper.Mapper.Map<UserData, User>(ret);
            var response = Request.CreateResponse<User>(HttpStatusCode.Created, retx);

            string uri = Url.Link("DefaultApi", new { id = user.id });
            response.Headers.Location = new Uri(uri);
            return response;
        }

        public void PutUser(PostPutUser user)//update
        {
            if (user == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
            //checks to make sure the user exists, the user's token is valid and they are trying to update their own user object
            if (!IsUserValidToUpdateUser(user.user, user.userId, user.sessionToken)) 
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

            UserData ret = AutoMapper.Mapper.Map<User, UserData>(user.user);
            ret = repository.Update(ret);
            User retx = AutoMapper.Mapper.Map<UserData, User>(ret);


        }

        //public User Get(string emailAddress)
        //{
        //    UserData result = repository.Get(emailAddress);
        //    User ret = AutoMapper.Mapper.Map<UserData, User>(result);

        //    return ret;
        //}



        public IEnumerable<User> GetAll(string sessionToken)
        {
            List<UserData> result = (List<UserData>)repository.GetList();
            List<User> users = new List<Models.UI.User>();
            foreach (UserData ud in result)
            {
                users.Add(AutoMapper.Mapper.Map<UserData, User>(ud));
            }
            return users;
        }
        
        private bool DoesFBUserDataNeedUpdated(User storedUser, User passedUser)
        {
            if (!storedUser.locale.Equals(passedUser.locale)) 
            {
                return true;
            }
            if (!storedUser.firstName.Equals(passedUser.firstName))
            {
                return true;
            }
            if (!storedUser.lastName.Equals(passedUser.lastName)) 
            {
                return true;
            }
            if (!storedUser.timezoneFromUTC.Equals(passedUser.timezoneFromUTC))
            {
                return true;
            }
            if (!storedUser.name.Equals(passedUser.name)) 
            {
                return true;
            }
            if (!storedUser.gender.Equals(passedUser.gender)) 
            {
                return true;
            }
            if (!storedUser.location.Equals(passedUser.location)) 
            {
                return true;
            }
           
            return false;
        }

    }
}
