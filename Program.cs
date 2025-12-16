using Microsoft.AspNetCore.Identity; 
using Microsoft.EntityFrameworkCore;
using StanaGO.Data; 
using StanaGO.Models; 

var builder = WebApplication.CreateBuilder (args);


builder.Services.AddDbContext<StanaGOContext> (options =>
    options.UseSqlServer (builder.Configuration.GetConnectionString ("DefaultConnection")));


builder.Services.AddIdentity<User, IdentityRole> (options => {  // optiuni cerinte pentru inregistrare (dezactivate pentru simplificarea testului)
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false; 
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false; 
    options.Password.RequireNonAlphanumeric = false; 
    options.Password.RequiredLength = 4;
})
    .AddEntityFrameworkStores<StanaGOContext> (); 

builder.Services.AddControllersWithViews ();

var app = builder.Build ();

using ( var scope = app.Services.CreateScope () )   // initializarea rolurilor in baza de date
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>> ();

    string [ ] roleNames = { "Casual", "Shepherd", "Moderator" };

    foreach ( var roleName in roleNames )
    {
        if ( !await roleManager.RoleExistsAsync (roleName) )
        {
            await roleManager.CreateAsync (new IdentityRole (roleName));
        }
    }
}


if ( !app.Environment.IsDevelopment () )
{
    app.UseExceptionHandler ("/Home/Error");
    app.UseHsts ();
}

app.UseHttpsRedirection ();
app.UseStaticFiles ();

app.UseRouting ();

app.UseAuthentication (); 
app.UseAuthorization (); 

app.MapControllerRoute (              // deschide prima data pagina de welcome
    name: "default",
    pattern: "{controller=Home}/{action=Welcome}/{id?}");

app.Urls.Add ("http://*:5022");
app.Urls.Add ("https://*:7262");

app.Run ();