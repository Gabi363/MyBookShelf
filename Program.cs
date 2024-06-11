using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ContextDb>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("ContextDb") ?? 
        throw new InvalidOperationException("Connection string 'ContextDb' not found.")));

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(10);
    options.Cookie.HttpOnly = true;//plik cookie jest niedostępny przez skrypt po stronie klienta
    options.Cookie.IsEssential = true;//pliki cookie sesji będą zapisywane dzięki czemu sesje będzie mogła być śledzona podczas nawigacji lub przeładowania strony
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

app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


using (var scope = app.Services.CreateScope()) {
    // create new user if database is empty
    using(ContextDb ?db = scope.ServiceProvider.GetService<ContextDb>()) {
        if (db is null) {
            throw new Exception("UserDBContext is null");
        }

        if (db.User.Count() == 0) {
            db.User.Add(new User {
                Username = "admin",
                Password = MyBookShelf.Controllers.UserController.CreateMD5Hash("admin"),
                IsAdmin = true,
                Token = MyBookShelf.Controllers.UserController.GenerateRandomToken()
            });

            db.SaveChanges();
        }
    }
}

// app.Use(async (ctx, next) =>
//         {
//             await next();

//             if ((ctx.Response.StatusCode == 404 || ctx.Response.StatusCode == 400) && !ctx.Response.HasStarted)
//             {
//                 string originalPath = ctx.Request.Path.Value ?? "";
//                 ctx.Items["originalPath"] = originalPath;
//                 ctx.Request.Path = "/login/";
//                 ctx.Response.Redirect("/login/");
//                 await next();
//             }
//         });

app.Run();
