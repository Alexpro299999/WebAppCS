using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyWebApp.Pages
{
    public class ProceduresModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public List<Procedure> Procedures { get; set; } = new List<Procedure>();

        [TempData]
        public string StatusMessage { get; set; } = string.Empty;

        public ProceduresModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task OnGetAsync()
        {
            Procedures = await _context.Procedures
                .Include(p => p.Reviews)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var procedure = await _context.Procedures
                .Include(p => p.Reviews) 
                .FirstOrDefaultAsync(p => p.Id == id); 

            if (procedure == null)
            {
                StatusMessage = "Ошибка: Процедура не найдена.";
                return RedirectToPage();
            }


            if (procedure.Reviews != null && procedure.Reviews.Any())
            {
                StatusMessage = "Ошибка: Невозможно удалить процедуру, так как существуют связанные отзывы.";
                return RedirectToPage();
            }

            try
            {
                _context.Procedures.Remove(procedure);
                await _context.SaveChangesAsync();
                StatusMessage = "Процедура успешно удалена.";
            }
            catch (DbUpdateException)
            {
                StatusMessage = "Ошибка: Не удалось удалить процедуру из-за проблем с базой данных.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Произошла непредвиденная ошибка: {ex.Message}";
            }

            return RedirectToPage();
        }
    }
}