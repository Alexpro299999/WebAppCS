using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;

namespace MyWebApp.Pages.Dynamic
{
    public class EditRecordModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditRecordModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public EavEntity Entity { get; set; }
        public EavRecord Record { get; set; }
        public Dictionary<int, string> CurrentValues { get; set; } = new();
        public Dictionary<int, int?> CurrentRelations { get; set; } = new();
        public Dictionary<int, List<SelectListItem>> RelationOptions { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id, int entityId)
        {
            Entity = await _context.EavEntities
                .Include(e => e.Attributes)
                .ThenInclude(a => a.LinkedEntity)
                .FirstOrDefaultAsync(e => e.Id == entityId);

            if (Entity == null) return NotFound();

            Record = await _context.EavRecords
                .Include(r => r.Values)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (Record == null) return NotFound();

            // Заполняем текущие значения
            foreach (var attr in Entity.Attributes)
            {
                var val = Record.Values.FirstOrDefault(v => v.EavAttributeId == attr.Id);
                if (attr.DataType == "relation")
                {
                    CurrentRelations[attr.Id] = val?.LinkedRecordId;
                }
                else
                {
                    CurrentValues[attr.Id] = val?.Value;
                }
            }

            // Загружаем опции для связей
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
                    Text = GetRecordDisplayName(r),
                    Selected = CurrentRelations.ContainsKey(attr.Id) && CurrentRelations[attr.Id] == r.Id
                }).ToList();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id, int entityId, Dictionary<int, string> values, Dictionary<int, int?> relations)
        {
            var record = await _context.EavRecords
                .Include(r => r.Values)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (record == null) return RedirectToPage("Data", new { entityId });

            // Обновление простых значений
            if (values != null)
            {
                foreach (var item in values)
                {
                    var existingVal = record.Values.FirstOrDefault(v => v.EavAttributeId == item.Key);
                    if (existingVal != null)
                    {
                        existingVal.Value = item.Value;
                    }
                    else if (!string.IsNullOrWhiteSpace(item.Value))
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

            // Обновление связей
            if (relations != null)
            {
                foreach (var item in relations)
                {
                    var existingVal = record.Values.FirstOrDefault(v => v.EavAttributeId == item.Key);

                    if (existingVal != null)
                    {
                        // Если значение уже было, обновляем ссылку
                        if (item.Value.HasValue)
                        {
                            existingVal.LinkedRecordId = item.Value.Value;
                            existingVal.Value = null; // Очищаем текстовое поле на всякий случай
                        }
                        else
                        {
                            // Если связь убрали, удаляем значение
                            _context.EavValues.Remove(existingVal);
                        }
                    }
                    else if (item.Value.HasValue)
                    {
                        // Если значения не было, создаем
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
            return RedirectToPage("Data", new { entityId });
        }

        private string GetRecordDisplayName(EavRecord record)
        {
            if (record == null) return string.Empty;
            var nameAttr = record.Values.FirstOrDefault(v =>
                v.EavAttribute != null &&
                (v.EavAttribute.Name.ToLower().Contains("name") ||
                 v.EavAttribute.Name.ToLower().Contains("имя") ||
                 v.EavAttribute.Name.ToLower().Contains("название")));
            if (nameAttr != null && !string.IsNullOrEmpty(nameAttr.Value)) return nameAttr.Value;
            var firstVal = record.Values.FirstOrDefault(v => !string.IsNullOrEmpty(v.Value));
            if (firstVal != null) return firstVal.Value;
            return $"#{record.Id}";
        }
    }
}