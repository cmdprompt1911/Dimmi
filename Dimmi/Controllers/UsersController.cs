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
        readonly IUserRepository _repository;
        static readonly IReviewStatisticsRepository _stat_repository = new ReviewStatisticsRepository();
        
        public UsersController()
        {
            _repository = new UserRepository();
        }

        public UsersController(IUserRepository repository)
        {
            this._repository = repository;
        }
        




        public virtual User Get(string sessionRequest, string sessionMaterial)
        {
                        
            //decrypt the session request
            //request should be in the form of uid:emailaddress -- the UID is from facebook.  If the user exists, the UID will be stored on their account.  can compare the UID and Email to get a valid user
            // then generate a access token based upon the user's UID, email and the LAST timestamp for when the user logged on (update from this method.
            // they will be requred to pass this access token with every request.  We can then compare it to the three values stored in the db (UID, email, lastaccessed) to determine if this is a valid user.
            PathProvider p = new PathProvider();
            string request = Crypto.Decrypt(new string[] { sessionMaterial, sessionRequest }, p);
            string[] parts = request.Split(new char[] { Char.Parse("#") });

            UserData checkUser = _repository.Get(parts[0], parts[1]);
            if (checkUser == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            //we have a valid user. 
            //update the lastLogin to NOW
            DateTime dt = DateTime.UtcNow;
            int hoursToExpire = int.Parse(System.Configuration.ConfigurationManager.AppSettings["SessionExpireHours"]);
            DateTime expiration = dt.AddHours(hoursToExpire);

            //now encrypt the three pieces (UID:email:lastLogin:timestamp)
            string toEncrypt = checkUser.oauthId + "#" + checkUser.emailAddress + "#" + checkUser.id + "#" + dt.ToString();
            
            string[] output = Crypto.Encrypter(toEncrypt, p);
            //now store the sessionMaterial and the lastlogin
            checkUser.lastLogin = dt;
            checkUser.sessionMaterial = output[0];
            checkUser.sessionToken = output[1];
            checkUser.expires = expiration;
            checkUser = _repository.Update(checkUser);

            //now map to UI model...
            User retx = AutoMapper.Mapper.Map<UserData, User>(checkUser);
            
            //populate the sessionToken and expiration and return;
            retx.sessionToken = output[1];
            //retx.expires = expiration;

            return retx;
        }

        public Guid GetUserIdFromHeaders(HttpRequestMessage request)
        {
            string[] auths = (string[])request.Headers.GetValues("Session");
            if (auths.Length == 0)
                return Guid.Empty;

            string[] total = auths[0].Split(new char[1] { Char.Parse(":") });
            Guid userId = Guid.Parse(total[0]);
            return userId;
        }

        //public bool IsUserValid(Guid userId, string sessionToken)
        public bool IsUserValid(HttpRequestMessage request)
        {
            //get the session and id from the headers
            string[] auths = (string[])request.Headers.GetValues("Session");
            if (auths.Length == 0)
                return false;

            string[] total = auths[0].Split(new char[1] { Char.Parse(":") });
            Guid userId = Guid.Parse(total[0]);
            string sessionToken = total[1];

            UserData testUser = _repository.GetByUserId(userId);
            string[] vectors = new string[] { testUser.sessionMaterial, sessionToken };
            PathProvider p = new PathProvider();
            sessionToken = Crypto.Decrypt(vectors, p);
            int hoursToExpire = int.Parse(System.Configuration.ConfigurationManager.AppSettings["SessionExpireHours"]);
            //oauthId:emailAddress:uid:timestamp
            string[] parts = sessionToken.Split(new char[] { Char.Parse("#") });
            DateTime checkDate = DateTime.Parse(parts[3]);
            if (!checkDate.ToString().Equals(testUser.lastLogin.ToString())) //login timestamps don't match...
                return false;
            if (checkDate.AddHours(hoursToExpire) <= DateTime.UtcNow) //session has expired
                return false;
            if (!parts[0].Equals(testUser.oauthId))
                return false;
            if (!parts[1].Equals(testUser.emailAddress))
                return false;
            if (!Guid.Parse(parts[2]).Equals(testUser.id))
                return false;

            return true;
        }

        private bool IsUserValidToUpdateUser(User userObj)
        {
            //get the session and id from the headers
            string[] auths = (string[])Request.Headers.GetValues("Session");
            if (auths.Length == 0)
                return false;

            string[] total = auths[0].Split(new char[1] { Char.Parse(":") });
            Guid userId = Guid.Parse(total[0]);
            string sessionToken = total[1];


            UserData testUser = _repository.GetByUserId(userId);
            if (testUser == null)
                return false;
            string[] vectors = new string[] { testUser.sessionMaterial, sessionToken };
            PathProvider p = new PathProvider();
            sessionToken = Crypto.Decrypt(vectors,p);
            int hoursToExpire = int.Parse(System.Configuration.ConfigurationManager.AppSettings["SessionExpireHours"]);
            //oauthId:emailAddress:uid:timestamp
            string[] parts = sessionToken.Split(new char[] { Char.Parse("#") });
            DateTime checkDate = DateTime.Parse(parts[3]);
            if (!checkDate.ToString().Equals(testUser.lastLogin.ToString())) //login timestamps don't match...
                return false;
            if (checkDate.AddHours(hoursToExpire) <= DateTime.UtcNow) //session has expired
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

            UserData check = _repository.Get(user.emailAddress);
            if (check != null)
            {
                throw new HttpResponseException(HttpStatusCode.MethodNotAllowed);
            }


            DateTime dt = DateTime.UtcNow;
            //now encrypt the three pieces (UID:email:lastLogin)
            string toEncrypt = user.oauthId + "#" + user.emailAddress + "#" + user.id + "#" + dt.ToString();
            PathProvider p = new PathProvider();
            string[] output = Crypto.Encrypter(toEncrypt, p);
            //now store the sessionMaterial and the lastlogin
            user.lastLogin = dt;
            user.sessionToken = output[1];
            UserData ret = AutoMapper.Mapper.Map<User, UserData>(user);
            int hoursToExpire = int.Parse(System.Configuration.ConfigurationManager.AppSettings["SessionExpireHours"]);
            DateTime expiration = dt.AddHours(hoursToExpire);
            ret.expires = expiration;
            ret.sessionMaterial = output[0];
            ret = _repository.Add(ret);

            User retx = AutoMapper.Mapper.Map<UserData, User>(ret);
            var response = Request.CreateResponse<User>(HttpStatusCode.Created, retx);

            string uri = Url.Link("DefaultApi", new { id = user.id });
            response.Headers.Location = new Uri(uri);
            return response;
        }

        public void PutUser(User user)//update
        {
            if (user == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
            //checks to make sure the user exists, the user's token is valid and they are trying to update their own user object
            if (!IsUserValidToUpdateUser(user)) 
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

            //UserData checkExpiration = repository.GetByUserId(user.id);

            //int hoursToExpire = int.Parse(System.Configuration.ConfigurationManager.AppSettings["SessionExpireHours"]);
           // DateTime expiration = checkExpiration.lastLogin.AddHours(hoursToExpire);

            UserData ret = AutoMapper.Mapper.Map<User, UserData>(user);
            //ret.expires = expiration;
            ret = _repository.Update(ret);
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
            List<UserData> result = (List<UserData>)_repository.GetList();
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
