using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using TaskAPI.Model;
using TaskAPI.Repository;

namespace TaskAPI.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class FilesController : ControllerBase
	{
		readonly ILogger<FilesController> _logger;
		readonly FilesRepository _filesRepo;

		public FilesController(FilesRepository filesRepo, ILogger<FilesController> logger)
		{
			_filesRepo = filesRepo;
			_logger = logger;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<Model.File>>> Get(int first = 0, int count = 10)
		{
			return new (await _filesRepo.GetList(first, count));
		}

		[HttpGet("{id}")]
		public async Task<ActionResult> GetFile(int id)
		{
			var file = await _filesRepo.Read(id);
			if (file is null)
				return NotFound();
			else
				return PhysicalFile(Directory.GetCurrentDirectory() + "\\Files\\" + file.Name, "application/octet-stream", file.Name);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> PutFile(int id, Model.File file)
		{
			if (id != file.Id)
				return BadRequest();

			if(await _filesRepo.Update(id, file))
				return NoContent();
			else
				return NotFound();
		}

		[HttpPost, DisableRequestSizeLimit]
		public async Task<ActionResult> PostFile([FromForm] TaskFileUpload taskFileUpload)
		{
			var file = new Model.File() { TaskId = taskFileUpload.TaskId, Name = taskFileUpload.UploadedFile.FileName };
			await _filesRepo.Create(file);
			await using var fileStream = new FileStream("Files\\" + taskFileUpload.UploadedFile.FileName, FileMode.Create);
			await taskFileUpload.UploadedFile.CopyToAsync(fileStream);
			return Ok();
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult<Model.File>> DeleteFile(int id)
		{
			if (await _filesRepo.Delete(id))
				return Ok();
			else
				return NotFound();
		}
	}
}
