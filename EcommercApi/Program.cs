
using EcommercApi.Data;  // Add this to reference your EcommerceDbContext
using EcommercApi.Services; 
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi


builder.Services.AddDbContext<EcommerceDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),sqlServerOptionsAction=>sqlServerOptionsAction.EnableRetryOnFailure(maxRetryCount:3,maxRetryDelay:TimeSpan.FromSeconds(2),errorNumbersToAdd:null)));

builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin()  // Allow all origins
                          .AllowAnyHeader()  // Allow any headers
                          .AllowAnyMethod()); //
    });
var jwtSettings = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))
        };
    });

builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    });

builder.Services.AddScoped<IServiceAdministration,ServiceAdministration>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseCors("AllowAll");
app.UseAuthentication(); // Enable JWT Authentication
app.UseAuthorization(); // Enable Authorization

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.Run();
