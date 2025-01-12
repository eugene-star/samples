using Microsoft.EntityFrameworkCore;

namespace TaskAPI.Repository
{
	public class TasksDbContext : DbContext
	{
		public DbSet<Model.Task> Tasks { get; set; }
		public DbSet<Model.File> Files { get; set; }

		public TasksDbContext(DbContextOptions<TasksDbContext> options) : base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Model.Task>().ToTable("Task");
			modelBuilder.Entity<Model.File>().ToTable("File");
		}
	}
}