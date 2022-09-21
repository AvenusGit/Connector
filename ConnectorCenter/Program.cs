using ConnectorCenter.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Net.Http.Headers;
using ConnectorCenter;
using ConnectorCenter.Services.Logs;
using ConnectorCenter.Models.Settings;


// �������������� ������� �� ��������� ������������ �������. ��� ����� �������� �����, ������� �� ��������, �� ���. ������� ��� ����������� ����.
// Watch = true �����, �.�. ����� ������� ����������� ��������� �������!
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "Configurations/Log.config", Watch = true)]

// ����� �� log4net ���������� ����������� ������. ����������� �������� log4net - ������������ ����������.
//TODO ��������� ���� �������� �� �����.
log4net.Util.LogLog.InternalDebugging = true;
log4net.GlobalContext.Properties["LogFileName"] = LogService.GetLastLogFilePath();
    
log4net.Config.XmlConfigurator.Configure();

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// ���������� ����������� ������� .Net Core
builder.Logging.Services.AddLogging();

// ���������� log4net �������
if(!File.Exists(LogSettings.ConfigurationPath))
{
    LogSettings.SaveDefaultConfiguration();
}
builder.Logging.AddLog4Net(LogSettings.ConfigurationPath);

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = "BearerOrCookies";
        options.DefaultSignInScheme = "BearerOrCookies";
        options.DefaultSignOutScheme = "BearerOrCookies";
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/login");
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
                // ���������, ����� �� �������������� �������� ��� ��������� ������
                ValidateIssuer = true,
                // ������, �������������� ��������
                ValidIssuer = AuthOptions.ISSUER,
                // ����� �� �������������� ����������� ������
                ValidateAudience = true,
                // ��������� ����������� ������
                ValidAudience = AuthOptions.AUDIENCE,
                // ����� �� �������������� ����� �������������
                ValidateLifetime = true,
                // ��������� ����� ������������
                IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                // ��������� ����� ������������
                ValidateIssuerSigningKey = true,
            };
        })
    .AddPolicyScheme("BearerOrCookies", "BearerOrCookies", options =>
    {
        options.ForwardDefaultSelector = context =>
        {
            string authorization = context.Request.Headers[HeaderNames.Authorization];
            if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
                return JwtBearerDefaults.AuthenticationScheme;
            return CookieAuthenticationDefaults.AuthenticationScheme;
        };
    });
        
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

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("PostgresConnectionString");
builder.Services.AddDbContext<DataBaseContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// ���������� MVC �������
builder.Services.AddControllersWithViews(mvcOtions =>
{
    mvcOtions.EnableEndpointRouting = false;
});
builder.Services.AddControllers
    (options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);

var app = builder.Build();

try
{
    log4net.Config.XmlConfigurator.Configure(new FileInfo(LogSettings.ConfigurationPath));
    ConnectorCenterApp.CreateInstance(app.Logger);
    ConnectorCenterApp.Instance.Initialize();
}
catch
{
    // ��������� � �������, ��� �������� ������ �� �������� ������
    app.Logger.LogCritical($"�� ������� ��������� ������������ ������� {LogSettings.ConfigurationPath}. " +
        $"��������� ������������� � ��������� �����.");
}

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

app.UseHttpsRedirection();
app.UseStaticFiles();

// ����������� ������� mvc
app.UseMvcWithDefaultRoute();

app.UseAuthentication();
app.UseAuthorization();

// �������� ������� mvc
app.UseMvc(routes =>
{
    routes.MapRoute( // ������� ������� ��� ���������� ���������� ����
        name: "default",
        template: "{controller=Home}/{action=Index}/{id?}");
});
// ������
using (var scope = app.Logger.BeginScope(".NET CORE 6"))
{
    app.Run();
}

public static class AuthOptions
{
    public const string ISSUER = "ConnectorCenter"; // �������� ������
    public const string AUDIENCE = "ConnectorClient"; // ����������� ������
    const string KEY = "ExtremeUltraSuperLoooongSecretKey";   // ���� ��� ��������
    public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
}
