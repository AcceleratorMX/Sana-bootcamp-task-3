using MyTodoList.Data;
using MyTodoList.Data.Repository;

namespace MyTodoList;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        builder.Services.AddScoped<JobRepository>();
        builder.Services.AddScoped<CategoryRepository>();

        var serverConnectionString = builder.Configuration.GetConnectionString("ServerConnection") ?? 
                                     throw new ArgumentNullException(nameof(DatabaseService));
        var dbConnectionString = builder.Configuration.GetConnectionString("DatabaseConnection") ??
                                 throw new ArgumentNullException(nameof(DatabaseService));


        var databaseService = new DatabaseService(serverConnectionString, dbConnectionString);
        databaseService.CreateDatabase();
        databaseService.CreateTables();

        builder.Services.AddSingleton(databaseService);

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

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Jobs}/{action=Todo}/{id?}");

        app.Run();
    }
}
