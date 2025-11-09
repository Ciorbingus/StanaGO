using Microsoft.AspNetCore.Identity; 
using Microsoft.EntityFrameworkCore;
using StanaGO.Data; 
using StanaGO.Models; 

var builder = WebApplication.CreateBuilder (args);


builder.Services.AddDbContext<StanaGOContext> (options =>
    options.UseSqlServer (builder.Configuration.GetConnectionString ("DefaultConnection")));


builder.Services.AddIdentity<User, IdentityRole> (options => {

    // Pentru a opri confirmarea pe email
    options.SignIn.RequireConfirmedAccount = false;

    // --- AICI ANULEZI PAROLA PUTERNICĂ ---
    // Adaugă aceste linii pentru a permite parole simple

    options.Password.RequireDigit = false; // Nu cere cifre
    options.Password.RequireLowercase = false; // Nu cere minuscule
    options.Password.RequireUppercase = false; // Nu cere majuscule
    options.Password.RequireNonAlphanumeric = false; // Nu cere simboluri (ex: !)

    // Setezi o lungime minimă mică
    options.Password.RequiredLength = 4;
})
    .AddEntityFrameworkStores<StanaGOContext> (); // Sau StanaGOContext

builder.Services.AddControllersWithViews ();

var app = builder.Build ();

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

app.MapControllerRoute (
    name: "default",
    pattern: "{controller=Home}/{action=Welcome}/{id?}");

app.Run ();