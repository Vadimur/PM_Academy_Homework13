using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DepsWebApp.AppDbContext;
using DepsWebApp.AppDbContext.Entities;
using DepsWebApp.Models;

namespace DepsWebApp.Services.CustomAuthentication
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class CustomAuthenticationService : ICustomAuthenticationService
    {
        private readonly AuthenticationDbContext _dbContext;

        public CustomAuthenticationService(AuthenticationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string> Register(RegistrationModel registrationModel)
        {
            if (string.IsNullOrWhiteSpace(registrationModel.Login) || string.IsNullOrWhiteSpace(registrationModel.Password))
            {
                return null;
            };

            bool isAlreadyExists = _dbContext.Users.Any(u => u.Login.Equals(registrationModel.Login));

            if (isAlreadyExists)
            {
                return null;
            }

            UserEntity userEntity = new UserEntity()
            {
                Login = registrationModel.Login,
                Password = registrationModel.Password
            };

            await _dbContext.Users.AddAsync(userEntity);
            await _dbContext.SaveChangesAsync();

            string plainCredentials = $"{registrationModel.Login}:{registrationModel.Password}";
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainCredentials);
            string authToken = Convert.ToBase64String(plainTextBytes);

            return authToken;


            /*if (string.IsNullOrWhiteSpace(registrationModel.Login) || string.IsNullOrWhiteSpace(registrationModel.Password))
            {
                return null;
            }

            string plainCredentials = $"{registrationModel.Login}:{registrationModel.Password}";
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainCredentials);
            string authToken = Convert.ToBase64String(plainTextBytes);
            
            User authDetails = new User(registrationModel.Login, registrationModel.Password);
            if (_storage.Save(authDetails))
            {
                return authToken;
            }
            else
            {
                return null;
            }*/

        }

        public bool Authenticate(string login, string password)
        {
            
            return _dbContext.Users.FirstOrDefault(u => u.Login.Equals(login) && u.Password.Equals(password)) != null;

            /*if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                return false;
            }
            
            User authenticationDetails = _storage.GetUser(login, password);

            return authenticationDetails != null;*/
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}