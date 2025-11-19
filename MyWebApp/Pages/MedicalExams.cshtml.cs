using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;

namespace MyWebApp.Pages
{
    public class MedicalExamsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public MedicalExamsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<MedicalExam> MedicalExams { get; set; }

        public async Task OnGetAsync()
        {
            MedicalExams = await _context.MedicalExams
                .Include(m => m.Client)
                .Include(m => m.Employee)
                .Include(m => m.Procedure)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var exam = await _context.MedicalExams.FindAsync(id);

            if (exam != null)
            {
                _context.MedicalExams.Remove(exam);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }
    }
}