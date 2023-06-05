using BooksBot.API.Configurations;
using BooksBot.API.Data;
using BooksBot.API.Data.Repositories;
using BooksBot.API.Middlewares;
using BooksBot.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace BooksBot.API
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
            services.AddScoped<IScraperService, ScraperService>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IBookDataService, BookDataService>();
            services.AddScoped<IDataImportService, DataImportService>();
            services.AddDistributedMemoryCache();
            services.AddAutoMapper(typeof(Startup));
            services.AddControllers();
            services.AddSingleton(Configuration.GetSection("AppConfiguration").Get<AppConfiguration>());
            services.AddSingleton(Configuration.GetSection("JwtSettings").Get<JwtSettings>());

            AddRepositories(services);
            services.AddCors(options =>
            {
                options.AddPolicy(
                                name: "AllowOrigin",
                                builder => { builder.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin(); });
            });


            services.AddIdentity<IdentityUser, IdentityRole>(options => { options.SignIn.RequireConfirmedAccount = false; })
                    .AddEntityFrameworkStores<BooksBotContext>()
                    .AddDefaultTokenProviders();

            // Adding Authorization
            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
            }
            );
            // Adding Authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            // Adding Jwt Bearer
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = Configuration["JwtSettings:Issuer"],
                    ValidAudience = Configuration["JwtSettings:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtSettings:Key"]))
                };
            });


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "BooksBot.API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });

            services.AddDbContext<BooksBotContext>(item =>
                            item.UseLazyLoadingProxies()
                            .UseSqlServer(Configuration.GetConnectionString("BooksBotConnectionString"))
                        ).AddTransient<IDatabaseSeeder, DatabaseSeeder>();
        }

        public static void AddRepositories(IServiceCollection services)
        {
            services.AddTransient(typeof(IRepositoryAsync<>), typeof(RepositoryAsync<>));
            services.AddTransient<IBookDataRepository, BookDataRepository>();
            services.AddTransient<ICrawlSourceRepository, CrawlSourceRepository>();
            services.AddTransient<IShouldCrawlEanRepository, ShouldCrawlEanRepository>();
            services.AddTransient<ISourceWebsiteDataRepository, SourceWebsiteDataRepository>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.ConfigObject.AdditionalItems["syntaxHighlight"] = new Dictionary<string, object> { ["activated"] = false };
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "BooksBot.API v1");
            });

            app.UseMiddleware<ExceptionHandlerMiddleware>();

            app.UseRouting();

            app.UseAuthorization();
            app.UseAuthentication();

            app.UseCors("AllowOrigin");
            app.UseHttpsRedirection();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            Initialize(app);

        }

        private IApplicationBuilder Initialize(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();

            var initializers = serviceScope.ServiceProvider.GetServices<IDatabaseSeeder>();

            foreach (var initializer in initializers)
            {
                initializer.Initialize();
            }

            return app;
        }
    }
}
