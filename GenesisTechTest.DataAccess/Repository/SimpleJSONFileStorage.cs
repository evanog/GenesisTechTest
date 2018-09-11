using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GenesisTechTest.Common.Models;
using GenesisTechTest.DataAccess.Interfaces;
using Newtonsoft.Json;

namespace GenesisTechTest.DataAccess.Repository
{
    public class SimpleJSONFileStorage : IStorageRepository
    {
        private static readonly string _storageName = "simpleJSONFileStorage.txt";

        public User GetByEmailOrDefault(string emaild)
        {
            return GetAll().FirstOrDefault(x => x.Email == emaild);
        }

        public User GetByUserIdOrDefault(Guid userid)
        {
            return GetAll().FirstOrDefault(x => x.Id == userid);
        }

        public void Create(User user)
        {
            var users = GetAll();
            users.Add(user);
            WriteToDb(users);
        }

        public void UpdateLastLogin(Guid id, DateTime lastLogin, string token)
        {
            var users = GetAll();
            var user = users.FirstOrDefault(x => x.Id == id);
            if (user != null)
            {
                user.LastLoginOn = lastLogin;
                user.Token = token;
                WriteToDb(users);
            }
        }

        public bool IsEmailAlreadyExists(string email)
        {
            return GetAll().Any(x => x.Email == email);
        }

        private IList<User> GetAll()
        {
            var result = new List<User>();
            if (File.Exists(GetPath()))
            {
                var list = File.ReadAllText(GetPath());
                result = JsonConvert.DeserializeObject<List<User>>(list);
            }
            return result;
        }

        private string GetPath()
        {
            return AppContext.BaseDirectory + _storageName;
        }

        private void WriteToDb(IList<User> users)
        {
            var json = JsonConvert.SerializeObject(users);
            using (var fileStream = File.Open(GetPath(), FileMode.Create, FileAccess.Write))
            {
                using (var writer = new StreamWriter(fileStream))
                {
                    writer.Write(json);
                }
            }
        }
    }
}
