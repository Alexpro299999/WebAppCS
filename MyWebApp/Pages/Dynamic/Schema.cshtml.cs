using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;

namespace MyWebApp.Pages.Dynamic
{
    public class SchemaModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public SchemaModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<EavEntity> Entities { get; set; } = new();

        public async Task OnGetAsync()
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            Entities = await _context.EavEntities
                .Include(e => e.Attributes)
                .OrderBy(e => e.Name)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostCreateEntityAsync(string entityName)
        {
            if (string.IsNullOrWhiteSpace(entityName))
            {
                ModelState.AddModelError(string.Empty, "Название таблицы не может быть пустым.");
                await LoadData();
                return Page();
            }

            // Проверка на дубликат имени таблицы
            if (await _context.EavEntities.AnyAsync(e => e.Name == entityName))
            {
                ModelState.AddModelError(string.Empty, $"Таблица с именем '{entityName}' уже существует.");
                await LoadData();
                return Page();
            }

            _context.EavEntities.Add(new EavEntity { Name = entityName });
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRenameEntityAsync(int id, string newName)
        {
            if (string.IsNullOrWhiteSpace(newName)) return RedirectToPage();

            var entity = await _context.EavEntities.FindAsync(id);
            if (entity != null)
            {
                // Проверяем, не занято ли новое имя другой таблицей
                if (await _context.EavEntities.AnyAsync(e => e.Name == newName && e.Id != id))
                {
                    ModelState.AddModelError(string.Empty, $"Таблица с именем '{newName}' уже существует.");
                    await LoadData();
                    return Page();
                }

                entity.Name = newName;
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAddAttributeAsync(int entityId, string attrName, string dataType, int? linkedEntityId)
        {
            if (string.IsNullOrWhiteSpace(attrName)) return RedirectToPage();

            // Проверка на дубликат поля внутри одной таблицы
            var exists = await _context.EavAttributes
                .AnyAsync(a => a.EavEntityId == entityId && a.Name == attrName);

            if (exists)
            {
                ModelState.AddModelError(string.Empty, $"Поле '{attrName}' уже существует в этой таблице.");
                await LoadData();
                return Page();
            }

            var attr = new EavAttribute
            {
                Name = attrName,
                EavEntityId = entityId,
                DataType = dataType ?? "string"
            };

            if (dataType == "relation" && linkedEntityId.HasValue)
            {
                attr.LinkedEntityId = linkedEntityId.Value;
            }

            _context.EavAttributes.Add(attr);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAttributeAsync(int id)
        {
            var attr = await _context.EavAttributes.FindAsync(id);
            if (attr != null)
            {
                _context.EavAttributes.Remove(attr);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteEntityAsync(int id)
        {
            var entity = await _context.EavEntities.FindAsync(id);
            if (entity != null)
            {
                _context.EavEntities.Remove(entity);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }
    }
}