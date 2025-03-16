using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer;
using ModelLayer.DTO;

namespace BusinessLayer.Interface
{
    public interface IUserAutharisationBL
    {
        Responce<RegisterResponceDTO> RegisterUserBL(UserRegistrationDTO newUser);

        Responce<string> LoginUserBL(LoginDTO loginCrediantials);

        Task<(bool Sent, bool found)> ForgotPasswordBL(string email);

        Task<bool> ResetPasswordBL(string token, string newPassword);
    }
}
