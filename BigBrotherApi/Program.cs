using AutoMapper;
using BigBrother.Helpers.MappingProfiles;
using BigBrother.Interfaces;
using BigBrother.Middlewares;
using BigBrother.Services;
using Entities.Domain;
using NLog.Web;
using Repository.Interfaces;
using Repository.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Host.UseNLog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var confBuilder = new ConfigurationBuilder()
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appSettings.json")
    .AddEnvironmentVariables();
var configuration = confBuilder.Build();

builder.Services.Configure<DbConnectionOptions>(configuration.GetSection("DbConnectionOptions"));

builder.Services.AddTransient<ILogFileParserService, LogFileParserService>();

builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IExamRepository, ExamRepository>();
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<IExamService, ExamService>();
builder.Services.AddSingleton<IGenerationFromScratchDetectionService, GenerationFromScratchDetectionService>();
builder.Services.AddSingleton<IDetectionService, DetectionService>();

var mappingConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new ExamMappingProfile());
    mc.AddProfile(new UserMappingProfile());
});
builder.Services.AddSingleton(mappingConfig.CreateMapper());

var app = builder.Build();

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
