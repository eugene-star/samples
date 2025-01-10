using System.ComponentModel.DataAnnotations.Schema;

namespace TaskAPI.Model
{
	public class File
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
		public int TaskId { get; set; }
		public string Name { get; set; } = string.Empty;
	}
}
