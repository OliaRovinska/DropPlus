using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DropPlus.BLL.AutoMapper;
using DropPlus.Common.Authentication;
using DropPlus.Common.Enums;
using DropPlus.Common.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DropPlus.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // Set up jwt authorization
            services.AddBearerAuthorization(Configuration);

            // Add swagger
            services.AddSwaggerDocumentation();

            Mapping.Initialize();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Use(async (context, next) => {
                await next();
                if ((context.Response.StatusCode == 404 || context.Response.StatusCode == 401 || context.Response.StatusCode == 403) &&
                    !Path.HasExtension(context.Request.Path.Value) &&
                    !context.Request.Path.Value.StartsWith("/api/"))
                {
                    context.Request.Path = "/dist/index.html";
                    await next();
                }
            });

            app.UseAuthentication();

            app.UseSwaggerDocumentation();

            app.UseMvcWithDefaultRoute();

            app.UseDefaultFiles();

            app.UseStaticFiles();
        }
    }
}
