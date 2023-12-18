using FasleSafar.Data;
using FasleSafar.Data.Repositories;
using FasleSafar.Data.Services;
using FasleSafar.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using NuGet.Configuration;
using Parbad.Builder;
using Parbad.Gateway.Mellat;
using Parbad.Gateway.Melli;
using Parbad.Gateway.ParbadVirtual;
using Parbad.Gateway.Saman;
using Parbad.Gateway.ZarinPal;
using Serilog;
using System.Net;
using System.Security.Claims;
using System.Text.Json.Serialization;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var settings = builder.Configuration.GetSection("Settings").Get<ConfigSettings>();

#region Db Context
//intro dbcontext to Core service for work with database
builder.Services.AddDbContext<FasleSafarContext>(options =>
{
    options.UseSqlServer(settings.ConnectionString);
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

#endregion

#region IoC

builder.Services.AddScoped<IOrderRep, OrderRep>();
builder.Services.AddScoped<IUserRep, UserRep>();
builder.Services.AddScoped<ITourRep, TourRep>();
builder.Services.AddScoped<IDestinationRep, DestinationRep>();
builder.Services.AddScoped<IContentRep, ContentRep>();
builder.Services.AddScoped<IReqTripRep, ReqTripRep>();
builder.Services.AddScoped<IMessageRep, MessageRep>();
builder.Services.AddScoped<IAttractionRep, AttractionRep>();
builder.Services.AddScoped<IPassengerRep, PassengerRep>();
builder.Services.AddScoped<ITokenRep, TokenRep>();
#endregion

#region AjaxSearch

builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
#endregion

#region AuthenticationCookies

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme) //Set Cookie Settings
    .AddCookie(option =>
    {
        option.LoginPath = "/Account/Login"; //Set login Action url
        option.LogoutPath = "/Account/Logout"; //Set logout Action url
        option.ExpireTimeSpan = TimeSpan.FromDays(20); //Set Time for Cookies (Stay login)
    });

#endregion

#region PaymentGetWay

builder.Services.AddParbad()
        .ConfigureGateways(gateways =>
        {
			gateways
			  .AddZarinPal()
			  .WithAccounts(accounts =>
			  {
				  accounts.AddInMemory(account =>
				  {
					  account.MerchantId = "f024bc76-f262-456e-8e9b-d59d32e12cd5";
					  account.IsSandbox = true;
				  });
			  });

			gateways
				.AddParbadVirtual()
				.WithOptions(options => options.GatewayPath = "/MyVirtualGateway");

			gateways
                 .AddMelli()
                 .WithAccounts(source =>
                 {
                     source.AddInMemory(account =>
                     {
                         account.TerminalId = "24103696";
                         account.TerminalKey = "LNicFFgaletU0Rojdodz/ifhaLrkBkbT";
                         account.MerchantId = "000000140342110";
                     });
                 });
        })
         .ConfigureHttpContext(httpContextBuilder => httpContextBuilder.UseDefaultAspNetCore())
       .ConfigureStorage(storageBuilder => storageBuilder.UseMemoryCache());

#endregion

string logPath = Path.Combine(Directory.GetCurrentDirectory(),
					  "wwwroot",
					  "serilog.txt");

builder.Host.UseSerilog((hostContext, services, configuration) => {
	configuration.WriteTo.File(logPath);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSerilogRequestLogging();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseParbadVirtualGateway();


// If Request is For Admin Part & The user is not Logined or The user is not Admin Go to Page /Account/Login
app.Use(async (context, next) =>
{
    // Do work that doesn't write to the Response.
    if (context.Request.Path.StartsWithSegments("/Admin"))
    {
        if (!context.User.Identity.IsAuthenticated)
        {
            context.Response.Redirect("/Account/Login");
        }
        else if (context.User.FindFirstValue("IsAdmin").ToString() != "مدیر")
        {
            context.Response.Redirect("/Account/Login");
        }
    }
    await next.Invoke();
    // Do logging or other work that doesn't write to the Response.
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseExceptionHandler(errorApp =>
{
	errorApp.Run(async context =>
	{
		context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
		context.Response.ContentType = "text/html";

		var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();

        // ثبت خطا در Serilog
        Log.Error(exceptionHandlerPathFeature.Error, "یک خطای داخلی رخ داد: {ErrorMessage}", exceptionHandlerPathFeature.Error.Message + '\n' + exceptionHandlerPathFeature.Error.StackTrace + '\n' + exceptionHandlerPathFeature.Error.Source + '\n' + exceptionHandlerPathFeature.Error.TargetSite + '\n'
            + exceptionHandlerPathFeature.Error.InnerException?.Message +'\n' + exceptionHandlerPathFeature.Error.InnerException?.StackTrace + '\n' + exceptionHandlerPathFeature.Error.InnerException?.Source + '\n' + exceptionHandlerPathFeature.Error.InnerException?.TargetSite + '\n');



        // ...
    });

});
app.Run();
