using CourrierFront.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Cryptography.Xml;

namespace CourrierFront.Controllers
{
    public class HomeController : Controller
    {
        private readonly CourrierDataAccess _courrierDataAccess;

        public HomeController(CourrierDataAccess courrierDataAccess)
        {
            _courrierDataAccess = courrierDataAccess;
        }

        public IActionResult Index(CourrierViewModel model)
        {
            List<Courrier> courriers = _courrierDataAccess.GetCourriers(
                model.Reference, model.Expediteur, model.Objet, model.Page, model.PageSize);

            model.Courriers = courriers;
            int totalCount = _courrierDataAccess.GetTotalCourriersCount(model.Reference, model.Expediteur, model.Objet);
            model.TotalCount = totalCount;
            return View(model);
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