using Microsoft.AspNetCore.Mvc; // Для IActionResult и TempData
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore; // Для Include, ToListAsync
using MyWebApp.Data;
using MyWebApp.Models;
using System.Collections.Generic; // Для List

namespace MyWebApp.Pages
{
	public class ReviewsModel : PageModel
	{
		private readonly ApplicationDbContext _context;
		public List<Review> Reviews { get; set; } = new List<Review>();
		public int? SelectedProcedureId { get; set; }

		[TempData] // Добавляем StatusMessage
		public string StatusMessage { get; set; } = string.Empty;

		public ReviewsModel(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task OnGetAsync(int? procedureId)
		{
			SelectedProcedureId = procedureId;

			Reviews = await _context.Reviews
				.Include(r => r.Client)
				.Include(r => r.Procedure)
				.OrderBy(r => r.Procedure.Name)
				.ToListAsync();
		}

		// --- Метод для удаления отзыва ---
		public async Task<IActionResult> OnPostDeleteAsync(int id)
		{
			var review = await _context.Reviews.FindAsync(id);

			if (review == null)
			{
				StatusMessage = "Ошибка: Отзыв не найден.";
				return RedirectToPage();
			}

			try
			{
				_context.Reviews.Remove(review);
				await _context.SaveChangesAsync();
				StatusMessage = "Отзыв успешно удален.";
			}
			catch (DbUpdateException)
			{
				StatusMessage = "Ошибка: Не удалось удалить отзыв из-за проблем с базой данных.";
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