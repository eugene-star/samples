using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using TaskAPI.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
	.Services.AddOpenApi()
	.AddDbContext<TasksDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("TasksDbConnectionString")))
	.AddTransient<TasksRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
	app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
