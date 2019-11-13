using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MVCManukauTech.Data;
using MVCManukauTech.Models;
using MVCManukauTech.Services;
using MVCManukauTech.Models.DB;
using Microsoft.AspNetCore.Html;
namespace MVCManukauTech
{
    public class Startup
    {
        //20180312 JPC customisation of (localdb) to enable location of the database under the same parent folder as the code.
        //That way app is portable to other machines which is good for teaching and learning
        //20180312 JPC inject Hosting Environment service - ref:
        //https://blog.mariusschulz.com/2016/05/22/getting-the-web-root-path-and-the-content-root-path-in-asp-net-core

        //20180312 JPC use of Session ref:
        //https://docs.microsoft.com/en-us/aspnet/core/fundamentals/app-state?tabs=aspnetcore2x
        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        public IConfiguration Configuration { get; }

        private readonly IHostingEnvironment _hostingEnvironment;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            
            //20180312 JPC customisation of (localdb) to enable location of the database under the same parent folder as the code.
            //That way app is portable to other machines which is good for teaching and learning
            //string databaseName = Configuration.GetConnectionString("DatabaseName");
            //string connectionString = Configuration.GetConnectionString("DefaultConnection");

            //20180312 JPC _hostingEnvironment injected above gives us the "ContentRootPath"
            //string path = _hostingEnvironment.ContentRootPath + "\\Database\\" + databaseName + ".mdf";

            //connectionString = connectionString.Replace("[-PATH-]", path);
            //connectionString = connectionString.Replace("[-DATABASENAME-]", databaseName);

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddDbContext<XSpy4CoreContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddMvc();
            // Adds a default in-memory implementation of IDistributedCache.
            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                // Comment-out returns to the default of 20 min
                // options.IdleTimeout = TimeSpan.FromSeconds(10);
                options.Cookie.HttpOnly = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
               // app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseAuthentication();

            //20180312 JPC Session. Note that this needs to go in before app.UseMvc
            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });


        }
    }
}
