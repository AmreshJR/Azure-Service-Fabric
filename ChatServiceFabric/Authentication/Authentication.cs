using System;
using System.Collections.Generic;
using System.Fabric;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.ServiceFabric.Data;
using AuthenticationDAL.Logger.MicroFrontendDal.BusinessRules.Logger;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AuthenticationDAL.DTO.Common;
using AuthenticationDAL.BusinessRules.User;
using Microsoft.EntityFrameworkCore;
using NLog.Web;
using AuthenticationDAL.DataModels;

namespace Authentication
{
    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance.
    /// </summary>
    internal sealed class Authentication : StatelessService
    {
        public Authentication(StatelessServiceContext context)
            : base(context)
        { }

        /// <summary>
        /// Optional override to create listeners (like tcp, http) for this service instance.
        /// </summary>
        /// <returns>The collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[]
            {
                new ServiceInstanceListener(serviceContext =>
                    new KestrelCommunicationListener(serviceContext, "ServiceEndpoint", (url, listener) =>
                    {
                        ServiceEventSource.Current.ServiceMessage(serviceContext, $"Starting Kestrel on {url}");

                        var builder = WebApplication.CreateBuilder();
                        var connection = builder.Configuration.GetConnectionString("ChatDb");
                        builder.Services.AddSingleton<StatelessServiceContext>(serviceContext);
                        builder.WebHost
                                    .UseKestrel()
                                    .UseContentRoot(Directory.GetCurrentDirectory())
                                    .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.None)
                                    .UseUrls(url);

                        builder.Services.AddControllers();
                        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                        builder.Services.AddEndpointsApiExplorer();
                        builder.Services.AddSwaggerGen();
                            //Jwt Authentication
                        var appSettingsSection = builder.Configuration.GetSection("DtoToken");
                        builder.Services.Configure<DtoToken>(appSettingsSection);

                            // configure jwt authentication
                        var appSettings = appSettingsSection.Get<DtoToken>();
                        var key = Encoding.ASCII.GetBytes(appSettings.JWT_Secret);

                        builder.Services.AddAuthentication(x =>
                        {
                             x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                             x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                             x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                        }).AddJwtBearer(x =>
                        {
                            x.RequireHttpsMetadata = false;
                            x.SaveToken = true;
                            x.TokenValidationParameters = new TokenValidationParameters
                            {
                                ValidateIssuerSigningKey = true,
                                IssuerSigningKey = new SymmetricSecurityKey(key),
                                ValidateIssuer = false,
                                ValidateAudience = false,
                                ClockSkew = TimeSpan.Zero
                            };
                        });
                        builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
                        builder.Services.AddDbContext<AppDbContext>(options =>
                        options.UseSqlServer(connection));
                        builder.Services.AddDbContext<AuthenticationContext>(options =>
                        options.UseSqlServer(connection));
                        builder.Services.AddCors(options =>
                        {
                            options.AddDefaultPolicy(x =>
                            {
                                x.AllowAnyOrigin()
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .SetIsOriginAllowed(_ => true);
                            });
                        });
                        builder.Services.AddScoped<IUser, AuthenticationDAL.BusinessRules.User.User>();
                        //builder.Services.AddTransient<Storage>();
                        builder.Services.AddSingleton<Log>();
                        //builder.Services.AddTransient<AppConfigDetails>();
                         //NLog: Setup NLog for Dependency injection
                        //builder.Logging.ClearProviders();
                        //builder.Host.UseNLog();
                        var app = builder.Build();



                        Log Logger = new();
                        Logger.InfoLog("Server Started");
                        if (app.Environment.IsDevelopment())
                        {
                            app.UseSwagger();
                            app.UseSwaggerUI();
                        }

                        app.UseAuthentication();

                        app.UseCors();

                        app.UseAuthorization();

                        app.MapControllers();

                        return app;


                    }))
            };
        }
    }
}
