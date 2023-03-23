
using System.Fabric;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System.Security.Cryptography.X509Certificates;
using ChatDAL.DTO.Common;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NLog.Web;
using ChatDAL.DataModels;
using ChatDAL.Logger.MicroFrontendDal.BusinessRules.Logger;
using MediatR;
using ChatDAL.ReadDataModels;

namespace ChatService
{
    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance.
    /// </summary>
    internal sealed class ChatService : StatelessService
    {
        public ChatService(StatelessServiceContext context)
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
                         var ReadOnlyconnection = builder.Configuration.GetConnectionString("ChatReadOnlyDb");
                        builder.Services.AddSingleton<StatelessServiceContext>(serviceContext);
                        builder.WebHost
                                    .UseKestrel()
                                    .ConfigureServices(services => services
                                    .AddSingleton<StatelessServiceContext>(serviceContext))
                                    .UseContentRoot(Directory.GetCurrentDirectory())
                                    .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.None)
                                    .UseUrls(url);
                        
                        // Add services to the container.
                        builder.Services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());
                        //builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
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
                        builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
                        builder.Services.AddDbContext<AppDbContext>(options =>
                        options.UseSqlServer(connection));
                        builder.Services.AddDbContext<ChatContext>(options =>
                        options.UseSqlServer(connection));
                        builder.Services.AddDbContext<ChatReadContext>(options =>
                        options.UseSqlServer(ReadOnlyconnection));
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
                        //builder.Services.AddScoped<ChatDAL.BusinessRules.Chat.Write.IChat, ChatDAL.BusinessRules.Chat.Write.Chat>();
                        //builder.Services.AddScoped<ChatDAL.BusinessRules.Chat.Read.IChat, ChatDAL.BusinessRules.Chat.Read.Chat>();
                        //builder.Services.AddScoped<IChatGPT, ChatGPT>();
                        //builder.Services.AddTransient<Storage>();
                        builder.Services.AddSingleton<Log>();
                        //builder.Services.AddTransient<AppConfigDetails>();
                        // NLog: Setup NLog for Dependency injection
                        builder.Logging.ClearProviders();
                        builder.Host.UseNLog();

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

        private X509Certificate2 FindMatchingCertificateBySubject(string subjectCommonName)
        {
            using (var store = new X509Store(StoreName.My, StoreLocation.LocalMachine))
            {
                store.Open(OpenFlags.OpenExistingOnly | OpenFlags.ReadOnly);
                var certCollection = store.Certificates;
                var matchingCerts = new X509Certificate2Collection();

                foreach (var enumeratedCert in certCollection)
                {
                    if (StringComparer.OrdinalIgnoreCase.Equals(subjectCommonName, enumeratedCert.GetNameInfo(X509NameType.SimpleName, forIssuer: false))
                      && DateTime.Now < enumeratedCert.NotAfter
                      && DateTime.Now >= enumeratedCert.NotBefore)
                    {
                        matchingCerts.Add(enumeratedCert);
                    }
                }

                if (matchingCerts.Count == 0)
                {
                    throw new Exception($"Could not find a match for a certificate with subject 'CN={subjectCommonName}'.");
                }

                return matchingCerts[0];
            }
        }
    }
}
