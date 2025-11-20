using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyWebApp.Models
{
    public class Client
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Укажите фамилию")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Укажите имя")]
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string Fio => $"{LastName} {FirstName} {MiddleName}".Trim();

        [Phone(ErrorMessage = "Некорректный номер")]
        public string Phone { get; set; }

        [EmailAddress(ErrorMessage = "Некорректный email")]
        public string Email { get; set; }

        public ICollection<Review>? Reviews { get; set; }
    }
}