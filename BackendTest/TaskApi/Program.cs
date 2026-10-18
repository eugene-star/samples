using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using TaskApi;

var builder = WebApplication.CreateBuilder(args);

var connKey = "Postgres";
var connStr = builder.Configuration.GetConnectionString(connKey);
if (string.IsNullOrWhiteSpace(connStr))
    throw new ApplicationException($"Не определена строка подключения для '{connKey}'.");

builder.Services.AddDbContext<TaskDbContext>(opts => opts.UseNpgsql(connStr));

var rabbitConnStr = builder.Configuration.GetConnectionString("RabbitMq");
if (string.IsNullOrWhiteSpace(rabbitConnStr))
    throw new ApplicationException("Не определена строка подключения RabbitMq");

var factory = new ConnectionFactory 
{
    Uri = new Uri(rabbitConnStr),
    AutomaticRecoveryEnabled = true,
    NetworkRecoveryInterval = TimeSpan.FromSeconds(5)
};
builder.Services.AddSingleton(factory);
builder.Services.AddSingleton<IRabbitService, RabbitService>();

#if USE_SWAGGER
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Task API",
        Version = "v1"
    });
});
#endif

var app = builder.Build();

#if USE_SWAGGER
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
#endif

using (var scope = app.Services.CreateScope())
{
    var rabbitService = scope.ServiceProvider.GetRequiredService<IRabbitService>();
    await rabbitService.InitializeAsync();
    app.Lifetime.ApplicationStopping.Register(async () => await rabbitService.ShutdownAsync());
}

#region Контроллеры
app.MapPost("/tasks", async (TaskItemDto dto, TaskDbContext db, CancellationToken ct) =>
{
    if (string.IsNullOrWhiteSpace(dto.Title) || dto.Title.Length > 200)
        return Results.BadRequest(new { error = "Title должен быть не null, не пустой, максимум 200" });

    var task = new TaskItem
    {
        Id = Guid.NewGuid(),
        Title = dto.Title,
        IsCompleted = false,
        CreatedAt = DateTimeOffset.UtcNow,
        CompletedAt = null,
        Priority = dto.Priority ?? Priority.Medium
    };

    await db.TaskItems.AddAsync(task, ct);
    await db.SaveChangesAsync(ct);

    return Results.Created($"/tasks/{task.Id}", task);
});

app.MapGet("/tasks", async (TaskDbContext db) => await db.TaskItems.ToListAsync());

app.MapPut("/tasks/{id}/complete", async (Guid id, TaskDbContext db, IRabbitService publisher, CancellationToken ct) =>
{
    var task = await db.TaskItems.FirstOrDefaultAsync(x => x.Id == id, ct);
    if (task is null)
        return Results.NotFound(new { error = "Нет такой задачи" });
    if (task.IsCompleted)
        return Results.Conflict(new { error = "Задача уже завершена" });

    task.IsCompleted = true;
    task.CompletedAt = DateTimeOffset.UtcNow;

    try
    {
        await db.SaveChangesAsync(ct);
    }
    catch (DbUpdateConcurrencyException)
    {
        return Results.Conflict(new { error = "Конфликт завершения задачи" });
    }

    await publisher.PublishTaskCompletedEventAsync(task, ct);

    return Results.Ok(task);
});

app.MapDelete("/tasks/{id}", async (Guid id, TaskDbContext db, CancellationToken ct) =>
{
    var task = await db.TaskItems.FindAsync([id], ct);
    if (task is null)
        return Results.NotFound();

    db.TaskItems.Remove(task);
    await db.SaveChangesAsync(ct);
    return Results.NoContent();
});
#endregion

app.Run();