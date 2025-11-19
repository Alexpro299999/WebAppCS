using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWebApp.Models
{
    public class Client
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ClientId { get; set; }

        [Required]
        [StringLength(200)]
        public string Fio { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;
        [Column(TypeName = "varbinary(max)")]
        public byte[]? Photo { get; set; }

        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}