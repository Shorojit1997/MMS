using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MMS.DataService.IConfiguration;
using MMS.Entities.DbSet;
using MMS.Entities.Dtos.Incomming;

namespace MMS.Web.Controllers
{
    [Authorize]
    public class ExpenseController : BaseController
    {
        public ExpenseController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Expenses(string MonthId,string MessId)
        {
            try
            {
                if (MonthId == null && MessId == null)
                    return RedirectToAction("ShowHistory", "Dashboard");
                
                if (MonthId == null)
                    return RedirectToAction("ShowMonthHistory", "Dashboard", new { MessId = MessId });
                

                var month = await _unitOfWork.Months.GetById(Guid.Parse(MonthId));
                if (month == null) {
                    throw new Exception("Invalid MonthId");
                }
                ViewBag.MonthName=month.Name;
                var expense = new ExpenseDTO()
                {
                    MonthId = month.Id,
                    MessId=month.MessId,
                };

                var expenseList= await _unitOfWork.Expenses.GetExpensesByMonthId(month.Id);
                var PersonId = HttpContext.User.Identity.Name;

                ViewBag.Expenses = expenseList;
                ViewBag.MessId = MessId;
                ViewBag.MonthId = MonthId;
                ViewBag.PersonId = PersonId;

                return View(expense);

            }
            catch(Exception ex)
            {
                TempData["Error"]=ex.Message;
                return RedirectToAction("ShowHistory", "Dashboard");
            }

        }



        [HttpPost]
        public async Task<IActionResult> Create(ExpenseDTO expense)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    throw new Exception("Invalid Parameters");
                }

                var month= await _unitOfWork.Months.GetById(expense.MonthId);
                if(month == null) { throw new Exception("Invalid Request"); }

                var newExpense = new Expense()
                {
                    Title = expense.Title,
                    Description = expense.Description,
                    Amount = expense.Amount,
                    MonthId = month.Id,
                    MessId = month.MessId,
                };

                await _unitOfWork.Expenses.Add(newExpense);
                await _unitOfWork.CompleteAsync();

                return RedirectToAction("Expenses", "Expense", new { MessId=month.MessId,MonthId=month.Id });

            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("ShowHistory", "Dashboard");
            }

        }


        [HttpGet]
        public async Task<IActionResult> Delete(string ExpenseId )
        {
            try
            {
                if (ExpenseId == null) throw new Exception("Invalid request");

                var expense= await _unitOfWork.Expenses.GetById(Guid.Parse(ExpenseId));
                if (expense == null) { throw new Exception("Invalid attempt"); }


                _unitOfWork.Expenses.Delete(expense);
                await _unitOfWork.CompleteAsync();

                return RedirectToAction("Expenses", "Expense", new { MessId = expense.MessId, MonthId = expense.MonthId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("ShowHistory", "Dashboard");
            }
        }

   
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    }
}
