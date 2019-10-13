using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using StockTrader.Service.InfrastructureService;
using StockTrader.Service.DataService;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using StockTrader.StockTrader_Model;
using System.Reflection;
using NSwag.AspNetCore;



namespace StockTrader
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var configurationBuilder = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json")
               .AddEnvironmentVariables();
            Configuration = configurationBuilder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddControllersAsServices();
            //services.AddDbContext<StockTraderContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()));
            //var connection = @"Server=HQ-IT-19116\SQLSERVERDEV;Database=StockTrader;User Id=kinso; Password=kinso;ConnectRetryCount=5";
            var connection = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value;
            services.AddDbContext<StockTraderContext>(options => options.UseSqlServer(connection));
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IStockService, StockService>();
            services.AddSingleton<ITransactionService, TransactionService>();
            services.AddSingleton<IStockHelperService, StockHelperService>();
            services.AddAuthorization(options =>
            { options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme).RequireAuthenticatedUser().Build(); });

            // Register the Swagger services
            services.AddSwaggerDocument(config =>
            {
                config.PostProcess = document =>
                {
                    document.Info.Version = "v1";
                    document.Info.Title = "Decagon Stock Trader App";
                    document.Info.Description = "An API to support any Stock Trading Client App";
                    document.Info.TermsOfService = "None";
                    document.Info.Contact = new NSwag.OpenApiContact
                    {
                        Name = "Kingsley Okei",
                        Email = "kingsleycokei@gmail.com",
                        Url = "https://twitter.com/kingsleycokei"
                    };
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //Register the NSwag Middleware for API Documentation
            //app.UseSwaggerUi(typeof(Startup).GetTypeInfo().Assembly, new SwaggerUiOwinSettings());

            //I'm using this here because my project currently targets .Net Core 1.1 due to my VStudio 2017 Community Edition's issue
            //In Core 2.x, this would be replaced with app.UseAuthentication().
            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                AuthenticationScheme = JwtBearerDefaults.AuthenticationScheme,
                SaveToken = true,
                RequireHttpsMetadata = false,
                TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = "http://kingsleycokei.com",
                    ValidateAudience = true,
                    ValidAudience = "http://decagonhq.com",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetSection("jwtSecret").GetSection("secret").Value))
                }
            });

            app.UseMvc();

            // Register the Swagger generator and the Swagger UI middlewares
            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("MVC not found");
            });
        }
    }
}
