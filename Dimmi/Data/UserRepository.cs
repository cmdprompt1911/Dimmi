using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using Dimmi.Models.Domain;
using Dimmi.DataInterfaces;
using MongoDB.Driver.Builders;



namespace Dimmi.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DBRepository.MongoRepository<UserData> _userRepository;
        private const string TypeDiscriminatorField = "_t";

        public UserRepository()
        {
            _userRepository = new DBRepository.MongoRepository<UserData>("Users");
        }

        public IEnumerable<UserData> GetList()
        {
            List<UserData> users = _userRepository.Collection.FindAll().ToList();
            return users;
        }

        public UserData Get(string emailAddress)
        {
            var query = Query.EQ("emailAddress", emailAddress);
            UserData user = _userRepository.Collection.FindOne(query);
            return user;
   
        }

        public UserData GetByUserId(Guid userId)
        {
            var query = Query.EQ("_id", userId.ToString());
            UserData user = _userRepository.Collection.FindOne(query);
            return user;

        }

        public UserData Get(string oathId, string emailAddress)
        {
            var query = Query.And(Query.EQ("emailAddress", emailAddress), Query.EQ("oauthId", oathId));
            UserData user = _userRepository.Collection.FindOne(query);
            return user;

        }

        public UserData Get(string oathId, string emailAddress, DateTime lastAccessed)
        {
            var query = Query.And(Query.EQ("emailAddress", emailAddress), Query.EQ("oauthId", oathId), Query.EQ("lastLogin", lastAccessed));
            UserData user = _userRepository.Collection.FindOne(query);
            return user;

        }

        public UserData GetByName(string name)
        {
            var query = Query.EQ("name", name);
            UserData user = _userRepository.Collection.FindOne(query);
            return user;
        }


        public void UpdateLoginTimeStamp(string emailAddress)
        {
            var query = Query.EQ("emailAddress", emailAddress);

            UserData userToUpdate = Get(emailAddress);
            userToUpdate.lastLogin = DateTime.UtcNow;
            _userRepository.Collection.Save(userToUpdate);
        }

        public UserData Add(UserData user)
        {
            _userRepository.Collection.Insert(user);
            return Get(user.emailAddress);
        }

        public UserData Update(UserData user)
        {
            _userRepository.Collection.Save(user);
            return Get(user.emailAddress);
        }


    }
}