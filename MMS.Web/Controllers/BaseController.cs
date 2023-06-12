using Microsoft.AspNetCore.Mvc;
using MMS.DataService.IRepository;
using MMS.DataService.Service;

namespace MMS.Web.Controllers
{
    public class BaseController : Controller
    {
        protected readonly ILogger<BaseController> _logger;
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IUnitOfService _unitOfService;
        public BaseController(IUnitOfWork unitOfWork, IUnitOfService unitOfService)
        {
            _unitOfWork = unitOfWork;
            _unitOfService = unitOfService;
        }
    }
}
