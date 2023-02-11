using AutoMapper;
using BigBrother.Helpers.MappingProfiles;
using BigBrother.Interfaces;
using BigBrother.Services;
using Microsoft.EntityFrameworkCore;
using Repository;
using Repository.Interfaces;
using Repository.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContextPool<AppDbContext>(o => 
    o.UseSqlite(builder.Configuration.GetConnectionString("Sqlite")));
builder.Services.AddSingleton<IExamService, ExamService>();
builder.Services.AddSingleton<IRepository, SqliteRepository>();

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();