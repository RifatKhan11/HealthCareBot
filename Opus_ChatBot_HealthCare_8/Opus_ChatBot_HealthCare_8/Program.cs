using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Opus_ChatBot_HealthCare_8.Data;
using Opus_ChatBot_HealthCare_8.HUB;
using Opus_ChatBot_HealthCare_8.IServices.IServices;
using Opus_ChatBot_HealthCare_8.LogicAdaptar;
using Opus_ChatBot_HealthCare_8.LogicAdaptar.Interface;
using Opus_ChatBot_HealthCare_8.Models.BotModels;
using Opus_ChatBot_HealthCare_8.Services;
using Opus_ChatBot_HealthCare_8.Services.Dapper.IInterfaces;
using Opus_ChatBot_HealthCare_8.Services.IServices;
using OPUSERP.SCM.SMSService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(); // This includes ViewFeatures
builder.Services.AddRazorPages();
builder.Services.AddMvcCore();

// Add services to the container
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

// IP Tracking
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear(); // Allow all networks
    options.KnownProxies.Clear();  // Allow all proxies
});

// Dependency Injection (DI)
builder.Services.AddScoped<IFacebookService, FacebookService>();
builder.Services.AddScoped<IResponseBuilderService, ResponseBuilderService>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IQueriesService, QueriesService>();
builder.Services.AddScoped<IQuestionReplayService, QuestionReplayService>();
builder.Services.AddScoped<ILanguageService, LanguageService>();
builder.Services.AddScoped<IOTPService, OTPService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IBotFlowService, BotFlowService>();
builder.Services.AddScoped<IServiceFlowService, ServiceFlowService>();
builder.Services.AddScoped<IUserInfoService, UserInfoService>();
builder.Services.AddScoped<IPassportInfoService, PassportInfoService>();
builder.Services.AddScoped<IPoliceDashBoardService, PoliceDashBoardService>();
builder.Services.AddScoped<IBankInfoService, BankInfoService>();
builder.Services.AddScoped<IKeyWordQuesService, KeyWordQuesService>();
builder.Services.AddScoped<IKnowledgeService, KnowledgeService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddScoped<IHubServiceManager, HubServiceManager>();
builder.Services.AddScoped<IDoctorInfoService, DoctorInfoService>();
builder.Services.AddScoped<ISMSService, SMSService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IBotService, BotService>();
builder.Services.AddScoped<IDapper, Opus_ChatBot_HealthCare_8.Services.Dapper.Dapper>();

// Database Settings
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity Settings
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Hosted Service
builder.Services.AddHostedService<ApiUpdateService>();

// CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder =>
    {
        builder.WithOrigins(
            "http://localhost:23997",
            "https://d7wq3zdp-23997.asse.devtunnels.ms/",
            "https://www.evercarebd.com/"
        ).AllowAnyHeader()
         .AllowAnyMethod()
         .AllowCredentials();
    });
});

// Master Data Services
builder.Services.AddScoped<IquestionCategoryService, questionCategoryService>();
builder.Services.AddScoped<IAPIFunction, APIFunction>();
builder.Services.AddSingleton<IGlobal, GlobalService>();

// Authentication Settings
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(1);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(1);
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
});

//Add MVC and JSON Configuration
//builder.Services.AddControllers()
//    .AddJsonOptions(options =>
//    {
//        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
//    });
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    });


// SignalR
builder.Services.AddSignalR();

var app = builder.Build();

// Middleware Configuration
app.UseForwardedHeaders();

app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Frame-Options", "ALLOWALL");
    await next();
});

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseCors("AllowSpecificOrigin");

//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCookiePolicy();
app.UseAuthentication();
app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<TotaChat>("/chat");

    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();
