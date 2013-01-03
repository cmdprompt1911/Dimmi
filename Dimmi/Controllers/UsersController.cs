using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Dimmi.Data;
using Dimmi.Models;
using Dimmi.DataInterfaces;


namespace Dimmi.Controllers
{
    public class UsersController : ApiController
    {
        static readonly IUserRepository repository = new UserRepository();

        public HttpResponseMessage PostByFB(User user)
        {
            if (user == null)
            {
                throw new HttpResponseException(HttpStatusCode.MethodNotAllowed);
            }
            repository.UpdateLoginTimeStamp(user.emailAddress);
            User storedUser = repository.Get(user.emailAddress);
            if (storedUser == null)
            {
                //create it
                user = repository.AddFromFBData(user);
            }
            else if (DoesFBUserDataNeedUpdated(storedUser, user))
            {
                user = repository.UpdateFromFBData(user);
            }
            else
            {
                user = storedUser;
            }
            
            var response = Request.CreateResponse<User>(HttpStatusCode.Created, user);

            string uri = Url.Link("DefaultApi", new { id = user.id });
            response.Headers.Location = new Uri(uri);
            return response;
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
            if (!storedUser.fBUsername.Equals(passedUser.fBUsername)) 
            {
                return true;
            }
            if (!storedUser.fBLink.Equals(passedUser.fBLink)) 
            {
                return true;
            }
            return false;
        }

    }
}
