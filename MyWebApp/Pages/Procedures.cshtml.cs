using Microsoft.AspNetCore.Mvc; // Для IActionResult и TempData
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore; // Для Include, ToListAsync
using MyWebApp.Data;
using MyWebApp.Models;
using System.Collections.Generic; // Для List

namespace MyWebApp.Pages
{
	public class ProceduresModel : PageModel
	{
		private readonly ApplicationDbContext _context;
		public List<Procedure> Procedures { get; set; } = new List<Procedure>();

		[TempData] // Добавляем StatusMessage
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

		// --- Метод для удаления процедуры ---
		public async Task<IActionResult> OnPostDeleteAsync(int id)
		{
			var procedure = await _context.Procedures
										.Include(p => p.Reviews) // Включаем отзывы для проверки
										.FirstOrDefaultAsync(p => p.ProcedureId == id);

			if (procedure == null)
			{
				StatusMessage = "Ошибка: Процедура не найдена.";
				return RedirectToPage();
			}

			// Проверка на связанные записи (отзывы)
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
				StatusMessage = "Ошибка: Не удалось удалить процедуру из-за проблем с базой данных (возможно, существуют непредвиденные связанные записи).";
			}
			catch (Exception ex)
			{
				StatusMessage = $"Произошла непредвиденная ошибка: {ex.Message}";
			}

			return RedirectToPage();
		}
		// --- Конец метода удаления ---
	}
}