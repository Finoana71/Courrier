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
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 2;
        public Pages<CourrierDestinataire> CourriersPage { get; set; }

        public User CurrentUser;
        public List<Flag> Flags { get; set; }
        public List<Statut> Statuts { get; set; }
        public List<Departement> Departements { get; set; }

        public IActionResult OnGet(int page = 1, int? flag = null, int? statut = null, string expediteur = null, int? departement = null, string reference = null)
        {
            PageNumber = page;
            ClaimsPrincipal currentUser = User;

            Flags = _dbContext.Flags.ToList();
            Statuts = _dbContext.Statuts.ToList();
            Departements = _dbContext.Departements.ToList();

            string email = currentUser.Identity.Name;
            CurrentUser = _userService.findByEmail(email);


            int pageSize = 10;
            Pages<CourrierDestinataire> courriersPage = null;

            if (string.IsNullOrEmpty(expediteur) && string.IsNullOrEmpty(reference) && flag == null && statut == null && departement == null)
            {
                courriersPage = _courrierService.GetCourriersPageByUser(CurrentUser, page, pageSize);
            }
            else
            {
                courriersPage = _courrierService.GetCourriersByCriteria(CurrentUser, page, pageSize, flag, statut, expediteur, departement, reference);
            }

            CourriersPage = courriersPage;

            return Page();
        }


        public ListeModel(CourrierService courrierService, UserService userService, AppDbContext dbContext)
        {
            _courrierService = courrierService;
            _userService = userService;
            _dbContext = dbContext;

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
    }
}
