using Microsoft.EntityFrameworkCore;
using ModsenLibCQS.Books.Queries;
using ModsenLibDb;
using Serilog;
using Serilog.Events;
using System.Text.Json.Serialization;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ModsenLibAPI
{
    /// <summary>
    /// Program class
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main Program method
        /// </summary>
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog((ctx, lc) =>
            lc.WriteTo.File(
                @"D:\Logs\modsenlibapi\libapi_data.log",
                LogEventLevel.Information,
                 rollingInterval: RollingInterval.Day,
                 retainedFileCountLimit: null,
                 rollOnFileSizeLimit: true,
                 fileSizeLimitBytes: 4_194_304)
                .WriteTo.Console(LogEventLevel.Verbose));

            // Add services to the container.

            builder.Services.AddControllers()
                 .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

            var connectionString = builder.Configuration.GetConnectionString("Default");

            builder.Services.AddDbContext<ModsenLibAPIContext>(
                optionsBuilder => optionsBuilder.UseSqlServer(connectionString));

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            // builder.Services.AddSwaggerGen();
            builder.Services.AddSwaggerGen(options =>
            {
                options.IncludeXmlComments(builder.Configuration["XmlDoc"]);
            });

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            builder.Services.AddMediatR(x => x.RegisterServicesFromAssemblies(typeof(GetBookByIdQuery).Assembly));
            var app = builder.Build();

            app.UseStaticFiles();
            app.UseRouting();
            app.UseHttpsRedirection();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}