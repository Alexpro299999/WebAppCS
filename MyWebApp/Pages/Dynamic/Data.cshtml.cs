using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;

namespace MyWebApp.Pages.Dynamic
{
    public class DataModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DataModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public EavEntity? Entity { get; set; }
        public List<EavEntity> AllEntities { get; set; } = new();
        public List<EavRecord> Records { get; set; } = new();
        public Dictionary<int, List<SelectListItem>> RelationOptions { get; set; } = new();

        [TempData]
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int? entityId)
        {
            AllEntities = await _context.EavEntities.OrderBy(e => e.Name).ToListAsync();

            if (!AllEntities.Any())
            {
                return RedirectToPage("Schema");
            }

            if (!entityId.HasValue)
            {
                entityId = AllEntities.First().Id;
            }

            Entity = await _context.EavEntities
                .Include(e => e.Attributes)
                .ThenInclude(a => a.LinkedEntity)
                .FirstOrDefaultAsync(e => e.Id == entityId.Value);

            if (Entity == null)
            {
                return RedirectToPage("Schema");
            }

            Records = await _context.EavRecords
                .Include(r => r.Values)
                .ThenInclude(v => v.LinkedRecord)
                .ThenInclude(lr => lr.Values)
                .ThenInclude(lrv => lrv.EavAttribute)
                .Where(r => r.EavEntityId == entityId.Value)
                .ToListAsync();

            foreach (var attr in Entity.Attributes.Where(a => a.DataType == "relation" && a.LinkedEntityId.HasValue))
            {
                var linkedRecords = await _context.EavRecords
                    .Include(r => r.Values)
                    .ThenInclude(v => v.EavAttribute)
                    .Where(r => r.EavEntityId == attr.LinkedEntityId.Value)
                    .ToListAsync();

                RelationOptions[attr.Id] = linkedRecords.Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = GetRecordDisplayName(r)
                }).ToList();
            }

            return Page();
        }

        public string GetRecordDisplayName(EavRecord record)
        {
            if (record == null) return string.Empty;

            var nameAttr = record.Values.FirstOrDefault(v =>
                v.EavAttribute != null &&
                (v.EavAttribute.Name.ToLower().Contains("name") ||
                 v.EavAttribute.Name.ToLower().Contains("имя") ||
                 v.EavAttribute.Name.ToLower().Contains("название")));

            if (nameAttr != null && !string.IsNullOrEmpty(nameAttr.Value))
                return nameAttr.Value;

            var firstVal = record.Values.FirstOrDefault(v => !string.IsNullOrEmpty(v.Value));
            if (firstVal != null) return firstVal.Value;

            return $"#{record.Id}";
        }

        public async Task<IActionResult> OnPostAddRecordAsync(int entityId, Dictionary<int, string> values, Dictionary<int, int?> relations)
        {
            var record = new EavRecord { EavEntityId = entityId };
            _context.EavRecords.Add(record);
            await _context.SaveChangesAsync();

            if (values != null)
            {
                foreach (var item in values)
                {
                    if (!string.IsNullOrWhiteSpace(item.Value))
                    {
                        _context.EavValues.Add(new EavValue
                        {
                            EavRecordId = record.Id,
                            EavAttributeId = item.Key,
                            Value = item.Value
                        });
                    }
                }
            }

            if (relations != null)
            {
                foreach (var item in relations)
                {
                    if (item.Value.HasValue)
                    {
                        _context.EavValues.Add(new EavValue
                        {
                            EavRecordId = record.Id,
                            EavAttributeId = item.Key,
                            LinkedRecordId = item.Value.Value
                        });
                    }
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToPage(new { entityId });
        }

        public async Task<IActionResult> OnPostDeleteRecordAsync(int id, int entityId)
        {
            // Проверка: ссылается ли кто-то на эту запись
            var referencingValue = await _context.EavValues
                .Include(v => v.EavRecord)
                .ThenInclude(r => r.EavEntity)
                .FirstOrDefaultAsync(v => v.LinkedRecordId == id);

            if (referencingValue != null)
            {
                ErrorMessage = $"Нельзя удалить запись #{id}, так как на неё ссылается запись #{referencingValue.EavRecordId} из таблицы '{referencingValue.EavRecord.EavEntity.Name}'. Сначала удалите или измените ссылающуюся запись.";
                return RedirectToPage(new { entityId });
            }

            var record = await _context.EavRecords.FindAsync(id);
            if (record != null)
            {
                try
                {
                    _context.EavRecords.Remove(record);
                    await _context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    ErrorMessage = "Ошибка при удалении записи. Возможно, существуют скрытые связи.";
                }
            }
            return RedirectToPage(new { entityId });
        }
    }
}