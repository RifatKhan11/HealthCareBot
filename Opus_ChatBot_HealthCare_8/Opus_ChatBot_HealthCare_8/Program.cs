using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Opus_ChatBot_HealthCare_8.Data;
using Opus_ChatBot_HealthCare_8.HUB;
using Opus_ChatBot_HealthCare_8.Models.BotModels;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(); // Replaces AddMvc()

#region Database Settings
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
#endregion

#region CORS Related Settings
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policyBuilder =>
        {
            policyBuilder.WithOrigins(
                "http://localhost:23997",
                "https://d7wq3zdp-23997.asse.devtunnels.ms/",
                "https://www.evercarebd.com/"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
        });
});
#endregion

#region For IP Tracking
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear(); // Allow all networks
    options.KnownProxies.Clear();  // Allow all proxies
});
#endregion

// Add SignalR
builder.Services.AddSignalR();

// Cookie Policy
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

var app = builder.Build();

// Use Forwarded Headers (for Nginx or reverse proxy)
app.UseForwardedHeaders();

app.Use(async (context, next) =>
{
    // Set X-Frame-Options header to allow framing from any origin
    context.Response.Headers.Add("X-Frame-Options", "ALLOWALL");
    await next();
});

// Error handling
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseCors("AllowSpecificOrigin"); //  Apply CORS Middleware

app.UseAuthentication();
app.UseAuthorization();

app.UseCookiePolicy();

// Configure SignalR
app.MapHub<TotaChat>("/chat");

// Configure MVC routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();