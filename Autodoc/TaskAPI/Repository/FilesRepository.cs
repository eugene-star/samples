using Microsoft.EntityFrameworkCore;
using TaskAPI.Model;

namespace TaskAPI.Repository
{
	public class FilesRepository : IReporitory<Model.File>
	{
		private readonly TasksDbContext _context;

		public FilesRepository(TasksDbContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<Model.File>> GetList(int first, int count)
		{
			return (await _context.Files.ToListAsync()).Skip(first).Take(count);
		}

		public async Task<Model.File?> Read(int id)
		{
			return await _context.Files.FindAsync(id);
		}

		public async Task<bool> Update(int id, Model.File file)
		{
			_context.Entry(file).State = EntityState.Modified;
			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!_context.Files.Any(e => e.Id == id))
					return false;
				else
					throw;
			}

			return true;
		}

		public async Task<Model.File> Create(Model.File file)
		{
			_context.Files.Add(file);
			await _context.SaveChangesAsync();
			return file;
		}

		public async Task<bool> Delete(int id)
		{
			var file = await _context.Files.FindAsync(id);
			if (file is null)
				return false;
			_context.Files.Remove(file);
			await _context.SaveChangesAsync();
			return true;
		}
	}
}
