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
using System;
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

            services.AddDbContext<BarberDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    //Yapı tanımlama
                    ValidateAudience = true, //izin verilen siteler denetlensin mi? true = evet, denetlenecek.
                    ValidateIssuer = true, //Bu ayar, gelen JWT'nin ihraç edenin (issuer) doğrulanıp doğrulanmayacağını belirler.
                    ValidateLifetime = true, //token yaşam süresi olsun mu? true = evet.
                    ValidateIssuerSigningKey = true, //token bize ait mi? evet. kontrol ediliyor bu yapı sayesinde.
                    //Değer atamaları
                    ValidIssuer = Configuration["Token: Issuer"],
                    ValidAudience = Configuration["Token: Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Token: SecurityKey"])), //security keyi byte çeviriyoruz. simetrik key aracılığıyla.
                    ClockSkew = TimeSpan.Zero //sunucular arası zaman farkı olursa bu farkı kapatmak istiyoruz. Süre eklemek için önce 
                    //json dosyasına gidip Expiration: 10 gibi bir süre eklemek gerekiyor.
                };
            });
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
            if (env.IsDevelopment()) //=>middleware = orta katman
            //ENVIRONMENT: ortam
            //IsDevelopment: uygulamanın geliştirme ortamında çalışıp çalışmadığını kontrol eder. bu durumda kodlar çalışır.
            {
                app.UseDeveloperExceptionPage(); //oluşan hataları daha ayrıntılı görmeyi, bu sayede hata ayıklama işlemlerini kolaylaştırır.
                app.UseSwagger(); //Swagger, API dökümantasyonunu otomatik oluşturur.Bu sayede API'yi kullanacak diğer geliştiricilerin API'yi nasıl kullanacaklarını anlamalarını kolaylaştırır.
                app.UseCors("AllowSpecificOrigin"); //bir nevi güvenlik duvarı. Originden(kaynaktan) diğerine HTTP isteklerini sınırlar.

                app.UseSwaggerUI(c =>//Swagger UI(user interface)'nin kullanıcı arayüzünü sağlar. Bu arayüz, belirli bir adres üzerinden API dokümantasyonunu görüntülememizi sağlar. 
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Barber v1"); //Swagger UI'nin /swagger/v1/swagger.json adresine API dokümantasyonunu yüklemesini ve "API Adı v1" başlığı altında göstermesini sağlar.
                });
            }

            //app.UseAuthentication();//Yaptığım tokeni uygulamaya bildirmem gerekiyor.
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
