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

        public EavEntity CurrentEntity { get; set; }
        public List<EavRecord> Records { get; set; }

        // Dictionary<EntityId, Dictionary<RecordId, DisplayText>>
        public Dictionary<int, Dictionary<int, string>> LookupValues { get; set; } = new();

        public async Task OnGetAsync(int entityId)
        {
            CurrentEntity = await _context.EavEntities
                .Include(e => e.Attributes)
                .FirstOrDefaultAsync(e => e.Id == entityId);

            if (CurrentEntity != null)
            {
                Records = await _context.EavRecords
                    .Where(r => r.EavEntityId == entityId)
                    .Include(r => r.Values)
                    .ToListAsync();

                // Load lookups for relation attributes
                var relationAttrs = CurrentEntity.Attributes.Where(a => a.DataType == "relation" && a.LinkedEntityId.HasValue).ToList();
                foreach (var attr in relationAttrs)
                {
                    await LoadLookupData(attr.LinkedEntityId.Value);
                }
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
                // Try to find a string attribute to use as name, otherwise use ID
                var nameVal = rec.Values.FirstOrDefault(v => v.EavAttribute.DataType == "string")?.Value;
                dict[rec.Id] = string.IsNullOrEmpty(nameVal) ? $"Запись #{rec.Id}" : nameVal;
            }
            LookupValues[linkedEntityId] = dict;
        }

        public async Task<IActionResult> OnPostSaveRecordAsync(int entityId)
        {
            var newRecord = new EavRecord { EavEntityId = entityId };
            _context.EavRecords.Add(newRecord);
            await _context.SaveChangesAsync();

            var formValues = Request.Form;
            foreach (var key in formValues.Keys)
            {
                if (key.StartsWith("values["))
                {
                    var attrIdStr = key.Replace("values[", "").Replace("]", "");
                    if (int.TryParse(attrIdStr, out int attrId))
                    {
                        var val = formValues[key].ToString();
                        _context.EavValues.Add(new EavValue
                        {
                            EavRecordId = newRecord.Id,
                            EavAttributeId = attrId,
                            Value = val
                        });
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