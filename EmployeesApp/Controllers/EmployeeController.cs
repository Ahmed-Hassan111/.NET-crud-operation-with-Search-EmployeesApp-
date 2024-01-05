using Microsoft.AspNetCore.Mvc;
using EmployeesApp.Models;

namespace EmployeesApp.Controllers
{
    public class EmployeeController : Controller
    {
        HRDataBaseContext dbContext = new HRDataBaseContext();
        public enum SortDirection
        {
            Ascending,
            Descending
        }

        public IActionResult Index(string SortField , string CurrentSortField , SortDirection SortDirection , string SearchByName)
        {
            
            var employees = GetEmployees();

            if (!string.IsNullOrEmpty(SearchByName))
                employees = employees.Where(e => e.EmployeeName.ToLower().Contains(SearchByName.ToLower()));

            return View(this.SortEmployees(employees.ToList(), SortField,CurrentSortField,SortDirection));
        }

        private IQueryable<Employee>  GetEmployees()
        {
            var employees = (from employee in dbContext.Employees
                             join department in dbContext.Departments on employee.DepartmentId equals department.DepartmentId
                             select new Employee
                             {
                                 EmployeeId = employee.EmployeeId,
                                 EmployeeName = employee.EmployeeName,
                                 EmployeeNumber = employee.EmployeeNumber,
                                 DOB = employee.DOB,
                                 HiringDate = employee.HiringDate,
                                 GrossSalary = employee.GrossSalary,
                                 NetSalary = employee.NetSalary,
                                 DepartmentId = employee.DepartmentId,
                                 DepartmentName = department.DepartmentName,
                             });
            
            return employees;
        }



        //Create 
        public IActionResult Create()
        {
            ViewBag.Departments = this.dbContext.Departments.ToList();
            return View();
        }


        [HttpPost]
        public IActionResult Create(Employee model)
        {
            ModelState.Remove("EmployeeId");
            ModelState.Remove("DepartmentName");
            ModelState.Remove("Department");

            if (ModelState.IsValid)
            {
                dbContext.Employees.Add(model);
                dbContext.SaveChanges();
                return RedirectToAction("Index");

            }
            ViewBag.Departments = this.dbContext.Departments.ToList();
            return View();
        }


        //Edit 
        public IActionResult Edit(int ID)
        {
            Employee data = this.dbContext.Employees.Where(e=> e.EmployeeId == ID).FirstOrDefault();
     
            ViewBag.Departments = this.dbContext.Departments.ToList();
            return View("Create" , data);

        }


        [HttpPost]
        public IActionResult Edit(Employee model)
        {
            ModelState.Remove("EmployeeId");
            ModelState.Remove("DepartmentName");
            ModelState.Remove("Department");

            if (ModelState.IsValid)
            {
                dbContext.Employees.Update(model);
                dbContext.SaveChanges();
                return RedirectToAction("Index");

            }
            ViewBag.Departments = this.dbContext.Departments.ToList();
            return View("Create",model);
        }
        
        public IActionResult Delete(int ID) 
        {
            Employee data = this.dbContext.Employees.Where(e => e.EmployeeId == ID).FirstOrDefault();

            if (data != null)
            {
                dbContext.Employees.Remove(data);
                dbContext.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        // Sort
        private List<Employee> SortEmployees(List<Employee> employees , string sortField , string currentSortField , SortDirection sortDirection)
        {
            if (string.IsNullOrEmpty(sortField))
            {
                ViewBag.SortField = "EmployeeNumber";
                ViewBag.SortDirection = SortDirection.Ascending;
            }
            else
            {
                if (currentSortField == sortField)
                {
                    ViewBag.SortDirection = sortDirection == SortDirection.Ascending ? SortDirection.Descending : SortDirection.Ascending;
                }
                else
                    ViewBag.SortDirection = SortDirection.Ascending;
                    ViewBag.SortField = sortField;
                var propertyInfo = typeof(Employee).GetProperty(ViewBag.SortField);
                if (ViewBag.SortDirection == SortDirection.Ascending) 
                {
                    employees = employees.OrderBy(e => propertyInfo.GetValue(e,null)).ToList();
                }
                else
                {
                    employees = employees.OrderByDescending(e => propertyInfo.GetValue(e, null)).ToList();

                }
            }
            return employees;
        }


    }
}
