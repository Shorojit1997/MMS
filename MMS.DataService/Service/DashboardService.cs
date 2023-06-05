using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using MMS.DataService.IRepository;
using MMS.DataService.IService;
using MMS.Entities.DbSet;
using MMS.Entities.Dtos.Incomming;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.DataService.Service
{
    public class DashboardService : IDashboardServices
    {
        private readonly IUnitOfWork _unitOfWork;
        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }
        public async Task<bool> Create(MessRequestDTO mess, string PersonId)
        {

            var newMess = new Mess()
            {
                Name = mess.Name,
                StartDate = mess.StartDate,
                UpdatedAt = DateTime.Now,
            };
     
            var personAddMess = new MessHaveMember()
            {
                IsManager = true,
                PersonId = Guid.Parse(PersonId),
                MessId = newMess.Id,
                UpdatedAt = DateTime.Now,
            };

            await _unitOfWork.Messes.Add(newMess);
            await _unitOfWork.MessHaveMembers.Add(personAddMess);
            await _unitOfWork.CompleteAsync();
            return true;

         
        }

        public async Task<bool> AddIntoMess(MessRequestDTO mess)
        {
            try
            {
                var messDetails = await _unitOfWork.Messes.GetById(mess.Id);
                messDetails.Name = mess.Name ?? messDetails.Name;
                messDetails.StartDate = mess.StartDate != null ? mess.StartDate : messDetails.StartDate;

                _unitOfWork.Messes.Update(messDetails);

                await _unitOfWork.CompleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public Task<bool> EditMessHistory(MessRequestDTO mess)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> AddIntoMess(string MessId, string UserId,string type, string curUserId)
        {
            try
            {
                var person = await _unitOfWork.MessHaveMembers
                    .GetByMessIdAndPersonId(Guid.Parse(MessId), Guid.Parse(curUserId));
                 if (person != null && person.IsManager == false)
                  {
                     return false;
                   }

                var addIntoMess = new MessHaveMember()
                {
                    PersonId = Guid.Parse(UserId),
                    MessId = Guid.Parse(MessId),
                    IsManager = (type == "Manager") ? true : false,
                    UpdatedAt = DateTime.Now,

                };

                var newMonth= await _unitOfWork.Months.GetLastMonthByMessId(Guid.Parse(MessId));

                if (newMonth != null)
                {
                    List<Days> days = new List<Days>();
                    for (int i = 1; i < 32; i++)
                    {
                        Days days2 = new Days()
                        {
                            Number = i,
                            Breakfast = 0,
                            Lunch = 0,
                            Dinner = 0,
                            Person_Id = Guid.Parse(UserId),
                            Month_Id = newMonth.Id
                        };

                        days.Add(days2);
                    }

                    await _unitOfWork.Days.AddRange(days);
                  
                }
                await _unitOfWork.MessHaveMembers.Add(addIntoMess);
                await _unitOfWork.CompleteAsync();

                return true;
            }
            catch(Exception e) {
                return false;
            }
        }
    }
}
