using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BusinessLayer.Interface;
using ModelLayer.DTO;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;
using RepositoryLayer.Service;
using RepositoryLayer.Helper;
using ModelLayer;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using BusinessLayer.Email;
using RabbitMQ.Client;
using BusinessLayer.RabbitMQ;

namespace BusinessLayer.Service
{
    public class UserAutharisationBL : IUserAutharisationBL
    {
        private readonly IMapper _mapper;
        private readonly IUserAutharisationRL _userAuthRL;
        private readonly IConfiguration _config;
        private readonly Jwt _jwt;
        private readonly EmailHelper _email;
        private readonly Producer _rabitMQProducer;
        public UserAutharisationBL(IMapper mapper, IUserAutharisationRL userAuthRL, IConfiguration config, Jwt jwt, EmailHelper email, Producer rabitMQProducer) 
        {
            _mapper = mapper;
            _userAuthRL = userAuthRL;   
            _config = config;
            _jwt = jwt;
            _email = email;
            _rabitMQProducer = rabitMQProducer;
        }


        public Responce<RegisterResponceDTO> RegisterUserBL(UserRegistrationDTO newUser)
        {
            bool Existing = _userAuthRL.Checkuser(newUser.Email);
            if (!Existing) 
            {
                string hashPass = PasswordHasher.HashPassword(newUser.Password);
                newUser.Password = hashPass;

                UserEntity newUserEntity = _mapper.Map<UserEntity>(newUser);

                UserEntity registeredUser = _userAuthRL.RegisterUserRL(newUserEntity);
                RegisterResponceDTO registerResponce = _mapper.Map<RegisterResponceDTO>(registeredUser);

                Responce<RegisterResponceDTO> registerResponceBack = new Responce<RegisterResponceDTO>();
                registerResponceBack.Success = true;
                registerResponceBack.Message = "User Registered Successfully";
                registerResponceBack.Data = registerResponce;
                return registerResponceBack;

            }
            Responce<RegisterResponceDTO> registerResponceBack1 = new Responce<RegisterResponceDTO>();
            registerResponceBack1.Success = false;
            registerResponceBack1.Message = "User Already Exists";

            RegisterResponceDTO registerResponceFailed = new RegisterResponceDTO();  
            registerResponceFailed.Email = newUser.Email;

            registerResponceBack1.Data = registerResponceFailed;
            return registerResponceBack1;
           

        }

        public Responce<string> LoginUserBL(LoginDTO loginCrediantials) 
        {
            //(bool Found, string HashPass) = _userAuthRL.GetUserCredentialsRL(loginCrediantials.Email);

            (bool login, string token) = _userAuthRL.LoginUserRL(loginCrediantials.Email, loginCrediantials.Password);

            if(login) 
            {

                Responce<string> responce = new Responce<string>();
                responce.Success = true;
                responce.Message = "Login Successfull";
                responce.Data = token;  
                return responce;       
            }
            Responce<string> responce1 = new Responce<string>();
            responce1.Success = false;
            responce1.Message = "Incorrect Email or Password";
            responce1.Data = "No Token Generated";
            return responce1;
        }

        public async Task<(bool Sent, bool found)> ForgotPasswordBL(string email)
        {
            bool exists = _userAuthRL.Checkuser(email);
            if (!exists)
            {
                return (false, false);
            }

            var resetToken = _jwt.GenerateResetToken(email);

            // Publish message to RabbitMQ
            var message = new { Email = email, ResetToken = resetToken };
            _rabitMQProducer.PublishMessage(message);

            return (true, true); // Assume success (actual sending happens in Consumer)
        }

        public async Task<bool> ResetPasswordBL(string token, string newPassword)
        {
            var email = _jwt.ValidateResetToken(token);
            if (email == null) return false;

            string newHashPassword = PasswordHasher.HashPassword(newPassword);

            return await _userAuthRL.UpdateUserPassword(email, newHashPassword);
        }

        

    }
}
