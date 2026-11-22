using DailyPlanner.AiCoach;
using DailyPlanner.Api.Auth;
using DailyPlanner.Finance;
using DailyPlanner.Life;
using DailyPlanner.Notify;
using DailyPlanner.Planner;
using DailyPlanner.SharedKernel.Events;
using DailyPlanner.SharedKernel.Time;

// Composition root only: shared services, auth, modules, middleware. No business logic here.
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IClock, SystemClock>();
builder.Services.AddScoped<IEventBus, InMemoryEventBus>();
builder.Services.AddDailyPlannerAuth(builder.Environment);

builder.Services
    .AddPlannerModule(builder.Configuration)
    .AddLifeModule(builder.Configuration)
    .AddFinanceModule(builder.Configuration)
    .AddNotifyModule(builder.Configuration)
    .AddAiCoachModule(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}

app.MapStaticAssets();

app.UseAuthentication();
app.UseAuthorization();

app.MapPlannerEndpoints();
app.MapLifeEndpoints();
app.MapFinanceEndpoints();
app.MapNotifyEndpoints();
app.MapAiCoachEndpoints();

// Unknown /api routes are 404s; everything else falls through to the Blazor client.
app.MapFallback("/api/{**path}", () => Results.NotFound());
app.MapFallbackToFile("index.html");

app.Run();

/// <summary>Exposed so integration tests can use WebApplicationFactory&lt;Program&gt;.</summary>
public partial class Program;
