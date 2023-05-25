using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Courrier.Pages
{

    public class LoginModel : PageModel
    {
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(ILogger<LoginModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            // Code ex�cut� lors du chargement de la page (m�thode HTTP GET)
        }

        public IActionResult OnPost(string username, string password)
        {
            // Code ex�cut� lo

            // Redirigez l'utilisateur vers une autre page apr�s la connexion r�ussie
            return RedirectToPage("/Index");
        }
    }
}
