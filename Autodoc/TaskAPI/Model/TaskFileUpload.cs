namespace TaskAPI.Model
{
	public class TaskFileUpload
	{
		public int TaskId { get; set; }
		public required IFormFile UploadedFile { get; set; }
	}
}
