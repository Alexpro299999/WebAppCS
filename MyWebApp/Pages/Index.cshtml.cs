using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;

namespace MyWebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public int ClientsCount { get; set; }
        public int ExamsCount { get; set; }
        public int EmployeesCount { get; set; }
        public int ProceduresCount { get; set; }

        public async Task OnGetAsync()
        {
            ClientsCount = await _context.Clients.CountAsync();
            ExamsCount = await _context.MedicalExams.CountAsync();
            EmployeesCount = await _context.Employees.CountAsync();
            ProceduresCount = await _context.Procedures.CountAsync();
        }
    }
}