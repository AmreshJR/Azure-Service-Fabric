
using System.Fabric;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using NotificationService.BusinessRules.Hub;
using NotificationService.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using NotificationService.DTO.Common;
using NotificationService.Logger;

namespace NotificationService
{
    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance.
    /// </summary>
    internal sealed class NotificationService : StatelessService
    {
        public NotificationService(StatelessServiceContext context)
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
                        //Jwt Authentication
                        var appSettingsSection = builder.Configuration.GetSection("DtoToken");
                        builder.Services.Configure<DtoToken>(appSettingsSection);

                        // configure jwt authentication
                        var appSettings = appSettingsSection.Get<DtoToken>();
                        var key = Encoding.ASCII.GetBytes(appSettings.JWT_Secret);
                        builder.Services.AddSignalR();
                        builder.Services.AddSingleton<StatelessServiceContext>(serviceContext);
                        builder.WebHost
                                    .UseKestrel()
                                    .UseContentRoot(Directory.GetCurrentDirectory())
                                    .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.None)
                                    .UseUrls(url);
                        
                        // Add services to the container.
                        
                        builder.Services.AddControllers();
                        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                        builder.Services.AddEndpointsApiExplorer();
                        builder.Services.AddSwaggerGen();
                        builder.Services.AddTransient<Log>();
                        builder.Services.AddSingleton<ChatHub>();
                        builder.Services.AddSingleton<ConnectionMapping>();
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
                            //Authorization for SignalR
                            x.Events = new JwtBearerEvents
                            {
                                OnMessageReceived = context =>
                                {
                                        var accessToken = context.Request.Query["access_token"];

                                        // If the request is for hub.
                                        var path = context.HttpContext.Request.Path;
                                        if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/chat")))
                                        {
                                            // Read the token out of the query string
                                            context.Token = accessToken;
                                        }
                                        return Task.CompletedTask;
                                }
                            };
                        });
                        var app = builder.Build();
                        
                        // Configure the HTTP request pipeline.
                        if (app.Environment.IsDevelopment())
                        {
                            app.UseSwagger();
                            app.UseSwaggerUI();
                        }

                        app.UseAuthentication();

                        app.UseCors();

                        app.UseAuthorization();
                        
                        app.MapControllers();

                        app.MapHub<ChatHub>("/chat");

                        return app;


                    }))
            };
        }
    }
}
