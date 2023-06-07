using Courrier.Models;
using Courrier.Models.Courrier;
using Courrier.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Courrier.Pages.Courrier
{
    public class TransfererModel : PageModel
    {

        private readonly CourrierService _courrierService;
        private readonly UserService _userService;

        public CourrierDestinataire Courrier { get; set; }    
        public List<User> Coursiers { get; set; }


        [BindProperty]
        public int selectedCoursier { get; set; } = 0;

        public TransfererModel(CourrierService courrierService, UserService userService)
        {
            _courrierService = courrierService;
            _userService = userService;
        }

        public IActionResult OnGet(int id)
        {
            Courrier = _courrierService.GetCourrierById(id);
            Coursiers = _userService.GetAllCoursier();

            if (Courrier == null)
            {
                // Gérer le cas où le courrier destinataire n'est pas trouvé
                return NotFound();
            }

            return Page();
        }

        public IActionResult OnPost(int id)
        {
            if (selectedCoursier != 0)
            {
                _courrierService.TransfererCoursier(id, selectedCoursier);
                return Redirect("/Courrier/Liste");
            }

            return Page();
        }
    }
}
