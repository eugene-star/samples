using Microsoft.EntityFrameworkCore;
using TaskAPI.Model;

namespace TaskAPI.Repository
{
	public class TasksDbContext : DbContext
	{
		public DbSet<WorkTask> Tasks { get; set; }


		public TasksDbContext(DbContextOptions<TasksDbContext> options) : base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<WorkTask>().ToTable("Task");
		}
	}
}
