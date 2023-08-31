using FasleSafar.Data;
using FasleSafar.Data.Repositories;
using FasleSafar.Data.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Parbad.Builder;
using Parbad.Gateway.Mellat;
using Parbad.Gateway.Melli;
using Parbad.Gateway.ParbadVirtual;
using Parbad.Gateway.Saman;
using System.Security.Claims;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

#region Db Context
//intro dbcontext to Core service for work with database
builder.Services.AddDbContext<FasleSafarContext>(options =>
{
    //options.UseSqlServer("Data Source =.;Initial Catalog=FasleSafarDB;Integrated Security=true;TrustServerCertificate=True;");
    options.UseSqlServer(@"Server=faslesafar.com;Initial Catalog=faslesa1_FaslesafarDb;User ID=faslesa1_user;Password=139P?9mee;MultipleActiveResultSets=true;TrustServerCertificate=True;");
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

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

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
app.Run();
