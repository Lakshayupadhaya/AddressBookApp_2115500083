using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryLayer.Entity;

namespace RepositoryLayer.Interface
{
    public interface IUserAutharisationRL
    {
        bool Checkuser(string Email);

        UserEntity RegisterUserRL(UserEntity newUser);

        //(bool found, string hashPass) GetUserCredentialsRL(string email);

        (bool login, string token) LoginUserRL(string email, string password);
    }
}
