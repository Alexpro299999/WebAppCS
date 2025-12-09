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

        public List<EavEntity> Entities { get; set; }

        public async Task OnGetAsync()
        {
            Entities = await _context.EavEntities
                .Include(e => e.Attributes)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostCreateEntityAsync(string entityName)
        {
            if (!string.IsNullOrWhiteSpace(entityName))
            {
                _context.EavEntities.Add(new EavEntity { Name = entityName });
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRenameEntityAsync(int id, string newName)
        {
            var entity = await _context.EavEntities.FindAsync(id);
            if (entity != null && !string.IsNullOrWhiteSpace(newName))
            {
                entity.Name = newName;
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAddAttributeAsync(int entityId, string attrName, string dataType, int? linkedEntityId)
        {
            if (!string.IsNullOrWhiteSpace(attrName))
            {
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
            }
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