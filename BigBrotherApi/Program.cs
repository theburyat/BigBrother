using AutoMapper;
using BigBrother.Helpers.MappingProfiles;
using BigBrother.Interfaces;
using BigBrother.Middlewares;
using BigBrother.Services;
using NLog.Web;
using Repository.Interfaces;
using Repository.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Logging.ClearProviders();
builder.Host.UseNLog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IRepository, SqliteRepository>();
builder.Services.AddSingleton<IExamService, ExamService>();

var mappingConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new ExamMappingProfile());
    mc.AddProfile(new UserMappingProfile());
});
builder.Services.AddSingleton(mappingConfig.CreateMapper());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<BbExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();