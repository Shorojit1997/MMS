using MMS.DataService.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.DataService.IConfiguration
{
    public interface IUnitOfWork
    {
        IPersonRepository Persons { get; }

        Task CompleteAsync();

    }
}
