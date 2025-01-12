using Microsoft.AspNetCore.Mvc;
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
			{
				_logger.LogError($"Task id={id} not found");
				return NotFound();
			}
			else
				return task;
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> PutTask(Model.Task task)
		{
			if(await _tasksRepo.Update(task))
				return Ok();
			else
			{
				_logger.LogError($"Task id={task.Id} not found");
				return NotFound();
			}
		}

		[HttpPost]
		public async Task<ActionResult<Model.Task>> PostTask(Model.Task task)
		{
			await _tasksRepo.Create(task);
			return CreatedAtAction("PostTask", task);
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult<Model.Task>> DeleteTask(int id)
		{
			if (await _tasksRepo.Delete(id))
				return Ok();
			else
			{
				_logger.LogError($"Task id={id} not found");
				return NotFound();
			}
		}
	}
}
