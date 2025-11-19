using System.ComponentModel.DataAnnotations;

namespace MyWebApp.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Укажите ФИО")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Укажите должность")]
        public string Position { get; set; }
    }
}