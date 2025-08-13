using Microsoft.AspNetCore.Identity;
using BookShopingCartMVC.Repository.IRepository;
using BookShopingCartMVC.Services.IServices;
using BookShopingCartMVC.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using BookShopingCartMVC.Models;
using System.Text.Json.Serialization;
using QuestPDF.Infrastructure;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true; // Optional: for better readability
    });

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IGenreRepository, GenreRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IUserOrderRepository, UserOrderRepository>();
builder.Services.AddScoped<IStockRepository, StockRepository>();
builder.Services.AddScoped<IFileServices, FileServices>();
builder.Services.AddScoped<ILikeRepository, LikeRepository>();
builder.Services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<IPdfGenerator, PdfGenerator>();

// Register QuestPDF license in your Application_Start event or similar
QuestPDF.Settings.License = LicenseType.Community;
var app = builder.Build();

// Seed the default Data into DB.
//using (var scop = app.Services.CreateScope())
//{
//    await DbSeeder.SeedDefault(scop.ServiceProvider);
//}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
      name: "Default",
      pattern: "{area=Guest}/{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
