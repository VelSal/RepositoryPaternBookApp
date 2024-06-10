using Microsoft.EntityFrameworkCore;
using RepositoryPaternBookApp.Data;
using RepositoryPaternBookApp.Interfaces;
using RepositoryPaternBookApp.Repositories;

namespace RepositoryPaternBookApp
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddDbContext<RepoContext>(options =>
			{
				options.UseSqlServer(builder.Configuration.GetConnectionString("RepoConn"));
			});

			builder.Services.AddControllersWithViews();

			// Add dependency injection lifetime
			builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

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
				pattern: "{controller=Books}/{action=Index}/{id?}");

			app.Run();
		}
	}
}
