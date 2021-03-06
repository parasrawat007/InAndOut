using InAndOut.Data;
using InAndOut.Models;
using InAndOut.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InAndOut.Controllers
{
    public class ExpensesController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ExpensesController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            IEnumerable<Expense> expenses = _db.Expenses.ToList();

            foreach (var exp in expenses)
            {
                exp.ExpenseType = _db.ExpenseTypes.FirstOrDefault(e => e.Id == exp.ExpenseTypeId);
            }            
            return View(expenses);
        }
        public IActionResult Create()
        {
            IEnumerable<SelectListItem> list = _db.ExpenseTypes.Select(e => new SelectListItem { 
                Text=e.Name,
                Value=e.Id.ToString()
            });

            ExpenseVM vm = new ExpenseVM()
            {
                Expense = new Expense(),
                TypeDropDown= list
            };


            ViewBag.TypeDropDown = list;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ExpenseVM vm)
        {
            if (ModelState.IsValid)
            {
                _db.Expenses.Add(vm.Expense);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(vm.Expense);
        }               
             
        public IActionResult Delete(int? Id)
        {
            if (Id == null || Id == 0)
                return NotFound();
            var expense = _db.Expenses.Find(Id);
            if (expense == null)
                return NotFound();
            return View(expense);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? Id)
        {
            var expense = _db.Expenses.Find(Id);
            if (expense == null)
                return NotFound();
            _db.Expenses.Remove(expense);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Update(int? Id)
        {
            if (Id == null || Id == 0)
                return NotFound();           
            ExpenseVM vm = new ExpenseVM()
            { 
                Expense= _db.Expenses.Find(Id),
                TypeDropDown=_db.ExpenseTypes.Select(e=>new SelectListItem(){ 
                    Text=e.Name,
                    Value=e.Id.ToString()
                })
            };
            if (vm.Expense == null)
                return NotFound();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(ExpenseVM vm)
        {
            if (ModelState.IsValid)
            {
                _db.Expenses.Update(vm.Expense);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(vm);
        }

    }
}
