using Hangfire;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Hosting.WindowsServices;
using QCard.ManagementSystem.Domain;
using QCard.ManagementSystem.Hangfire.Extension;
using QCard.ManagementSystem.Hangfire.Filters;
using QCard.ManagementSystem.Hangfire.Schedulers;
using QCard.ManagementSystem.Infrastructure.Helper;
using QCard.ManagementSystem.Infrastructure.Helper.Model;
using System.Globalization;


var option = new WebApplicationOptions
{
    Args = args,
    ContentRootPath = WindowsServiceHelpers.IsWindowsService() ? AppContext.BaseDirectory : default,
    ApplicationName = System.Diagnostics.Process.GetCurrentProcess().ProcessName
};

var builder = WebApplication.CreateBuilder(option);

// Set default culture
var defaultCulture = new CultureInfo("id-ID"); //get value from appsettings.json
CultureInfo.DefaultThreadCurrentCulture = defaultCulture;
CultureInfo.DefaultThreadCurrentUICulture = defaultCulture;

//use windows service as a host
builder.Host.UseWindowsService();

var services = builder.Services;

IServiceProvider serviceProvider = services.BuildServiceProvider();
IWebHostEnvironment env = serviceProvider.GetService<IWebHostEnvironment>();
// Enable runtime compilation
services.AddRazorPages();
services.ConfigureHangfire();

services.AddHangfireServer(option =>
{
    option.SchedulePollingInterval = TimeSpan.FromSeconds(1);
});

services.Configure<FormOptions>(options =>
{
    options.ValueCountLimit = int.MaxValue;
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartHeadersLengthLimit = int.MaxValue;
});

//Register Memory Cache
services.AddMemoryCache();

//Register BackgroundJob
services.AddTransient<IHangfireJob, HangfireJob>();
services.AddScoped<IDbHelper>(options =>
{
    return new DbHelper();
});
services.Configure<SmtpSettings>(
    builder.Configuration.GetSection("Smtp")
);
services.AddScoped<IEmailService, SmtpEmailService>();

//Add service for accessing current HttpContext
services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
services.AddSingleton<IUrlHelperFactory, UrlHelperFactory>();
services.AddScoped(it => it.GetService<IUrlHelperFactory>()
                 .GetUrlHelper(it.GetService<IActionContextAccessor>().ActionContext));

services.Configure<IISServerOptions>(options =>
    options.MaxRequestBodySize = long.MaxValue
);

//IApplicationBuilder app = builder.Build();

var app = builder.Build();
//Register BackgroundJob
app.Services.GetService<IHangfireJob>().RegisterSchedule();

if (env.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else app.UseExceptionHandler("/Error?statusCode=500");

app.UseHangfireDashboard("", new DashboardOptions()
{
    Authorization = new[] { new HangFireAuthorizationFilter() }

});

//RecurringJob.AddOrUpdate(() => Worker.)
app.UseStatusCodePagesWithReExecute("/Error/{0}");
app.UseHttpsRedirection();
app.UseRouting();
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = context =>
    {

        context.Context.Response.Headers.Add("cache-control", new[] { "public,max-age=31536000" });
        context.Context.Response.Headers.Add("Expires", new[] { DateTime.UtcNow.AddYears(1).ToString("R") }); // Format RFC1123

    }
});

//Hangfire
app.UseHangfireDashboard("/Dashboard", new DashboardOptions
{
    DashboardTitle = "LIUGONG QCard -  Background Job"
});

app.UseHsts();
app.MapControllers();

//app.Urls.Add("http://localhost:2552");
app.Run();

////for IIS Hosting============================================================================================================================================
//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//var services = builder.Services;

//services.AddControllers();

//services.AddEndpointsApiExplorer();
//services.AddSwaggerGen(option =>
//{
//    option.SwaggerDoc("v1", new OpenApiInfo
//    {
//        Title = "LIUGONG QCard Hangfire API",
//        Version = "v1",
//        Description = "Trigger Backgorund Job using API Endpoints"
//    });
//    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//    {
//        In = ParameterLocation.Header,
//        Description = "Token",
//        Name = "Authorization",
//        Type = SecuritySchemeType.Http,
//        BearerFormat = "JWT",
//        Scheme = "Bearer"
//    });
//    option.AddSecurityRequirement(new OpenApiSecurityRequirement
//    {
//        {
//            new OpenApiSecurityScheme
//            {
//                Reference = new OpenApiReference
//                {
//                    Type=ReferenceType.SecurityScheme,
//                    Id="Bearer"
//                }
//            },
//            Array.Empty<string>()
//        }
//    });
//});

////register job service
//services.AddTransient<IHangfireJob, HangfireJob>();

////Add service for accessing current HttpContext
//services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
//services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
//services.AddSingleton<IUrlHelperFactory, UrlHelperFactory>();
//services.AddScoped(it => it.GetService<IUrlHelperFactory>()
//                 .GetUrlHelper(it.GetService<IActionContextAccessor>().ActionContext));

//services.AddScoped<IDbHelper>(options =>
//{
//    return new DbHelper();
//});

//services.ConfigureHangfire();
//services.AddHangfireServer(option =>
//{
//    option.SchedulePollingInterval = TimeSpan.FromSeconds(1);
//});

//services.AddHttpContextAccessor();

//var configBuilder = new ConfigurationBuilder()
//    .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
//    .AddJsonFile("appsettings.json");
//var configuration = configBuilder.Build();

//services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
//}).AddJwtBearer(options =>
//{
//    options.TokenValidationParameters = new TokenValidationParameters()
//    {
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidAudience = configuration["JwtSettings:Audience"],
//        ValidIssuer = configuration["JwtSettings:Issuer"],
//        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:SecretKey"]))
//    };
//});

//var app = builder.Build();

//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

//app.UseSwagger();
//app.UseSwaggerUI();

////register job
//app.Services.GetService<IHangfireJob>().RegisterSchedule();

////Hangfire
//app.UseHangfireDashboard("/Dashboard", new DashboardOptions
//{
//    DashboardTitle = "LIUGONG QCard -  Background Job"
//});

//app.UseHttpsRedirection();
//app.UseStaticFiles();

//app.UseRouting();

//app.UseAuthorization();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");

//app.Run();
