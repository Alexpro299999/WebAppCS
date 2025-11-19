using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyWebApp.Data;
using MyWebApp.Models;

namespace MyWebApp.Pages
{
    public class AddEditClientModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public AddEditClientModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Client Client { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id.HasValue)
            {
                Client = await _context.Clients.FindAsync(id.Value);
                if (Client == null) return NotFound();
            }
            else
            {
                Client = new Client();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            if (Client.Id == 0)
            {
                _context.Clients.Add(Client);
            }
            else
            {
                _context.Clients.Update(Client);
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("./Clients");
        }
    }
}