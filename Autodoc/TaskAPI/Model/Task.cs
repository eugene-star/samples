using System.ComponentModel.DataAnnotations.Schema;

namespace TaskAPI.Model
{
	public class Task
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
		public DateTime Dt { get; set; } = DateTime.Now;
		public string Name { get; set; } = string.Empty;
		public string Status { get; set; } = string.Empty;

		public ICollection<File> Files { get; set; } = new List<File>();
	}
}