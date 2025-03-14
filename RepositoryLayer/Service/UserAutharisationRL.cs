using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Helper;
using RepositoryLayer.Interface;

namespace RepositoryLayer.Service
{
    public class UserAutharisationRL : IUserAutharisationRL
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly Jwt _jwt;
        public UserAutharisationRL(ApplicationDBContext dbContext, Jwt jwt) 
        {
            _dbContext = dbContext;
            _jwt = jwt;
        }    
        public bool Checkuser(string Email) 
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == Email);  
            if (user == null) 
            {
                return false;
            }
            return true;
        }

        public UserEntity RegisterUserRL(UserEntity newUser) 
        {
            _dbContext.Users.Add(newUser);
            _dbContext.SaveChanges();
            return newUser;
        }

        //public (bool found, UserEntity user) GetUserCredentialsRL(string email)
        //{
        //    UserEntity user = _dbContext.Users.FirstOrDefault(e => e.Email == email);

        //    if(user == null) 
        //    {
        //        return (false, user);
        //    }
        //    return(true, user);
            
        //}

        public (bool login, string token) LoginUserRL(string email, string password) 
        {
            UserEntity user = _dbContext.Users.FirstOrDefault(e => e.Email == email);

            if(user != null) 
            {
                bool PasswordMatch = PasswordHasher.VerifyPassword(password, user.Password);

                if (PasswordMatch)
                {
                    string token = _jwt.GenerateToken(user);
                    return (true, token);
                }
            }
            return (false, string.Empty);
        }
    }
}
