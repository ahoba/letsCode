using Forcebook.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Forcebook
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
            services.AddDbContext<RebelsDbContext>(opt => opt.UseInMemoryDatabase("Rebels"));

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Forcebook API",
                    Description = "May the Force be with you",
                    Contact = new OpenApiContact
                    {
                        Name = "R2D2",
                        Email = string.Empty
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Use under LICX",
                        Url = new Uri("https://example.com/license"),
                    }
                });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
                c.SchemaFilter<EnumSchemaFilter>();
            });

            services.AddSwaggerGenNewtonsoftSupport();
        }

        public class EnumSchemaFilter : ISchemaFilter
        {
            public void Apply(OpenApiSchema model, SchemaFilterContext context)
            {
                if (context.Type.IsEnum)
                {
                    model.Enum.Clear();
                    Enum.GetNames(context.Type)
                        .ToList()
                        .ForEach(name => model.Enum.Add(new OpenApiString($"{Convert.ToInt64(Enum.Parse(context.Type, name))} - {name}")));
                }
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Forcebook v1"));

                using (var scope = app.ApplicationServices.CreateScope())
                {
                    using (var context = scope.ServiceProvider.GetService<RebelsDbContext>())
                    {
                        context.Database.EnsureCreated();
                    }
                }
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
