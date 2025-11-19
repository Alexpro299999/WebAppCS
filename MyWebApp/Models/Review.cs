using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWebApp.Models
{
	public class Review
	{
		[Key] 
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
		public int ReviewId { get; set; }

		[Required(ErrorMessage = "Клиент обязателен.")] 
		public int ClientId { get; set; }

		[Required(ErrorMessage = "Процедура обязательна.")] 
		public int ProcedureId { get; set; }

		[Required(ErrorMessage = "Оценка обязательна.")] 
		[Range(1, 5, ErrorMessage = "Оценка должна быть от 1 до 5.")] 
		public int Rating { get; set; }

		[StringLength(1000, ErrorMessage = "Комментарий не может быть длиннее 1000 символов.")] 
		public string? Comment { get; set; } 

		[ForeignKey("ClientId")] 
		public virtual Client Client { get; set; } = null!; 

		[ForeignKey("ProcedureId")] 
		public virtual Procedure Procedure { get; set; } = null!; 
	}
}