using FaceNoteBook.Extensions;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

Env.Load();

// Add services
builder.Services.AddControllers();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}


app.UseRouting();
app.MapControllers();

app.Run();
