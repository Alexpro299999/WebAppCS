using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWebApp.Models
{
    public class EavEntity
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public List<EavAttribute> Attributes { get; set; } = new List<EavAttribute>();
        public List<EavRecord> Records { get; set; } = new List<EavRecord>();
    }

    public class EavAttribute
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string DataType { get; set; } = "string";
        public int? LinkedEntityId { get; set; }
        public int EavEntityId { get; set; }
        public EavEntity EavEntity { get; set; }
    }

    public class EavRecord
    {
        public int Id { get; set; }
        public int EavEntityId { get; set; }
        public EavEntity EavEntity { get; set; }
        public List<EavValue> Values { get; set; } = new List<EavValue>();
    }

    public class EavValue
    {
        public int Id { get; set; }
        public string? Value { get; set; }
        public int EavRecordId { get; set; }
        public EavRecord EavRecord { get; set; }
        public int EavAttributeId { get; set; }
        public EavAttribute EavAttribute { get; set; }
    }
}