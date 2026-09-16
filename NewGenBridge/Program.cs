using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NewGenBridge.Data;
using NewGenBridge.Middleware;
using NewGenBridge.Models;
using NewGenBridge.Services;
using NewGenBridge.UoW;
using QuestPDF.Infrastructure;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
QuestPDF.Settings.License = LicenseType.Community;
// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddIdentity<NPUser, IdentityRole>(options =>
    { options.SignIn.RequireConfirmedAccount = false; })
    .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders(); ;
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 5;
    options.Password.RequireUppercase = false;

});

builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();

builder.Services.AddScoped<ITranslationService, TranslationService>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.None;
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Home/Privacy";
});

builder.Services.AddRazorPages();

builder.Services.AddScoped<UnitOfWork>();
builder.Services.AddTransient<AccountService>();
builder.Services.AddTransient<AttendanceService>();
builder.Services.AddTransient<BranchService>();
builder.Services.AddTransient<LevelService>();
builder.Services.AddTransient<LessonService>();
builder.Services.AddTransient<PaymentService>();
builder.Services.AddTransient<StudentService>();
builder.Services.AddTransient<UserService>();
builder.Services.AddTransient<SimpleLoggerService>();
builder.Services.AddHostedService<MidResetService>();
builder.Services.AddTransient<ContractService>();
builder.Services.AddTransient<HubService>();

builder.Services.AddControllersWithViews();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("DsotcOnly", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim(ClaimTypes.Name, "dsotc");
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseMiddleware<Microsoft.AspNetCore.Localization.RequestLocalizationMiddleware>();
app.UseMiddleware<ApiKeyMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.MapRazorPages();

app.Run();
