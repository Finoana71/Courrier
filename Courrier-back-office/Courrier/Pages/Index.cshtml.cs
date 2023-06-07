using Courrier.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Courrier.Pages
{
    public class IndexModel : PageModel
    {
        private readonly CourrierService _courrierService;

        public Dictionary<string, int> CourriersParFlag { get; set; }
        public Dictionary<string, int> CourriersParStatut { get; set; }

        public IndexModel(CourrierService courrierService)
        {
            _courrierService = courrierService;
        }

        public void OnGet()
        {
            // Récupérer les statistiques
            CourriersParFlag = _courrierService.GetStatCourrierFlag();
            CourriersParStatut = _courrierService.GetStatCourrierStatut();
        }
    }
}