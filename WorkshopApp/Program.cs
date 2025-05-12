using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using WorkshopApp.Middlewares;
using WorkshopApp.Repositories;
using WorkshopApp.Services;

namespace WorkshopApp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();
        builder.Services.AddControllers();
        builder.Services.AddScoped<IClientRepository, ClientRepository>(); 
        builder.Services.AddScoped<IMechanicRepository, MechanicRepository>();
        builder.Services.AddScoped<IVisitsRepository, VisitsRepository>();
        builder.Services.AddScoped<IVisitsService, VisitsService>();

            
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "WorkshopApp",
                Version = "v1",
                Description = "Rest API for managing Workshop visits",
                Contact = new OpenApiContact
                {
                    Name = "API Suppoert",
                    Email = "suppert@example.com",
                    Url = new Uri("https://example/suppert")
                },
                License = new OpenApiLicense
                {
                    Name = "MIT License",
                    Url = new Uri("https://opensource.org/licenses/MIT")
                }
            });
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseGlobalExceptionHandling(); //registering my custom middleware

        app.UseSwagger();
        
        //Enable middleware to serve swagger-ui
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "WorkshopApp API v1");

            //Basic UI Customization
            c.DocExpansion(DocExpansion.List);
            c.DefaultModelsExpandDepth(0); //Hide schemas section by default
            c.DisplayRequestDuration(); //Show request duration
            c.EnableFilter(); //Enable filtering operation
        });

        app.UseHttpsRedirection();

        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}