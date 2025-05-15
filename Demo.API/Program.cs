using Demo.DAL;
using Demo.DAL.Repositories.Implementations;
using Demo.DAL.Repositories.Interfaces;
using Demo.Hubs;
using Demo.Kafka;
using Demo.Managers.Implementations;
using Demo.Managers.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services.AddControllers();
builder.Services.AddDbContext<DemoDbContext>(options =>
    options.UseNpgsql(Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? 
        "Host=150.241.88.0;Port=5433;Database=db_task;Username=username_db;Password=password"));

builder.Services.Configure<KafkaProducerConfig>(builder.Configuration.GetSection("Kafka"));
builder.Services.AddSingleton<IKafkaProducerService, KafkaProducerService>();

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

var mySignalRCorsPolicy = "MySignalRCorsPolicy";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: mySignalRCorsPolicy,
        policy =>
        {
            // ВАЖНО: Это разрешит любой origin, который делает запрос,
            // но только если AllowCredentials() используется, браузер все равно
            // будет требовать точное совпадение origin'а в ответе.
            // Эта функция будет вызвана для каждого запроса с Origin.
            policy.SetIsOriginAllowed(origin => true) // Разрешает любой origin
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials(); // <--- ВАЖНО для SignalR
        });
});

var app = builder.Build();

app.UseCors("AllowAll");

app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();
app.MapControllers();

app.UseCors(mySignalRCorsPolicy);
app.MapHub<TaskNotificationHub>("/taskNotificationHub");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DemoDbContext>();
    await db.Database.MigrateAsync();
}

app.Run();
