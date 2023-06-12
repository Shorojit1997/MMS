using Microsoft.AspNetCore.Http;
using MMS.Authentication.Models.DTO.Incomming;
using MMS.Entities.Dtos.Incomming;
using MMS.Entities.Dtos.Outgoing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.DataService.IService
{

    public interface IProfileService
    {
        Task<ProfileUpdateResponseDTO> DetailsService(string Id);
        Task<bool> UpdateProfileService(ProfileUpdateRequestDTO person, IFormFile ImageFile, string Id);
        Task<bool> ChangePasswordService(ChangePasswordDTO Details,string id);
    }
}
