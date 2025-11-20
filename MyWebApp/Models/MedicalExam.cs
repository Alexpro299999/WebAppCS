using System;
using System.ComponentModel.DataAnnotations;

namespace MyWebApp.Models
{
    public class MedicalExam
    {
        public int Id { get; set; }

        [DataType(DataType.Date)]
        [Required]
        public DateTime Date { get; set; }

        public int ClientId { get; set; }
        public Client? Client { get; set; }

        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        public int ProcedureId { get; set; }
        public Procedure? Procedure { get; set; }

        [Required(ErrorMessage = "Укажите результат")]
        public string Result { get; set; }
    }
}