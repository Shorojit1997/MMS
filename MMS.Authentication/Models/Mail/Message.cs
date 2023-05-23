using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.Authentication.Models.Mail
{
    public class Message
    {
        public List<MailboxAddress> To { set; get; }
        public string Subject { get; set; }
        public string Content { get; set; }

        public Message(IEnumerable<string> to,string subject,string content) {

            To= new List<MailboxAddress>();
            To.AddRange(to.Select(x=> new MailboxAddress("email",x)));
            Subject = subject;
            Content = content; 
        
        }
    }
}
