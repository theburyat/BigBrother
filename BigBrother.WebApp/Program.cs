using BigBrother.Domain.Interfaces.Providers;
using BigBrother.Domain.Interfaces.Repositories;
using BigBrother.Domain.Interfaces.Services;
using BigBrother.Domain.Providers;
using BigBrother.Domain.Services;
using BigBrother.Middlewares;
using BigBrother.Repository.Context.Factory;
using BigBrother.Repository.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRazorPages();
builder.Services.AddControllers();

var conf = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory).AddJsonFile("appsettings.json").Build();
builder.Services.AddSingleton<IContextFactory, ContextFactory>(_ => new ContextFactory(conf.GetConnectionString("DbConnection") ?? string.Empty));

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

builder.Services.AddSingleton<IConnectionService, ConnectionService>();
builder.Services.AddSingleton<IDetectionService, DetectionService>();
builder.Services.AddSingleton<IAnalysisService, AnalysisService>();

builder.Services.AddSingleton<InitializeService>();

var app = builder.Build();

app.UseExceptionMiddleware();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();
