using CourrierFront.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CourrierFront.Controllers
{
    public class HomeController : Controller
    {
        private readonly CourrierDataAccess _courrierDataAccess;

        public HomeController(CourrierDataAccess courrierDataAccess)
        {
            _courrierDataAccess = courrierDataAccess;
        }

        public ActionResult Index(string reference, string expediteur, string objet, int page = 1, int pageSize = 10)
        {
            List<Courrier> courriers = _courrierDataAccess.GetCourriers(reference, expediteur, objet, page, pageSize);
            return View(courriers);
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
    }
}