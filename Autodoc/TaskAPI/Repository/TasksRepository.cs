using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskAPI.Model;

namespace TaskAPI.Repository
{
	public class TasksRepository
	{
		private readonly TasksDbContext _context;

		public TasksRepository(TasksDbContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<WorkTask>> GetList(int first, int count)
		{
			return (await _context.Tasks.ToListAsync()).Skip(first).Take(count);
		}

		public async Task<WorkTask> Read(int id)
		{
			return await _context.Tasks.FindAsync(id);
		}

		public async Task<bool> Update(int id, WorkTask task)
		{
			_context.Entry(task).State = EntityState.Modified;
			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!_context.Tasks.Any(e => e.Id == id))
					return false;
				else
					throw;
			}

			return true;
		}

		public async Task<WorkTask> Create(WorkTask task)
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
