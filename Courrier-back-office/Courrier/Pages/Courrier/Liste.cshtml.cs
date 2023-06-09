using Courrier.DAL;
using Courrier.Models;
using Courrier.Models.courrier;
using Courrier.Models.Courrier;
using Courrier.Services;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Courrier.Pages.Courrier
{
    [Route("/Courrier/Liste")]

    public class ListeModel : PageModel
    {
        private readonly UserService _userService;
        private readonly CourrierService _courrierService;
        private readonly AppDbContext _dbContext; 
        private readonly ExportPdfService _exportPdfService;

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 2;
        public Pages<CourrierDestinataire> CourriersPage { get; set; }

        public User CurrentUser;
        public List<Flag> Flags { get; set; }
        public List<Statut> Statuts { get; set; }
        public List<Departement> Departements { get; set; }
        [BindProperty(SupportsGet = true)]
        public int? flag { get; set; }
        [BindProperty(SupportsGet = true)]
        public int? statut { get; set; }
        [BindProperty(SupportsGet = true)]
        public string expediteur { get; set; }
        [BindProperty(SupportsGet = true)]
        public int? departement { get; set; }
        [BindProperty(SupportsGet = true)]
        public string reference { get; set; }

        [HttpGet]
        public IActionResult OnGet()
        {
            PageNumber = 1;
            if (int.TryParse(Request.Query["page"].FirstOrDefault(), out int pageValue))
            {
                PageNumber = pageValue;
            } 
            ClaimsPrincipal currentUser = User;

            Flags = _dbContext.Flags.ToList();
            Statuts = _dbContext.Statuts.ToList();
            Departements = _dbContext.Departements.ToList();

            string email = currentUser.Identity.Name;
            CurrentUser = _userService.findByEmail(email);

            Pages<CourrierDestinataire> courriersPage = null;

            if (string.IsNullOrEmpty(expediteur) && string.IsNullOrEmpty(reference) && flag == null && statut == null && departement == null)
            {
                courriersPage = _courrierService.GetCourriersPageByUser(CurrentUser, PageNumber, PageSize);
            }
            else
            {
                courriersPage = _courrierService.GetCourriersByCriteria(CurrentUser, PageNumber, PageSize, flag, statut, expediteur, departement, reference);
            }

            CourriersPage = courriersPage;

            return Page();
        }


        public ListeModel(CourrierService courrierService, UserService userService
                , AppDbContext dbContext, ExportPdfService exportPdfService)
        {
            _courrierService = courrierService;
            _userService = userService;
            _dbContext = dbContext;
            _exportPdfService = exportPdfService;
        }

        public IActionResult OnPostTransfererSecretaire(int courrierId)
        {
            // Add your logic here for transferring to a secretary
            // You can use the 'courrierId' parameter to perform the necessary operations
            _courrierService.TransfererSecretaire(courrierId);
            // Redirect back to the current page
            return RedirectToPage("/Courrier/Liste");
        }

        public IActionResult OnPostTransfererDirecteur(int courrierId)
        {
            // Add your logic here for sending to a director
            // You can use the 'courrierId' parameter to perform the necessary operations

            _courrierService.TransfererDirecteur(courrierId);
            // Redirect back to the current page
            return RedirectToPage("/Courrier/Liste");
        }

        public ActionResult OnPostExporterPdf()
        {
            ClaimsPrincipal currentUser = User;
            string email = currentUser.Identity.Name;
            CurrentUser = _userService.findByEmail(email);
            // Récupérer les données de la liste des courriers
            var courriers = _courrierService.GetCourriersByCriteriaSansPage(CurrentUser, flag, statut, expediteur, departement, reference);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                // Exportez les courriers vers le flux MemoryStream
                _exportPdfService.ExportCourriersToPdf(courriers, memoryStream);

                // Récupérez les données du MemoryStream
                byte[] pdfBytes = memoryStream.ToArray();

                // Retournez le fichier PDF en tant que résultat de l'action
                return File(pdfBytes, "application/pdf", "liste_courriers.pdf");
            }
        }
    }
}
