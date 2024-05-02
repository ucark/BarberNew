using Barber.BarberDB;
using Barber.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace Barber
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
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder =>
                    {
                        builder
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowAnyOrigin();
                    });
            });

            // JWT servislerinin ve ayarlarının ekleme
            var jwtSettings = Configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];
            services.AddScoped<TokenService>(provider => new TokenService(secretKey));
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "My Barber App",
                        ValidAudience = "API Servers",
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                    };
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireBarberLoggedIn", policy =>
                {
                    policy.RequireClaim("User Role", "Barber");
                });

                options.AddPolicy("RequireCustomerLoggedIn", policy =>
                {
                    policy.RequireClaim("User Role", "Customer");
                });
            });

            services.AddDbContext<BarberDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Barber API", Version = "v1" });
            });

            services.AddTransient<BarberManager>();
            services.AddTransient<CustomerManager>();
            services.AddTransient<EmployeeRegister>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, BarberManager barberManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseCors("AllowSpecificOrigin");
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Barber v1");
                });
            }
            else
            {
                app.UseExceptionHandler("/error"); // Hata yönetimi middleware'i
                app.UseHsts(); // HTTP Strict Transport Security
                app.UseHttpsRedirection(); // HTTPS'e yönlendirme
                app.UseCors("AllowSpecificOrigin"); // Prodüksiyon ortamında CORS tanımlaması
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
