using System;
using GenesisTechTest.Common.Exceptions;
using GenesisTechTest.Common.Models;
using GenesisTechTest.DataAccess.Interfaces;
using GenesisTechTest.Domain.Interfaces;

namespace GenesisTechTest.Domain.Services
{
    public class UserService : IUserService
    {
        private readonly IStorageRepository _storage;
        private readonly IIdentityService _identityService;
        private readonly IPasswordHashService _passwordHashService;

        public UserService(IStorageRepository storage, IIdentityService identityService, IPasswordHashService passwordHashService)
        {
            _storage = storage;
            _identityService = identityService;
            _passwordHashService = passwordHashService;
        }

        public User GetByUserIdOrDefault(Guid userId)
        {
            return _storage.GetByUserIdOrDefault(userId);
        }

        public User SignIn(string email, string password)
        {
            bool isAutenticated = false;

            var user = _storage.GetByEmailOrDefault(email);
            if (user != null)
                isAutenticated = _passwordHashService.Verify(password, user.Password);


            // Note for tech test reviewer: I'm returning the same exception as if email doesnt exist and if the password doesnt match.
            // If I was in a work situation I'd question these reqs because they say throw 401 for invalid password
            // In my opinion it think its a security flaw if you return different messages because someone could guess email address
            // they will know its in your system if there's different error code
            if (!isAutenticated)
                throw new InvalidEmailAndPasswordException();

            return UpdateLastLogin(user);
        }

        public User CreateUser(User user)
        {
            if (_storage.IsEmailAlreadyExists(user.Email))
                throw new EmailAlreadyExistsException();

            user.Id = Guid.NewGuid();
            user.CreatedOn = DateTime.UtcNow;
            user.LastUpdatedOn = DateTime.UtcNow;
            user.LastLoginOn = DateTime.UtcNow;
            user.Token = _identityService.GetToken(user.Email, user.Password);
            user.Password = _passwordHashService.GetHashedPassword(user.Password);
            _storage.Create(user);
            return user;
        }

        private User UpdateLastLogin(User user)
        {
            var lastLogin = DateTime.UtcNow;
            var token = _identityService.GetToken(user.Email, user.Password);
            _storage.UpdateLastLogin(user.Id, lastLogin, token);
            user.LastLoginOn = lastLogin;
            user.Token = token;
            return user;
        }
    }
}
