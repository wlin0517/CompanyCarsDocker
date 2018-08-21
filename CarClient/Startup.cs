﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CarClient.DAL;
using CarClient.Models;
using CarClient.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CarClient
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
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc();
            var task = ConfigureServicesAsync(services);

            task.Wait();
        }

        public async Task ConfigureServicesAsync(IServiceCollection services)
        {
            string aspNetDb = null;
            var aspNetDbLocation = new AspNetDbLocation();
            try
            {
                aspNetDb = await aspNetDbLocation.GetAspNetDbAsync();
            }
            catch (Exception e)
            {
                //Do nothing
            }
            if (aspNetDb != null)
            {
                var test = File.Exists(aspNetDb);
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlite("Data Source=" + aspNetDb));
            }
            else
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlite("Data Source=" + Directory.GetCurrentDirectory() + "/App_Data/AspNet.db"));
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Magic8Ball}/{action=Index}/{id?}");
            });
        }
    }
}
