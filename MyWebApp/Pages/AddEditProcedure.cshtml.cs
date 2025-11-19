using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore; // Для AsNoTracking
using MyWebApp.Data;
using MyWebApp.Models;

namespace MyWebApp.Pages
{
	public class AddEditProcedureModel : PageModel
	{
		private readonly ApplicationDbContext _context;

		public AddEditProcedureModel(ApplicationDbContext context)
		{
			_context = context;
		}

		[BindProperty]
		public Procedure Procedure { get; set; } = new Procedure();

		public async Task<IActionResult> OnGetAsync(int? id)
		{
			if (id.HasValue)
			{
				Procedure = await _context.Procedures.FindAsync(id.Value);
				if (Procedure == null)
				{
					return NotFound();
				}
			}
			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}

			if (Procedure.ProcedureId == 0)
			{
				// Новая процедура
				_context.Procedures.Add(Procedure);
			}
			else
			{
				// Редактирование существующей процедуры
				var existingProcedure = await _context.Procedures.FindAsync(Procedure.ProcedureId);
				if (existingProcedure == null)
				{
					return NotFound();
				}

				existingProcedure.Name = Procedure.Name;
				existingProcedure.Price = Procedure.Price;

				_context.Procedures.Update(existingProcedure);
			}

			await _context.SaveChangesAsync();
			return RedirectToPage("/Procedures");
		}
	}
}