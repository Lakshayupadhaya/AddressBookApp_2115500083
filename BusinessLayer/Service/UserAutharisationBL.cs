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

namespace BusinessLayer.Service
{
    public class UserAutharisationBL : IUserAutharisationBL
    {
        private readonly IMapper _mapper;
        private readonly IUserAutharisationRL _userAuthRL;
        public UserAutharisationBL(IMapper mapper, IUserAutharisationRL userAuthRL) 
        {
            _mapper = mapper;
            _userAuthRL = userAuthRL;   
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
    }
}
