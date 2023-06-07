using Courrier.DAL;
using Courrier.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
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

        public async Task<ActionResult> OnPostAsync(string email, string password)
        {
            using (var dbContext = new AppDbContext())
            {
                var user = dbContext.Users.FirstOrDefault(u => u.Email == email);

                if (user != null)
                {
                    bool isPasswordValid = BCryptNet.Verify(password, user.Password);

                    if (isPasswordValid)
                    {
                        user.Role = dbContext.Roles.Find(user.IdRole);
                        var claims = new List<Claim>{
                            new Claim(ClaimTypes.Name, user.Email),
                            new Claim(ClaimTypes.Role, user.Role.Id + ""),
                            // Add other role claims as needed
                        };

                        var claimsIdentity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
                        await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, new ClaimsPrincipal(claimsIdentity));
                        // Redirect to a protected page or perform any other desired action
                        return RedirectToPage("/Index");
                    }
                }

                ModelState.AddModelError(string.Empty, "Email ou mot de passe incorrect.");
            }
            return Page();
        }
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostLogoutAsync()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToPage("/Index");
        }
    }
}
