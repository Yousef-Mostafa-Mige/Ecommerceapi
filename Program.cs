using Ecommerceapi.Data;
using Ecommerceapi.services.ProductServices;
using Ecommerceapi.services.UserServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

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