using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JustNote.Attributes;
using JustNote.Models;
using JustNote.Serivces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;

namespace JustNote
{
    public class Startup
    {
        public const string AppS3BucketKey = "AppS3Bucket";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        readonly string JustNoteSpecificOrigins = "_JustNoteSpecificOrigins";

        public static IConfiguration Configuration { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddCors(options =>
            {
                options.AddPolicy(JustNoteSpecificOrigins,
                builder =>
                {
                    builder.WithOrigins("http://justnote-test.us-west-2.elasticbeanstalk.com/", 
                        "https://cb5eza7o22.execute-api.us-west-2.amazonaws.com/Prod")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            services.AddSingleton<TokenManagerService>();
            services.AddScoped<IDatabaseItemService<Note>, NoteService>();
            services.AddScoped<IDatabaseItemService<Folder>, FolderService>();

            services.AddAWSService<Amazon.S3.IAmazonS3>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "Test API",
                    Description = "ASP.NET Core Web API"
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
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

            app.UseCors(JustNoteSpecificOrigins);

            app.UseHttpsRedirection();
            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/Prod/swagger/v1/swagger.json", "Test API V1");
            });
        }
    }
}
