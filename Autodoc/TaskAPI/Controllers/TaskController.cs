using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskAPI.Model;
using TaskAPI.Repository;

namespace TaskAPI.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class TasksController : ControllerBase
	{
		readonly ILogger<TasksController> _logger;
		readonly TasksRepository _tasksRepo;

		public TasksController(TasksRepository tasksRepo, ILogger<TasksController> logger)
		{
			_tasksRepo = tasksRepo;
			_logger = logger;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<Model.Task>>> Get(int first = 0, int count = 10)
		{
			return new (await _tasksRepo.GetList(first, count));
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<Model.Task>> GetTask(int id)
		{
			var task = await _tasksRepo.Read(id);
			if (task is null)
				return NotFound();
			return task;
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> PutTask(int id, Model.Task task)
		{
			if (id != task.Id)
				return BadRequest();

			if(await _tasksRepo.Update(id, task))
				return NoContent();
			else
				return NotFound();
		}

		[HttpPost]
		public async Task<ActionResult<Model.Task>> PostTask(Model.Task task)
		{
			await _tasksRepo.Create(task);
			return CreatedAtAction("PostTask", new { id = task.Id }, task);
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult<Model.Task>> DeleteTask(int id)
		{
			if (await _tasksRepo.Delete(id))
				return NoContent();
			else
				return NotFound();
		}
	}
}
