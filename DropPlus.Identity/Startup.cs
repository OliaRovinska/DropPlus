using DropPlus.Common.Authentication;
using DropPlus.Common.Helpers;
using DropPlus.DAL;
using DropPlus.DAL.Entities;
using DropPlus.Identity.Authentication.TokenFactory;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DropPlus.Identity
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Configure application services
            services.AddSingleton<ITokenFactory, TokenFactory>();

            // Connect to db
            services.AddDbContext<DropPlusDbContext>(options =>
                options.UseSqlServer(Configuration["Data:ConnectionString"]));

            // Set up IdentityRole 
            services.AddIdentity<AppUser, IdentityRole>(opts =>
                {
                    opts.Password.RequiredLength = 6;
                    opts.Password.RequireNonAlphanumeric = false;
                    opts.Password.RequireLowercase = false;
                    opts.Password.RequireUppercase = false;
                    opts.Password.RequireDigit = false;
                    opts.User.RequireUniqueEmail = true;
                }).AddEntityFrameworkStores<DropPlusDbContext>()
                .AddDefaultTokenProviders();

            // Set up jwt authorization
            services.AddBearerAuthorization(Configuration);

            // Add swagger
            services.AddSwaggerDocumentation();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseSwaggerDocumentation();

            app.UseMvc();
        }
    }
}
