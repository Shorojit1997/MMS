using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using MMS.DataService.IRepository;
using MMS.DataService.IService;
using MMS.DataService.Others;
using MMS.Entities.DbSet;
using MMS.Entities.Dtos.Incomming;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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

        public async Task<bool> EditMessHistory(MessRequestDTO mess)
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

       

        public async Task<bool> AddIntoMess(string MessId, string UserId,string type, string curUserId)
        {
            try
            {
                //finding current user
                var person = await _unitOfWork.MessHaveMembers
                    .GetByMessIdAndPersonId(Guid.Parse(MessId), Guid.Parse(curUserId));

                //checking the user is manager or not
                 if (person != null && person.IsManager == false)
                  {
                     return false;
                   }

                //add a user into a mess
                var addIntoMess = new MessHaveMember()
                {
                    PersonId = Guid.Parse(UserId),
                    MessId = Guid.Parse(MessId),
                    IsManager = (type == "Manager") ? true : false,
                    UpdatedAt = DateTime.Now,

                };

                //get the last month under this mess
                var newMonth= await _unitOfWork.Months.GetLastMonthByMessId(Guid.Parse(MessId));
                
                //checking the month is empty or not
                if (newMonth != null)
                {
                    //finding the user's day's information because this user was previously a member of this mess or not.
                    var existingDays = await _unitOfWork.Days.GetDaysByMonthIdAndPersonId(newMonth.Id, Guid.Parse(UserId));

                    //checking any document is found or not
                    if (existingDays.Count()==0) 
                    {
                        //if not exist , created new days under this month.
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
                    
                  
                }
                //add into mess
                await _unitOfWork.MessHaveMembers.Add(addIntoMess);
                await _unitOfWork.CompleteAsync();

                return true;
            }
            catch(Exception e) {
                return false;
            }
        }

        public async Task<bool> CreateNewMonth(MonthDTO monthDetails,string id)
        {
            //get messId from month.id because MessId can not null
            var messId = monthDetails.Id;

            var member = await _unitOfWork.MessHaveMembers.GetByMessIdAndPersonId(messId, Guid.Parse(id));
            //checking the null
            if (member == null)
                throw new Exception("Something happend error! please try again");

            //check this member is manager or not

            if (!member.IsManager)
                throw new Exception("Sorry! Only mess manager can create the month history");


            var person = await _unitOfWork.Persons.GetById(Guid.Parse(id));
            //adding the new month history
            var newMonth = new Month()
            {
                Name = monthDetails.Name,
                StartDate = monthDetails.StartDate,
                AddedBy = person.Name,
                MessId = messId,
                UpdatedAt = DateTime.Now,
            };

            //Creating this month days history for this user.
            var allMembers = await _unitOfWork.MessHaveMembers.GetAllMembersByMessId(messId);

            //adding all days accoding to the newly created month.
            List<Days> days = new List<Days>();
          
            foreach (var singleMember in allMembers)
            {
                for (int i = 1; i < 32; i++)
                {
                    Days days2 = new Days()
                    {
                        Number = i,
                        Breakfast = 0,
                        Lunch = 0,
                        Dinner = 0,
                        Person_Id = singleMember.Id,
                        Month_Id = newMonth.Id
                    };

                    days.Add(days2);
                }
            }


            await _unitOfWork.Months.Add(newMonth);
            await _unitOfWork.Days.AddRange(days);
            await _unitOfWork.CompleteAsync();

            return true;
           
        }

        public async Task<Guid> DeleteMonthHistory(string MonthId, string PersonId)
        {
            //checking the null value
            if (string.IsNullOrEmpty(MonthId) || !ValidityChecker.IsValidGuid(MonthId))
                throw new Exception("Invalid request.");

            //finding the month detalis by monthId
            var monthDetails = await _unitOfWork.Months.GetById(Guid.Parse(MonthId));

            //checking the this month exist or not in the database
            if (monthDetails == null)
                throw new Exception("Invalid request.");
            

            //get MessId from the month informations
            Guid messId = monthDetails.MessId != null ? monthDetails.MessId : Guid.NewGuid();


            var member = await _unitOfWork.MessHaveMembers.GetByMessIdAndPersonId(messId, Guid.Parse(PersonId));

            if (member == null)
                throw new Exception("Internal server error");
          
            //checking the current user have role manager or not
            if (!member.IsManager)
                throw new Exception("Sorry you have't any permission!");
           
            //delete from the database and change the database
            await _unitOfWork.Months.Delete(monthDetails);
            await _unitOfWork.CompleteAsync();
            //return messId to controller
            return monthDetails.MessId;
        }

        public async Task<MonthDTO> EditMonthHistory(string MessId, string MonthId)
        {
            if (string.IsNullOrEmpty(MonthId) || string.IsNullOrEmpty(MessId) || !ValidityChecker.IsValidGuid(MonthId) || !ValidityChecker.IsValidGuid(MessId))
                throw new Exception("Invalid route");
          
            //get month details by monthId
            var month = await _unitOfWork.Months.GetById(Guid.Parse(MonthId));

            //null checking 
            if (month == null)
                throw new Exception("Invalid request");
            var requestDto = new MonthDTO()
            {
                Id = month.Id,
                MessId = Guid.Parse(MessId),
                Name = month.Name,
                StartDate = month.StartDate,
            };

            return requestDto;
        }

        public async Task<MonthDTO> EditMonthHistoryForPostController(MonthDTO month, string PersonId)
        {
            if (!ValidityChecker.IsValidGuid(month.Id) || !ValidityChecker.IsValidGuid(Convert.ToString(month.MessId)))
                throw new Exception("Invalid request");

           
            var messId = month.MessId ?? Guid.NewGuid();
            var details = await _unitOfWork.MessHaveMembers.GetByMessIdAndPersonId(messId, Guid.Parse(PersonId));

            if (details == null || !details.IsManager)
                throw new Exception( "Sorry! You have no edit permission");
           
            var existingMonth = await _unitOfWork.Months.GetById(month.Id);

            if (existingMonth == null)
                throw new Exception("Sorry! You have provided wrong informations");
            
            //change the updated informations
            existingMonth.Name = month.Name ?? existingMonth.Name;
            existingMonth.StartDate = month.StartDate != null ? month.StartDate : existingMonth.StartDate;


            _unitOfWork.Months.Update(existingMonth);

            await _unitOfWork.CompleteAsync();

            return month;
        }

        public async Task<bool> DeleteMessHistory(string MessId, string PersonId)
        {
            if (string.IsNullOrEmpty(MessId) || !ValidityChecker.IsValidGuid(MessId))
                throw new Exception("Invalid request");


            var member = await _unitOfWork.MessHaveMembers
                .GetByMessIdAndPersonId(Guid.Parse(MessId), Guid.Parse(PersonId));

            if (member == null)
                throw new Exception("Somethings Happened wrong!");
            if (!member.IsManager)
                throw new Exception("Sorry you have't any permission!");


            await _unitOfWork.Messes.DeleteMessByMessId(Guid.Parse(MessId));
            await _unitOfWork.CompleteAsync();

            return true;
        }
    }
}
