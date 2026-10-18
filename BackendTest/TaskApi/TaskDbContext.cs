using Microsoft.EntityFrameworkCore;

namespace TaskApi;

public class TaskDbContext(DbContextOptions<TaskDbContext> options) : DbContext(options)
{
    public DbSet<TaskItem> TaskItems => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TaskItem>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(200);

            b.Property(x => x.RowVersion)
                .IsConcurrencyToken()
                .HasColumnType("bytea");
        });
    }
}
