using Microsoft.AspNetCore.Identity;
using MMS.Authentication.Models.DTO.Incomming;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MMS.DataService.IService
{
    public interface IAuthService
    {
        Task<ClaimsIdentity> LoginPost(LoginRequestDTO person);

        Task<ClaimsIdentity> RegistrationPost(RegistrationRequestDTO person);

        Task<ClaimsIdentity> ConfirmMail(string Token, string Email);
        Task<bool> ResetPassword(ResetPasswordDTO Details);
    }
}
