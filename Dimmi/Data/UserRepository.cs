using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using Dimmi.Models;
using Dimmi.DataInterfaces;
using MongoDB.Driver.Builders;



namespace Dimmi.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DBRepository.MongoRepository<User> _userRepository;
        private const string TypeDiscriminatorField = "_t";

        public UserRepository()
        {
            _userRepository = new DBRepository.MongoRepository<User>("Users");
        }

        public IEnumerable<User> GetList()
        {
            return _userRepository.Collection.FindAll();
        }
        
        public User Get(string emailAddress)
        {
            var query = Query.EQ("emailAddress", emailAddress);
            User user = _userRepository.Collection.FindOne(query);
            return user;
   
        }

        public User GetByName(string name)
        {
            var query = Query.EQ("name", name);
            User user = _userRepository.Collection.FindOne(query);
            return user;
        }


        public void UpdateLoginTimeStamp(string emailAddress)
        {
            var query = Query.EQ("emailAddress", emailAddress);

            User userToUpdate = Get(emailAddress);
            userToUpdate.lastLogin = DateTime.UtcNow;
            _userRepository.Collection.Save(userToUpdate);
        }

        public User Add(User user)
        {
            _userRepository.Collection.Insert(user);
            return Get(user.emailAddress);
        }

        public User Update(User user)
        {
            _userRepository.Collection.Save(user);
            return Get(user.emailAddress);
        }


    }
}