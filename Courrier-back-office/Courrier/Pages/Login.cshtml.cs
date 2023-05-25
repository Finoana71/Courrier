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
            // Code exécuté lors du chargement de la page (méthode HTTP GET)
        }

        public IActionResult OnPost(string username, string password)
        {
            // Code exécuté lo

            // Redirigez l'utilisateur vers une autre page après la connexion réussie
            return RedirectToPage("/Index");
        }
    }
}
