
using BLL.Hubs;
using BLL.Interface;
using BLL.IService;
using BLL.Service;
using BLL.Services;
using BLL.Services.BLL.Services;
using Common.Config;
using DAL.Data;
using DAL.IRepositories;
using DAL.Repositories;
using DAL.UnitOfWork;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using Service.IService;
using Service.Service;

namespace SpeakAI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            })
    .AddCookie();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IValidationHandleService, ValidationHandleService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<ICourseService, CourseService>();
            builder.Services.AddScoped<ILoginService, LoginService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddSingleton<IAIService, AIService>();
            builder.Services.AddScoped<IPremiumSubscriptionService, PremiumSubscriptionService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<ITransactionService, TransactionService>();
            builder.Services.AddScoped<IValidationHandleService, ValidationHandleService>();
            builder.Services.AddScoped<IVnPayService, VnPayService>();
            builder.Services.AddScoped<IVoucherService, VoucherService>();
            builder.Services.AddHostedService<VoucherBackgroundService>();
            builder.Services.AddLogging();




            builder.Services.AddScoped<IPaymentService,PaymentService>();
            builder.Services.AddScoped<ITransactionService,TransactionService>();
            builder.Services.AddScoped<IValidationHandleService,ValidationHandleService>();
            builder.Services.AddScoped<IVnPayService, VnPayService>();
            builder.Services.AddScoped<IGoogleAuthService, GoogleAuthService>();
            builder.Services.AddScoped<IJwtProvider, JwtProvider>();
            builder.Services.Configure<GoogleAuthConfig>(builder.Configuration.GetSection("Google"));

            builder.Services.AddScoped<ChatHub>();
            builder.Services.AddSignalR();
            builder.Services.AddMemoryCache();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    policy =>
                    {
                        policy.AllowAnyOrigin()
                              .AllowAnyMethod()
                              .AllowAnyHeader();
                    });
            });

            //builder.Services.AddDbContext<SpeakAIContext>(options =>
            //options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<SpeakAIContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), b => b.MigrationsAssembly("DAL")));
            var app = builder.Build();

        

            app.UseSwagger();
            app.UseSwaggerUI();


            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.MapHub<ChatHub>("/chatHub");

            app.MapControllers();
            app.UseCors("AllowAll");
            app.Run();
        }
    }
}
