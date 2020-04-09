using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using APBD_19._03_CW3.DAL;
using APBD_19._03_CW3.Middlewares;
using APBD_19._03_CW3.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace APBD_19._03_CW3
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
            services.AddSingleton<IDbService, MockDbService>();
            services.AddTransient<IStudentServiceDB, SqlStudentServiceDb>();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            
            app.Use(async (context, next) =>
            {
                if (!context.Request.Headers.ContainsKey("Index"))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Nie podales indexu w naglowku");
                    return;
                }

                string studentIndex = context.Request.Headers["Index"].ToString();
                Console.WriteLine(studentIndex);
                
                using (var client = new SqlConnection("Data Source=db-mssql;Initial Catalog=s19036;Integrated Security=True"))
                using (var com = new SqlCommand())
                {
                    com.Connection = client;
                    client.Open();

                    com.CommandText = "Select * FROM Student WHERE IndexNumber = @studentIndex";
                    com.Parameters.AddWithValue("studentIndex", studentIndex);
                    var db = com.ExecuteReader();

                    if (!db.Read())
                    {
                        context.Response.StatusCode = StatusCodes.Status404NotFound;
                        context.Response.WriteAsync("Nie znaleziono takiego studenta");
                        return;
                    }
                }
                await next();
            });

            app.UseMiddleware<LogMiddleware>();


            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}