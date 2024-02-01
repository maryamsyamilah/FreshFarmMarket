using System.ComponentModel.DataAnnotations;
namespace FreshFarmMarket.Model
{
	public class AuditLog
	{
		public int Id { get; set; }
		[Required]
		public string UserEmail { get; set; }
		[Required]
		public string EntityName { get; set; }
		[Required]
		public string Action { get; set; }
		[Required]
		public DateTime Timestamp { get; set;}

		public string Changes { get; set; }
	}
}
