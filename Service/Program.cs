using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using Service.Repositories.Implements;
using Service.Repositories.Interfaces;
using Service.Repositories.MyDb;
using Service.Services.Implements;
using Service.Services.Interfaces;
using System.Text;

namespace Service;

public class Program
{
    public static async Task Main(string[] args)
    {
        string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var levelSwitch = new Serilog.Core.LoggingLevelSwitch();
        var logger = new LoggerConfiguration()
            .MinimumLevel.ControlledBy(levelSwitch)
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.WithProperty("InstanceId", Guid.NewGuid().ToString("n"))
            .Enrich.FromLogContext()
            .WriteTo.Console(
            outputTemplate: "{Timestamp:HH:mm} [{Level}] ({ThreadId}) {Message}{NewLine}{Exception}",
            theme: AnsiConsoleTheme.Code,
            restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information
            )
            .WriteTo.Debug(
            outputTemplate: "{Timestamp:HH:mm} [{Level}] ({ThreadId}) {Message}{NewLine}{Exception}",
            restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information
            )
            .Filter.ByExcluding(e => e.MessageTemplate.Text.Contains("_framework"))
            .CreateLogger();
        //builder.Logging.ClearProviders();
        builder.Logging.AddSerilog(logger);

        string connectionString = builder.Configuration.GetConnectionString("SupabaseConnection");
        builder.Services.AddDbContext<MyDbContext>(options =>
        options.UseNpgsql(connectionString));
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: MyAllowSpecificOrigins,
                              policy =>
                              {
                                  policy.WithOrigins("http://*.ap-southeast-1.elb.amazonaws.com",
                                                      "https://localhost:7069",
                                                      "http://localhost:5233",
                                                      "https://localhost:7000"
                                                      )
                                  .SetIsOriginAllowedToAllowWildcardSubdomains()
                                  .AllowAnyHeader()
                                  .AllowAnyMethod();
                              });
        });

        builder.Services.AddAuthorization();
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(o =>
        {
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = builder.Configuration.GetValue<string>("Jwt:Issuer"),
                ValidAudience = builder.Configuration.GetValue<string>("Jwt:Audience"),
                IssuerSigningKey = new SymmetricSecurityKey
                (Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("Jwt:Key"))),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true
            };
        });

        builder.Services.AddHealthChecks();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseBlazorFrameworkFiles();
        app.UseStaticFiles();

        app.UseRouting();

        //if (app.Environment.IsDevelopment() == false)
        {
            app.UseCors(MyAllowSpecificOrigins);
        }

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.MapHealthChecks("/healthz");

        app.Run();
    }
}