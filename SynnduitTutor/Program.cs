using Microsoft.EntityFrameworkCore;
using SynnduitTutor.Components;
using SynnduitTutor.Data;
using SynnduitTutor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// --- Persistence: SQLite mastery store (zero-setup, file-based) ---
var dbPath = builder.Configuration["Database:Path"]
             ?? Path.Combine(builder.Environment.ContentRootPath, "App_Data", "tutor.db");
Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
builder.Services.AddDbContextFactory<TutorDbContext>(o => o.UseSqlite($"Data Source={dbPath}"));

// --- Curriculum (read-only authored content) ---
builder.Services.AddSingleton<CurriculumStore>();

// --- Engine ---
builder.Services.AddScoped<GatingEngine>();
builder.Services.AddScoped<ScoringService>();
builder.Services.AddScoped<MasteryService>();
builder.Services.AddScoped<IRemediationService, StubRemediationService>();

// --- Stub auth: current learner per circuit ---
builder.Services.AddScoped<LearnerSession>();

var app = builder.Build();

// Create the database on first run (no migrations needed for the core slice).
using (var scope = app.Services.CreateScope())
{
    var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<TutorDbContext>>();
    using var db = factory.CreateDbContext();
    db.Database.EnsureCreated();

    // Fail fast if the curriculum can't be located/parsed.
    _ = scope.ServiceProvider.GetRequiredService<CurriculumStore>().Graph;
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
