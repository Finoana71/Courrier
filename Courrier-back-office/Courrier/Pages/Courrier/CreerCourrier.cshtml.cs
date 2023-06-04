using Courrier.DAL;
using Courrier.Models;
using Courrier.Models.Courrier;
using Courrier.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace Courrier.Pages.Courrier
{
    public class CreerCourrierModel : PageModel
    {

        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly AppDbContext _dbContext;
        private readonly CourrierService _courrierService;
        private readonly UserService _userService;
        private List<Departement> departements { get; set; }

        [BindProperty]
        public CourrierModel Courrier { get; set; } = default!;

        [BindProperty]
        public IFormFile FileUpload { get; set; }

        [BindProperty]
        public List<string> SelectedDepartements{ get; set; }

        public void OnGet()
        {
            ViewData["Departements"] = new SelectList(_dbContext.Departements, "Id", "Nom");
            ViewData["IdFlag"] = new SelectList(_dbContext.Flags, "Id", "Libelle");

        }

        public CreerCourrierModel(IWebHostEnvironment webHostEnvironment, AppDbContext dbContext,
            CourrierService courrierService, UserService userService
            )
        {
            _webHostEnvironment = webHostEnvironment;
            _dbContext = dbContext;
            _userService = userService;
            //departements = _dbContext.Departements.ToList();
            _courrierService = courrierService;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ClaimsPrincipal currentUser = User;
            //if (!ModelState.IsValid || _context.Courriers == null || Courrier == null)
            if (_dbContext.Courriers == null || Courrier == null)
            {
                return Page();
            }

            string email = currentUser.Identity.Name;
            User user = _userService.findByEmail(email);
            _courrierService.Creer(Courrier, user, SelectedDepartements, FileUpload);
            await _dbContext.SaveChangesAsync();

            return RedirectToPage("/Courrier/Liste");
        }
    }
}
