using CourrierFront.Models;
using CourrierFront.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Utilities.Zlib;
using SelectPdf;
using System.Diagnostics;
using System.Reflection;
using System.Security.Cryptography.Xml;

namespace CourrierFront.Controllers
{
    public class HomeController : Controller
    {
        private readonly CourrierDataAccess _courrierDataAccess;
        private readonly ExportPdfService _exportPdfService;
        private readonly IConfiguration _configuration;

        public HomeController(CourrierDataAccess courrierDataAccess, ExportPdfService exportPdfService,
            IConfiguration configuration)
        {
            _courrierDataAccess = courrierDataAccess;
            _exportPdfService = exportPdfService;
            _configuration = configuration;
        }

        public IActionResult Index(CourrierViewModel model)
        {
            List<Courrier> courriers = _courrierDataAccess.GetCourriers(
                model.Reference, model.Expediteur, model.Objet, model.Page, model.PageSize);
            string urlBack = _configuration.GetValue<string>("AppSettings:url_back");

            ViewBag.UrlBack = urlBack;

            model.Courriers = courriers;
            int totalCount = _courrierDataAccess.GetTotalCourriersCount(model.Reference, model.Expediteur, model.Objet);
            model.TotalCount = totalCount;
            return View(model);
        }

        public ActionResult ExportToPdf(CourrierViewModel model)
        {
            List<Courrier> courriers = _courrierDataAccess.GetCourriersNoPaginate(
                model.Reference, model.Expediteur, model.Objet);


            using (MemoryStream memoryStream = new MemoryStream())
            {
                // Exportez les courriers vers le flux MemoryStream
                _exportPdfService.ExportCourriersToPdf(courriers, memoryStream);

                // Récupérez les données du MemoryStream
                byte[] pdfBytes = memoryStream.ToArray();

                // Retournez le fichier PDF en tant que résultat de l'action
                return File(pdfBytes, "application/pdf", "courriers.pdf");
            }
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