using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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

        public EavEntity? CurrentEntity { get; set; }
        public List<EavRecord> Records { get; set; } = new();
        public List<EavEntity> AllEntities { get; set; } = new();

        // Данные для режима редактирования
        public int? EditRecordId { get; set; }
        public Dictionary<int, string> EditValues { get; set; } = new();

        public Dictionary<int, Dictionary<int, string>> LookupValues { get; set; } = new();

        public async Task OnGetAsync(int? entityId, int? editRecordId = null)
        {
            if (entityId == null || entityId == 0)
            {
                AllEntities = await _context.EavEntities.OrderBy(e => e.Name).ToListAsync();
                return;
            }

            CurrentEntity = await _context.EavEntities
                .Include(e => e.Attributes)
                .FirstOrDefaultAsync(e => e.Id == entityId);

            if (CurrentEntity != null)
            {
                Records = await _context.EavRecords
                    .Where(r => r.EavEntityId == entityId)
                    .Include(r => r.Values)
                    .ToListAsync();

                var relationAttrs = CurrentEntity.Attributes
                    .Where(a => a.DataType == "relation" && a.LinkedEntityId.HasValue)
                    .ToList();

                foreach (var attr in relationAttrs)
                {
                    await LoadLookupData(attr.LinkedEntityId.Value);
                }

                // Логика загрузки записи для редактирования
                if (editRecordId.HasValue)
                {
                    var recordToEdit = Records.FirstOrDefault(r => r.Id == editRecordId.Value);
                    if (recordToEdit != null)
                    {
                        EditRecordId = editRecordId;
                        EditValues = recordToEdit.Values.ToDictionary(v => v.EavAttributeId, v => v.Value ?? "");
                    }
                }
            }
            else
            {
                AllEntities = await _context.EavEntities.OrderBy(e => e.Name).ToListAsync();
            }
        }

        private async Task LoadLookupData(int linkedEntityId)
        {
            if (LookupValues.ContainsKey(linkedEntityId)) return;

            var records = await _context.EavRecords
                .Where(r => r.EavEntityId == linkedEntityId)
                .Include(r => r.Values)
                .ThenInclude(v => v.EavAttribute)
                .ToListAsync();

            var dict = new Dictionary<int, string>();
            foreach (var rec in records)
            {
                var nameVal = rec.Values.FirstOrDefault(v => v.EavAttribute.DataType == "string")?.Value;
                dict[rec.Id] = string.IsNullOrEmpty(nameVal) ? $"Запись #{rec.Id}" : nameVal;
            }
            LookupValues[linkedEntityId] = dict;
        }

        public async Task<IActionResult> OnPostSaveRecordAsync(int entityId, int? recordId)
        {
            EavRecord record;

            if (recordId.HasValue)
            {
                // Редактирование
                record = await _context.EavRecords
                    .Include(r => r.Values)
                    .FirstOrDefaultAsync(r => r.Id == recordId.Value);

                if (record == null) return NotFound();
            }
            else
            {
                // Создание новой
                record = new EavRecord { EavEntityId = entityId };
                _context.EavRecords.Add(record);
                await _context.SaveChangesAsync(); // Сохраняем, чтобы получить ID
            }

            var formValues = Request.Form;
            foreach (var key in formValues.Keys)
            {
                if (key.StartsWith("values["))
                {
                    var attrIdStr = key.Replace("values[", "").Replace("]", "");
                    if (int.TryParse(attrIdStr, out int attrId))
                    {
                        var newVal = formValues[key].ToString();

                        // Ищем существующее значение
                        var existingValue = record.Values.FirstOrDefault(v => v.EavAttributeId == attrId);

                        if (existingValue != null)
                        {
                            existingValue.Value = newVal;
                            _context.EavValues.Update(existingValue);
                        }
                        else
                        {
                            _context.EavValues.Add(new EavValue
                            {
                                EavRecordId = record.Id,
                                EavAttributeId = attrId,
                                Value = newVal
                            });
                        }
                    }
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToPage(new { entityId });
        }

        public async Task<IActionResult> OnPostDeleteRecordAsync(int recordId, int entityId)
        {
            var record = await _context.EavRecords.FindAsync(recordId);
            if (record != null)
            {
                _context.EavRecords.Remove(record);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage(new { entityId });
        }
    }
}