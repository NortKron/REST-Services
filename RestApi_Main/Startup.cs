using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using RestApi_Main.Models;

namespace RestApi_Main
{
    public class Startup
    {
        public const string CFG_DATA_DB = @"ConnectionStrings:DefaultConnection";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Регистрация IHttpClientFactory
            //services.AddHttpClient();

            services.AddControllers()
                .AddNewtonsoftJson();                

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo 
                { 
                    Title = "REST API", 
                    Version = "v1",
                    Description = "Тестовый REST-сервис"
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddDbContext<DataContext>(options => options.UseSqlServer(Configuration[CFG_DATA_DB]));
            //services.AddDbContext<DataContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            //services.AddDbContext<DataContext>(options => options.UseSqlServer("Server=LAPTOP-DELL\\SQLEXPRESS; Database=REST_DB; User ID=sa;Password=112233; Trusted_Connection=True;"));

            services.AddDbContext<DataContext>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "RestApi_Main v1"));
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            /*
            // Включениа логгирования
            app.Run(async (context) =>
            {
                // пишем на консоль информацию
                //logger.LogInformation("Processing request {0}", context.Request.Path);
                // или так
                //logger.LogInformation($"Processing request {context.Request.Path}");

                await context.Response.WriteAsync("Hello World!");
            });
            */

            app.UseStatusCodePages();
            app.UseStaticFiles();

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                //endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}");
            });
        }
    }
}
