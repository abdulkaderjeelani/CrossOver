using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CRMApplication.Controllers
{
    public class HomeController : Controller
    {        
        public ActionResult Launch()
        {
            return View();
        }

        public IActionResult Index()
        {           
            return View();
        }

        public IActionResult Logout()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

    
    }
}
