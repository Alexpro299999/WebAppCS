using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;
using System.Collections.Generic; // Добавлено для IList

namespace MyWebApp.Pages
{
	public class ClientsModel : PageModel
	{
		private readonly ApplicationDbContext _context;
		public List<Client> Clients { get; set; } = new List<Client>();

		[TempData] // <--- ДОБАВЛЕНО: Атрибут TempData
		public string StatusMessage { get; set; } = string.Empty; // <--- ИЗМЕНЕНО: теперь StatusMessage доступен для записи из TempData

		public ClientsModel(ApplicationDbContext context)
		{
			_context = context;
		}

		// Загрузка данных
		public async Task OnGetAsync()
		{
			Clients = await _context.Clients
				.Include(c => c.Reviews) // Убедитесь, что Client имеет навигационное свойство Reviews
				.ToListAsync();
		}

		// Метод удаления
		public async Task<IActionResult> OnPostDeleteAsync(int id)
		{
			var client = await _context.Clients
									   .Include(c => c.Reviews) // Включаем отзывы для проверки
									   .FirstOrDefaultAsync(c => c.ClientId == id);

			if (client == null)
			{
				StatusMessage = "Ошибка: Клиент не найден.";
				return RedirectToPage();
			}


			if (client.Reviews != null && client.Reviews.Any())
			{
				StatusMessage = "Ошибка: Невозможно удалить клиента, так как существуют связанные заказы (отзывы)."; // Уточнил сообщение
				return RedirectToPage();
			}
			try
			{
				_context.Clients.Remove(client);
				await _context.SaveChangesAsync();
				StatusMessage = "Клиент успешно удален.";
			}
			catch (DbUpdateException)
			{
				StatusMessage = "Ошибка: Не удалось удалить клиента из-за проблем с базой данных (возможно, существуют непредвиденные связанные записи).";
			}
			catch (Exception ex)
			{
				StatusMessage = $"Произошла непредвиденная ошибка: {ex.Message}";
			}

			return RedirectToPage();
		}
	}
}