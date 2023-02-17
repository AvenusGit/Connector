using ConnectorCenter.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Net.Http.Headers;
using ConnectorCenter;
using ConnectorCenter.Services.Logs;
using ConnectorCenter.Models.Settings;
using ConnectorCenter.Services.Authorize;

// Upload logger configuration. It sometimes doesn't work.
// Watch = true IMPORTANT!, need for logger configuration update!
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "Configurations/Log.config", Watch = true)]

// Logger Internal logging. Default = false.
log4net.Util.LogLog.InternalDebugging = true;
log4net.GlobalContext.Properties["LogFileName"] = LogService.GetLastLogFilePath();

// Apply logger configuration 
log4net.Config.XmlConfigurator.Configure();

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

//Adding CORS
var MyAllowSpecificOrigins = "currentCORS";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost")
                                            .AllowAnyHeader()
                                            .AllowAnyMethod();
                      });
});

// Adding default console logger
builder.Logging.Services.AddLogging();

// Adding log4net file logger
if(!File.Exists(LogSettings.ConfigurationPath))
{
    LogSettings.SaveDefaultConfiguration();
}
builder.Logging.AddLog4Net(LogSettings.ConfigurationPath);

// Authentification
// Cookies for web, jwt for API
builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = "BearerOrCookies";
        options.DefaultSignInScheme = "BearerOrCookies";
        options.DefaultSignOutScheme = "BearerOrCookies";
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        // Default login page
        options.LoginPath = new PathString("/login");
        options.ExpireTimeSpan = TimeSpan.FromHours(4);
        options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.IsEssential = true;
        //TODO options.AccessDeniedPath =
    })
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = JwtAuthorizeService.AuthOptions.ISSUER,
                ValidateAudience = true,
                ValidAudience = JwtAuthorizeService.AuthOptions.AUDIENCE,
                ValidateLifetime = true,
                IssuerSigningKey = JwtAuthorizeService.AuthOptions.GetSymmetricSecurityKey(),
                ValidateIssuerSigningKey = true,
            };
        })
    .AddPolicyScheme("BearerOrCookies", "BearerOrCookies", options =>
    {
        options.ForwardDefaultSelector = context =>
        {
            string authorization = context.Request.Headers[HeaderNames.Authorization];
            // jwt token using only if headers contains request for this
            if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
                return JwtBearerDefaults.AuthenticationScheme;
            return CookieAuthenticationDefaults.AuthenticationScheme;
        };
    });

// Authorization policy
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(JwtBearerDefaults.AuthenticationScheme, policy =>
    {
        policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
        policy.RequireAuthenticatedUser();
    });
    options.AddPolicy(CookieAuthenticationDefaults.AuthenticationScheme, policy =>
    {
        policy.AuthenticationSchemes.Add(CookieAuthenticationDefaults.AuthenticationScheme);
        policy.RequireAuthenticatedUser();
    });
    options.AddPolicy("BearerOrCookies", policy =>
    {
        policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
        policy.AuthenticationSchemes.Add(CookieAuthenticationDefaults.AuthenticationScheme);
        policy.RequireAuthenticatedUser();
    });
    //options.DefaultPolicy = options.GetPolicy("BearerOrCookies")!;
});

// Connect Postgre DB
var connectionString = builder.Configuration.GetConnectionString("PostgresConnectionString");
builder.Services.AddDbContext<DataBaseContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Adding MVC
builder.Services.AddControllersWithViews(mvcOtions =>
{
    mvcOtions.EnableEndpointRouting = false;
});
builder.Services.AddControllers
    (options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);

var app = builder.Build();

try
{
    // Application singletone initialization
    log4net.Config.XmlConfigurator.Configure(new FileInfo(LogSettings.ConfigurationPath));
    ConnectorCenterApp.CreateInstance(app.Logger);
    ConnectorCenterApp.Instance.Initialize();
}
catch
{
    app.Logger.LogCritical($"Не удалось применить конфигурацию логгера {LogSettings.ConfigurationPath}. " +
        $"Проверьте существование и структуру файла.");
}
app.UseCors(MyAllowSpecificOrigins);
app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error"); //TODO
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// static files access (wwwroot folder)
app.UseStaticFiles();

// mvc default route
app.UseMvcWithDefaultRoute();

// enable mvc
app.UseMvc(routes =>
{
    routes.MapRoute( // default route
        name: "default",
        template: "{controller=Home}/{action=Index}/{id?}");
});
// run
using (var scope = app.Logger.BeginScope(".NET CORE 6"))
{
    app.Run();
}