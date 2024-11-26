using BigBrother.Domain.Interfaces.Providers;
using BigBrother.Domain.Interfaces.Repositories;
using BigBrother.Domain.Interfaces.Services;
using BigBrother.Domain.Providers;
using BigBrother.Domain.Services;
using BigBrother.Repository.Context.Factory;
using BigBrother.Repository.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var connectionString = "Host=localhost;Port=5432;Database=bb;Username=postgres;Password=postgres";
builder.Services.AddSingleton<IContextFactory, ContextFactory>(_ => new ContextFactory(connectionString));

builder.Services.AddSingleton<IGroupRepository, GroupRepository>();
builder.Services.AddSingleton<ISessionRepository, SessionRepository>();
builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IScoreRepository, ScoreRepository>();
builder.Services.AddSingleton<IActionRepository, ActionRepository>();

builder.Services.AddSingleton<IGroupProvider, GroupProvider>();
builder.Services.AddSingleton<ISessionProvider, SessionProvider>();
builder.Services.AddSingleton<IUserProvider, UserProvider>();
builder.Services.AddSingleton<IScoreProvider, ScoreProvider>();
builder.Services.AddSingleton<IActionProvider, ActionProvider>();

builder.Services.AddSingleton<IDetectionService, DetectionService>();
builder.Services.AddSingleton<IAnalysisService, AnalysisService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
