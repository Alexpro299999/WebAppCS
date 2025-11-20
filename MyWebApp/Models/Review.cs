using System;
using System.ComponentModel.DataAnnotations;

namespace MyWebApp.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required]
        public int ClientId { get; set; }
        public Client? Client { get; set; }

        [Required]
        public int ProcedureId { get; set; }
        public Procedure? Procedure { get; set; }

        [Range(1, 5, ErrorMessage = "Оценка должна быть от 1 до 5")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "Напишите комментарий")]
        public string Comment { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;
    }
}