using Microsoft.AspNetCore.Mvc;
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
			{
				_logger.LogError($"File id={id} not found");
				return NotFound();
			}
			else
				return PhysicalFile(Directory.GetCurrentDirectory() + "\\Files\\" + file.Name, "application/octet-stream", file.Name);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> PutFile(Model.File file)
		{
			if(await _filesRepo.Update(file))
				return Ok();
			else
			{
				_logger.LogError($"Can't update file id={file.Id} name={file.Name}");
				return NotFound();
			}
		}

		[HttpPost, DisableRequestSizeLimit]
		public async Task<ActionResult> PostFile([FromForm] TaskFileUpload taskFileUpload)
		{
			var file = new Model.File() { TaskId = taskFileUpload.TaskId, Name = taskFileUpload.UploadedFile.FileName };
			await _filesRepo.Create(file);
			await using var fileStream = new FileStream("Files\\" + taskFileUpload.UploadedFile.FileName, FileMode.Create);
			await taskFileUpload.UploadedFile.CopyToAsync(fileStream);
			return CreatedAtAction("PostFile", file);
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult<Model.File>> DeleteFile(int id)
		{
			if (await _filesRepo.Delete(id))
				return Ok();
			else
			{
				_logger.LogError($"File id={id} not found");
				return NotFound();
			}
		}
	}
}
