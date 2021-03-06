using asp.netCoreWebAppMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace asp.netCoreWebAppMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public JsonResult GetStudentDetails(int id)
        {
            TestStudentRepository repository = new TestStudentRepository();
            Student studentDetails = repository.GetStudentById(id);
            return Json(studentDetails);
        }

        public JsonResult GetStudentGetAllStudent()
        {
            TestStudentRepository repository = new TestStudentRepository();
            List<Student> studentDetails = repository.GetAllStudent();
            return Json(studentDetails);
        }
    }
}
