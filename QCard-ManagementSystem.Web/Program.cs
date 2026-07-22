using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using QCard.ManagementSystem;
using QCard.ManagementSystem.Domain;
using QCard.ManagementSystem.Infrastructure.Cache;
using QCard.ManagementSystem.Infrastructure.Helper;
using QCard.ManagementSystem.Infrastructure.Helper.Model;
using QCard.ManagementSystem.Infrastructure.Web.Memory;
using QCard.ManagementSystem.Service;
using QCard.ManagementSystem.Web.Filters;
using Rotativa.AspNetCore;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add configuration
var configuration = builder.Configuration;
var environment = builder.Environment;

// Initialize application-level cache
using (var db = new DbHelper())
{
    var config = db.ConfigRepository.FindAll();
    ApplicationCacheManager.Set(ApplicationCacheManager.AppConfigCacheKey, config.ToList());

    var rolePermission = db.RolePermissionRepository.FindAll();
    var menu = db.MenuRepository.FindAll();
    ApplicationCacheManager.Set(ApplicationCacheManager.PermissionCacheKey, rolePermission.ToList());
    ApplicationCacheManager.Set(ApplicationCacheManager.PermissionMenuCacheKey, menu.ToList());
}

// Services configuration
builder.Services.AddRazorPages();
    //.AddRazorRuntimeCompilation();
builder.Services.AddControllersWithViews();

builder.Services.AddMvc(o => o.Filters.Add<ExceptionFilter>())
    .AddJsonOptions(opts => opts.JsonSerializerOptions.PropertyNamingPolicy = null)
    .SetCompatibilityVersion(CompatibilityVersion.Latest)
    .AddRazorPagesOptions(options =>
    {
        options.Conventions.AuthorizeAreaFolder("Identity", "/Core/Account/Login");
        options.Conventions.AuthorizeAreaPage("Identity", "/Core/Account/Logout");
    });

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => false;
    options.MinimumSameSitePolicy = SameSiteMode.None;
    options.HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always;
    options.Secure = builder.Environment.IsDevelopment()
        ? CookieSecurePolicy.SameAsRequest
        : CookieSecurePolicy.Always;
});

builder.Services.Configure<FormOptions>(options =>
{
    options.ValueCountLimit = int.MaxValue;
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartHeadersLengthLimit = int.MaxValue;
    options.MultipartBodyLengthLimit = int.MaxValue;
});

builder.Services.AddAntiforgery(options =>
{
    options.FormFieldName = "__RequestVerificationToken";
    options.HeaderName = "X-CSRF-TOKEN";
});

builder.Services.Configure<SmtpSettings>(
    builder.Configuration.GetSection("Smtp"));

builder.Services.AddScoped<IEmailService, SmtpEmailService>();

builder.Services.AddMemoryCache();
//builder.Services.AddDistributedMemoryCache();  // Menambahkan Distributed Memory Cache untuk sesi

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Sesuaikan dengan waktu yang diinginkan
    options.Cookie.Name = ".QCard_ManagementSystem.Session";
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
        ? CookieSecurePolicy.SameAsRequest
        : CookieSecurePolicy.Always;
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
builder.Services.AddSingleton<IUrlHelperFactory, UrlHelperFactory>();
builder.Services.AddScoped(it => it.GetService<IUrlHelperFactory>().GetUrlHelper(it.GetService<IActionContextAccessor>().ActionContext));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "QCard.ManagementSystem" + (builder.Environment.IsDevelopment() ? ".DEV" : "");
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.IsEssential = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(29); // Sesuaikan dengan waktu sesi
        options.SlidingExpiration = true;
        options.LoginPath = "/core/account/login";  // Sesuaikan dengan path login Anda
        options.LogoutPath = "/core/account/logout";  // Sesuaikan dengan path logout Anda
    });

builder.Services.AddKendo();

builder.Services.AddScoped<IDbHelper>(_ => new DbHelper());
builder.Services.Scan(scanner => scanner.FromAssemblyOf<DbService>()
    .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")))
    .AsSelf()
    .WithTransientLifetime());

builder.Services.AddDataProtection()
    .SetApplicationName("QCard.ManagementSystem");
    //.DisableAutomaticKeyGeneration();

#if DEBUG
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = int.MaxValue;
});
#else
builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = int.MaxValue;
});
#endif

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error?statusCode=500");
}

app.UseStatusCodePagesWithReExecute("/Error/{0}");
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseCookiePolicy();
app.UseSession(); // Pastikan session digunakan dalam pipeline
Agit.Framework.Web.HttpContext.Services = app.Services;
app.UseHttpContext();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

RotativaConfiguration.Setup(Path.Combine(app.Environment.WebRootPath));
app.Run();

