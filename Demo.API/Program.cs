using Demo.DAL;
using Demo.DAL.Repositories.Implementations;
using Demo.DAL.Repositories.Interfaces;
using Demo.Managers.Implementations;
using Demo.Managers.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();
builder.Services.AddDbContext<DemoDbContext>(options =>
    options.UseNpgsql(Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? 
        "Host=localhost;Port=5433;Database=db_task;Username=username_db;Password=password"));

builder.Services.AddScoped<IProjectTaskRepository, PostgresProjectTaskRepository>();
builder.Services.AddScoped<IWorkerRepository, PostgresWorkerRepository>();

builder.Services.AddScoped<IProjectTaskManager, ProjectTaskManager>();
builder.Services.AddScoped<IWorkerManager, WorkerManager>();

//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowFrontend", policy =>
//    {
//        policy.WithOrigins("https://demo.internship.visiflow-ai.ru")
//            .AllowAnyHeader()
//            .AllowAnyMethod();
//    });
//});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin() 
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("AllowAll");

app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DemoDbContext>();
    await db.Database.MigrateAsync();
}

app.Run();
