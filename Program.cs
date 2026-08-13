using Ecommerceapi.Data;
using Ecommerceapi.services.CategoryService;
using Ecommerceapi.services.OrderServices;
using Ecommerceapi.services.ProductServices;
using Ecommerceapi.services.UserServices;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddFluentValidationAutoValidation();

// 3. مسح وتسجيل كافة الـ Validators الموجودة في المشروع أوتوماتيكياً
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddAuthentication(
    JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer =
                    builder.Configuration["AppSettings:Issuer"],

                ValidAudience =
                    builder.Configuration["AppSettings:Audience"],

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            builder.Configuration[
                                "AppSettings:SecretKey"]!
                        )
                    )
            };
    });

builder.Services.AddAuthorization();

builder.Services.AddOpenApi();

builder.Services.AddScoped<IUserServices, UserServices>();

builder.Services.AddScoped<IProductServices, ProductServices>();
builder.Services.AddScoped<IcategoryServices, CategoryServices>();
builder.Services.AddScoped<IOrderServices, OrderServices>();

builder.Services.AddDbContext<AppDBContext>(options =>
    options.UseMySQL(
        builder.Configuration.GetConnectionString(
            "DefaultConnection")!
    ));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();