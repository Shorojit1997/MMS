using Microsoft.AspNetCore.Http;
using MMS.DataService.IRepository;
using MMS.DataService.IService;
using MMS.Entities.Dtos.Incomming;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.DataService.Service
{
    public class ExpenseService:IExpenseService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ExpenseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }

        
    }

}
