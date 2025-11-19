using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;

namespace MyWebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public int TotalClients { get; set; }
        public int TotalProcedures { get; set; }
        public int TotalReviews { get; set; }

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task OnGetAsync()
        {
            TotalClients = await _context.Clients.CountAsync();
            TotalProcedures = await _context.Procedures.CountAsync();
            TotalReviews = await _context.Reviews.CountAsync();
        }
    }
}