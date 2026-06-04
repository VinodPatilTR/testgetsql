var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

var app = builder.Build();

app.UseStaticFiles();
app.UseSession();
app.MapControllers();

// Redirect root → login page
app.MapGet("/", context =>
{
    context.Response.Redirect("/login.html");
    return Task.CompletedTask;
});

// Convenience logout redirect
app.MapGet("/logout", context =>
{
    context.Response.Redirect("/api/auth/logout");
    return Task.CompletedTask;
});

app.Run();
