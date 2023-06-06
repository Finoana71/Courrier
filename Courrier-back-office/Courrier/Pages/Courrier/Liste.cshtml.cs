using Courrier.DAL;
using Courrier.Models;
using Courrier.Models.Courrier;
using Courrier.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Courrier.Pages.Courrier
{
    public class ListeModel : PageModel
    {
        private readonly UserService _userService;
        private readonly CourrierService _courrierService;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 2;
        public Pages<CourrierDestinataire> CourriersPage { get; set; }

        public User CurrentUser;
        public void OnGet(int? page)
        {
            PageNumber = page ?? 1;
            ClaimsPrincipal currentUser = User;

            string email = currentUser.Identity.Name;
            CurrentUser = _userService.findByEmail(email);

            CourriersPage = _courrierService.GetCourriersPageByUser(CurrentUser, PageNumber, PageSize);
        }

        public ListeModel(CourrierService courrierService, UserService userService)
        {
            _courrierService = courrierService;
            _userService = userService;
        }
    }
}
