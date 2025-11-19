using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyWebApp.Data;
using MyWebApp.Models;
using System.IO;
using Microsoft.EntityFrameworkCore; // <--- ЭТА ДИРЕКТИВА БЫЛА ПРОПУЩЕНА

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
		public Client Client { get; set; } = new Client();

		[BindProperty]
		public IFormFile? UploadedPhoto { get; set; }

		public async Task<IActionResult> OnGetAsync(int? id)
		{
			if (id.HasValue)
			{
				Client = await _context.Clients.FindAsync(id.Value);
				if (Client == null)
				{
					return NotFound();
				}
			}
			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			// Убираем валидацию для Client.Photo, если у него есть [Required] атрибут
			// и пользователь не загружает новое фото.
			// Если у Client.Photo нет [Required], эту строку можно убрать.
			ModelState.Remove("Client.Photo");

			if (!ModelState.IsValid)
			{
				// Если валидация не прошла, но клиент уже имеет фото,
				// нужно его загрузить, чтобы оно отобразилось снова.
				// Это особенно важно при редактировании.
				if (Client.ClientId != 0)
				{
					var existingClientForReload = await _context.Clients
						.AsNoTracking() // Теперь AsNoTracking() должен быть доступен
						.FirstOrDefaultAsync(c => c.ClientId == Client.ClientId);
					if (existingClientForReload != null)
					{
						Client.Photo = existingClientForReload.Photo;
					}
				}
				return Page();
			}

			// Обработка загруженного фото
			if (UploadedPhoto != null && UploadedPhoto.Length > 0)
			{
				using (var memoryStream = new MemoryStream())
				{
					await UploadedPhoto.CopyToAsync(memoryStream);
					Client.Photo = memoryStream.ToArray();
				}
			}

			if (Client.ClientId == 0)
			{
				_context.Clients.Add(Client);
			}
			else
			{
				var existingClient = await _context.Clients.FindAsync(Client.ClientId);
				if (existingClient == null)
				{
					return NotFound();
				}

				existingClient.Fio = Client.Fio;
				existingClient.Phone = Client.Phone;

				if (UploadedPhoto != null && UploadedPhoto.Length > 0)
				{
					existingClient.Photo = Client.Photo;
				}

				_context.Clients.Update(existingClient);
			}

			await _context.SaveChangesAsync();
			return RedirectToPage("/Clients");
		}
	}
}