using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyWebApp.Models
{
    public class Procedure
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Укажите название услуги")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Укажите цену")]
        [Range(0, 100000, ErrorMessage = "Цена должна быть положительной")]
        public decimal Price { get; set; }

        public ICollection<Review>? Reviews { get; set; }
    }
}