using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyWebApp.Data;
using MyWebApp.Models;

namespace MyWebApp.Pages
{
    public class AddEditEmployeeModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public AddEditEmployeeModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Employee Employee { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id.HasValue)
            {
                Employee = await _context.Employees.FindAsync(id.Value);
                if (Employee == null) return NotFound();
            }
            else
            {
                Employee = new Employee();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            if (Employee.Id == 0)
            {
                _context.Employees.Add(Employee);
            }
            else
            {
                _context.Employees.Update(Employee);
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("./Employees");
        }
    }
}