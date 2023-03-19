using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using ModsenLibGateWay.Helpers;
using ModsenLibGateWayCQS.Tokens.Commands;
using ModsenLibGateWayCQS.Users.Commands;
using ModsenLibGateWayCQS.Users.Queries;
using ModsenLibGateWayDb;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Serilog;
using Serilog.Events;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using MD5 = ModsenLibGateWay.Helpers.MD5;

namespace ModsenLibGateWay
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
                 @"D:\Logs\modsenlibgate\gateapi_data.log",
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

            builder.Services.AddDbContext<ModsenLibGateWayContext>(
                optionsBuilder => optionsBuilder.UseSqlServer(connectionString));

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            // builder.Services.AddSwaggerGen();
            builder.Services.AddSwaggerGen(options =>
            {
                options.IncludeXmlComments(builder.Configuration["XmlDoc"]);
            });

            builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
            builder.Services.AddOcelot(builder.Configuration);

            builder.Services
              .AddAuthentication(options =>
              {
                  options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                  options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                  options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
              })
              .AddJwtBearer("jwt_auth_scheme", opt =>
              {
                  opt.RequireHttpsMetadata = false;
                  opt.SaveToken = true;
                  opt.TokenValidationParameters = new TokenValidationParameters()
                  {
                      ValidIssuer = builder.Configuration["Token:Issuer"],
                      ValidAudience = builder.Configuration["Token:Issuer"],
                      IssuerSigningKey =
                          new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:JwtSecret"])),
                      ClockSkew = TimeSpan.Zero
                  };
              });

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            //builder.Services.AddMediatR(typeof(AddRefreshTokenCommand).Assembly);
            builder.Services.AddMediatR(x => x.RegisterServicesFromAssemblies(typeof(AddRefreshTokenCommand).Assembly));
            builder.Services.AddScoped<JWTSha256>();
            builder.Services.AddScoped<MD5>();
            builder.Services.AddScoped<DataChecker>();

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
            //app.UseOcelot();
            app.UseOcelot().Wait();

            app.Run();
        }
    }
}