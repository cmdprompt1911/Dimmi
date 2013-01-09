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

        public HttpResponseMessage PostUser(User user)
        {
            if (user == null)
            {
                throw new HttpResponseException(HttpStatusCode.MethodNotAllowed);
            }

            User check = repository.Get(user.emailAddress);
            if (check != null)
            {
                throw new HttpResponseException(HttpStatusCode.MethodNotAllowed);
            }

            repository.Add(user);
            user = (User)repository.Get(user.emailAddress);
            var response = Request.CreateResponse<User>(HttpStatusCode.Created, user);

            string uri = Url.Link("DefaultApi", new { id = user.id });
            response.Headers.Location = new Uri(uri);
            return response;
        }

        public void PutUser(User user)
        {
            if (user == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
            user = (User)repository.Update(user);
            
        }

        public User Get(string emailAddress)
        {
            return (User)repository.Get(emailAddress);
        }

        public IEnumerable<User> GetAll()
        {
            return repository.GetList();
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
