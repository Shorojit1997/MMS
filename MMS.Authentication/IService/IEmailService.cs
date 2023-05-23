using MMS.Authentication.Models.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.Authentication.IService
{
    public interface IEmailService
    {
        Task<bool> SendMail(Message message);
    }
}
