using RenewalReminder.Data;
using RenewalReminder.Services;
using Microsoft.EntityFrameworkCore;
using RenewalReminder.Services.Abstract;
using RenewalReminder.Services.Concrete;
using RenewalReminder.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});

builder.Services.AddDbContext<DbContext, AppDbContext>(opt =>
{
    if (!string.IsNullOrEmpty(builder.Configuration.GetConnectionString("Mssql")))
    {
        opt.UseSqlServer(builder.Configuration.GetConnectionString("Mssql")).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<IUserAccessor, UserAccessor>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddSingleton<IValidationService, ValidationService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ICentralService, CentralService>();



// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(typeof(SessionFilterAttribute));

}).AddViewOptions(options =>
{
    options.HtmlHelperOptions.ClientValidationEnabled = false;
});

var app = builder.Build();

var trCulture = System.Globalization.CultureInfo.GetCultureInfo("tr-TR");
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture(trCulture),
    SupportedCultures = new List<System.Globalization.CultureInfo> { trCulture },
    SupportedUICultures = new List<System.Globalization.CultureInfo> { trCulture }
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();


app.UseSession();
app.UseCookiePolicy(new CookiePolicyOptions()
{
    Secure = CookieSecurePolicy.SameAsRequest
});

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

