namespace TaskAPI.Model
{
	public class WorkTask
	{
		public int Id { get; set; }
		public DateTime Dt { get; set; } = DateTime.Now;
		public string Name { get; set; }
		public string Status { get; set; }
	}
}