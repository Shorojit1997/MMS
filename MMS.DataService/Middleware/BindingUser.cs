using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using MMS.DataService.Data;
using MMS.DataService.IConfiguration;

namespace MMS.DataService.Middleware
{
    public class BindingUser : IMiddleware
    {
        private readonly IUnitOfWork _unitOfWork;
        public BindingUser(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var Id = context.User.Identity.Name;
            if(Id != null)
            {
               var person= await _unitOfWork.Persons.GetById(Guid.Parse(Id));
                if(person != null)
                {
                    context.Items["Person"]= person;
                }
            }
            
            await next(context);
        }
    }
}
