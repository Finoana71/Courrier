using Courrier.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BCryptNet = BCrypt.Net.BCrypt;

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

        public IActionResult OnPost(string email, string password)
        {
            using (var dbContext = new AppDbContext())
            {
                var user = dbContext.Users.FirstOrDefault(u => u.email == email);

                if (user != null)
                {
                    bool isPasswordValid = BCryptNet.Verify(password, user.password);

                    if (isPasswordValid)
                    {
                        // Les informations d'identification sont valides, enregistrez l'utilisateur dans la session
                        HttpContext.Session.SetString("UserId", user.Id.ToString());
                        HttpContext.Session.SetString("Username", user.username);

                        return RedirectToPage("/Index");
                    }
                }

                ModelState.AddModelError(string.Empty, "Email ou mot de passe incorrect.");
            }
            return Page();
        }
    }
}
