using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;

namespace MyWebApp.Pages
{
    public class AddEditMedicalExamModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public AddEditMedicalExamModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public MedicalExam MedicalExam { get; set; }

        public SelectList ClientList { get; set; }
        public SelectList EmployeeList { get; set; }
        public SelectList ProcedureList { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id.HasValue)
            {
                MedicalExam = await _context.MedicalExams.FindAsync(id.Value);
                if (MedicalExam == null) return NotFound();
            }
            else
            {
                MedicalExam = new MedicalExam { Date = DateTime.Today, Result = "Годен" };
            }

            await LoadDropdowns();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return Page();
            }

            if (MedicalExam.Id == 0)
            {
                _context.MedicalExams.Add(MedicalExam);
            }
            else
            {
                _context.MedicalExams.Update(MedicalExam);
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("./MedicalExams");
        }

        private async Task LoadDropdowns()
        {
            ClientList = new SelectList(await _context.Clients.ToListAsync(), "Id", "LastName");
            EmployeeList = new SelectList(await _context.Employees.ToListAsync(), "Id", "FullName");
            ProcedureList = new SelectList(await _context.Procedures.ToListAsync(), "Id", "Name");
        }
    }
}