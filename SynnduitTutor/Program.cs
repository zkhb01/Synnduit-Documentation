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

// --- Curriculum (read-only authored content), one store per training course ---
builder.Services.AddSingleton<CourseCatalog>();

// --- Donut incentive config (appsettings: "Donuts") ---
var donutOptions = builder.Configuration.GetSection("Donuts").Get<DonutOptions>() ?? new DonutOptions();
builder.Services.AddSingleton(donutOptions);

// --- Engine ---
builder.Services.AddScoped<GatingEngine>();
builder.Services.AddScoped<ScoringService>();
builder.Services.AddScoped<MasteryService>();
builder.Services.AddScoped<DonutService>();

// --- Remediation: live Claude when an API key is configured, else the deterministic stub ---
builder.Services.AddScoped<StubRemediationService>();
var anthropicOptions = builder.Configuration.GetSection("Anthropic").Get<AnthropicOptions>() ?? new AnthropicOptions();
if (string.IsNullOrWhiteSpace(anthropicOptions.ApiKey))
    anthropicOptions.ApiKey = Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY");
builder.Services.AddScoped<StubAnswerGrader>();
if (!string.IsNullOrWhiteSpace(anthropicOptions.ApiKey))
{
    builder.Services.AddSingleton(anthropicOptions);
    builder.Services.AddSingleton(new Anthropic.AnthropicClient { ApiKey = anthropicOptions.ApiKey });
    builder.Services.AddScoped<IRemediationService, ClaudeRemediationService>();
    builder.Services.AddScoped<IAnswerGrader, ClaudeAnswerGrader>();
}
else
{
    builder.Services.AddScoped<IRemediationService>(sp => sp.GetRequiredService<StubRemediationService>());
    builder.Services.AddScoped<IAnswerGrader>(sp => sp.GetRequiredService<StubAnswerGrader>());
}

// --- Auth: current learner per circuit. Stub sign-in by default; Entra SSO when AzureAd is configured. ---
builder.Services.AddScoped<LearnerSession>();
builder.AddEntraAuth();

var app = builder.Build();

// Create the database on first run (no migrations needed for the core slice).
using (var scope = app.Services.CreateScope())
{
    var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<TutorDbContext>>();
    using var db = factory.CreateDbContext();
    db.Database.EnsureCreated();

    // EnsureCreated() never alters an existing table, so additive columns must be patched in by hand
    // (we have no migrations in this slice). Idempotently add Learner.IsAuditor to pre-existing DBs.
    var hasAuditor = db.Database
        .SqlQueryRaw<int>("SELECT COUNT(*) AS Value FROM pragma_table_info('Learners') WHERE name = 'IsAuditor'")
        .AsEnumerable().First() > 0;
    if (!hasAuditor)
        db.Database.ExecuteSqlRaw("ALTER TABLE Learners ADD COLUMN IsAuditor INTEGER NOT NULL DEFAULT 0");

    // Fail fast if any configured course's curriculum can't be located/parsed.
    var catalog = scope.ServiceProvider.GetRequiredService<CourseCatalog>();
    foreach (var course in catalog.Courses)
        _ = catalog.GetStore(course.Id)!.Graph;
}

// Hosting under a sub-path (e.g. behind SDM's IIS site at "/Tutor"). Set Hosting:PathBase to "/Tutor"
// in production; leave empty to serve at the site root (local dev). The <base href> in App.razor reads
// the same value so in-app (base-relative) links resolve correctly under the prefix.
var pathBase = (app.Configuration["Hosting:PathBase"] ?? "").Trim().TrimEnd('/');
if (!string.IsNullOrEmpty(pathBase))
    app.UsePathBase(pathBase);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseEntraAuth();   // no-op unless AzureAd is configured
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
