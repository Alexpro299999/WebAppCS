using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering; // Для SelectList
using Microsoft.EntityFrameworkCore; // Для ToListAsync
using MyWebApp.Data;
using MyWebApp.Models;
using System.Collections.Generic; // Для List

namespace MyWebApp.Pages
{
	public class AddEditReviewModel : PageModel
	{
		private readonly ApplicationDbContext _context;

		public AddEditReviewModel(ApplicationDbContext context)
		{
			_context = context;
		}

		[BindProperty]
		public Review Review { get; set; } = new Review();

		public SelectList ClientOptions { get; set; } = default!; // Список клиентов для выпадающего списка
		public SelectList ProcedureOptions { get; set; } = default!; // Список процедур для выпадающего списка

		public async Task<IActionResult> OnGetAsync(int? id)
		{
			// Загружаем данные для выпадающих списков
			await PopulateSelectLists();

			if (id.HasValue)
			{
				Review = await _context.Reviews.FindAsync(id.Value);
				if (Review == null)
				{
					return NotFound();
				}
			}
			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (Review.ReviewId == 0)
			{
				_context.Reviews.Add(Review);
			}
			else
			{
				var existingReview = await _context.Reviews.FindAsync(Review.ReviewId);
				if (existingReview == null)
				{
					return NotFound();
				}

				existingReview.ClientId = Review.ClientId;
				existingReview.ProcedureId = Review.ProcedureId;
				existingReview.Rating = Review.Rating;
				existingReview.Comment = Review.Comment;

				_context.Reviews.Update(existingReview);
			}

			await _context.SaveChangesAsync();
			return RedirectToPage("/Reviews");
		}

		private async Task PopulateSelectLists()
		{
			ClientOptions = new SelectList(await _context.Clients.OrderBy(c => c.Fio).ToListAsync(), "ClientId", "Fio");
			ProcedureOptions = new SelectList(await _context.Procedures.OrderBy(p => p.Name).ToListAsync(), "ProcedureId", "Name");
		}
	}
}