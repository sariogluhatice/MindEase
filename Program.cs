using Microsoft.EntityFrameworkCore;
using MindEase.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// DbContext kaydı
builder.Services.AddDbContext<MindEaseContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("MindEaseDb")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Başlangıç verilerini (seed) ekle
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<MindEaseContext>();
    context.Database.EnsureCreated();
    SeedData.Initialize(context);
}

app.Run();
