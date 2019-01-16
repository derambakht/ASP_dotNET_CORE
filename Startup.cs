using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SampleApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            #region Add DbContext

            services.AddDbContext<SampleDbContext>(options =>
               options.UseSqlServer(Configuration.GetConnectionString("DbConnection")));

            #endregion

            #region Add MemoryCache

            services.AddMemoryCache();

            #endregion

            #region Add Cors To Services

            services.AddCors(o => o.AddPolicy("APIPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));

            #endregion

            #region Add Swagger To Services

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "سرویس های سامانه",
                    Description = "سرویس های مورد نیاز برای سامانه",
                    Contact = new Swashbuckle.AspNetCore.Swagger.Contact
                    {
                        Name = "Sample API",
                        Email = "derambakht@gmail.com",
                        Url = "https://www.cvbuilder.me/"
                    }
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            #endregion

            #region Read key from AppSettting & Add to Services.AppSettings

            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("AppSetting");
            services.Configure<AppSetting>(appSettingsSection);
            Common.GlobalStaticData.PageItemCount = appSettingsSection.GetValue<int>("PageItemCount");

            #endregion

            #region Configure JWT Authentication

            var appSettings = appSettingsSection.Get<AppSetting>();
            var key = Encoding.UTF8.GetBytes(appSettings.Secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            #endregion

            #region Add Register DI Serices

            // configure DI for application services
            services.AddScoped(typeof(IAsyncRepository<>), typeof(GenericRepository<>));
            services.AddScoped(typeof(IUserService), typeof(UserService));

            #endregion

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            loggerFactory.AddFile("Logs/api-{Date}.txt");

            app.UseCors("APIPolicy");

            #region register Swagger

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Leasing API");
            });

            #endregion

            app.UseAuthentication();

            app.UseStaticFiles();
            app.UseHttpsRedirection();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}");
            });
        }
    }
}
