using Microsoft.EntityFrameworkCore;

namespace TaskAPI.Repository
{
	public class TasksRepository : IReporitory<Model.Task>
	{
		private readonly TasksDbContext _context;

		public TasksRepository(TasksDbContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<Model.Task>> GetList(int first, int count)
		{
			return (await _context.Tasks.ToListAsync()).Skip(first).Take(count);
		}

		public async Task<Model.Task?> Read(int id)
		{
			return await _context.Tasks
				.Include(t => t.Files)
				.SingleOrDefaultAsync(t => t.Id == id);
		}

		public async Task<bool> Update(Model.Task task)
		{
			_context.Entry(task).State = EntityState.Modified;
			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!_context.Tasks.Any(e => e.Id == task.Id))
					return false;
				else
					throw;
			}

			return true;
		}

		public async Task<Model.Task> Create(Model.Task task)
		{
			_context.Tasks.Add(task);
			await _context.SaveChangesAsync();
			return task;
		}

		public async Task<bool> Delete(int id)
		{
			var task = await _context.Tasks.FindAsync(id);
			if (task is null)
				return false;
			_context.Tasks.Remove(task);
			await _context.SaveChangesAsync();
			return true;
		}
	}
}
