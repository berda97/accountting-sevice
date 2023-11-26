
using Microsoft.EntityFrameworkCore;
using AccountingService.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using AccountingService.Services;
using AccountingService.Interface;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(o =>
{
    var Key = Encoding.UTF8.GetBytes(builder.Configuration["JwtConfig:Key"]);
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtConfig:Issuer"],
        ValidAudience = builder.Configuration["JwtConfig:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Key)
    };
});

builder.Services.AddScoped<JwtTokenService>(); //addscoped tip registracije
builder.Services.AddScoped<ClaimsService>();
builder.Services.AddScoped<NetSalaryService>();
builder.Services.AddScoped<ExchangeRateService>();
builder.Services.AddScoped<ITimeService>(provider =>
{
    int minutes = builder.Configuration.GetValue<int>("JwtConfig:ExpiryInMinutes");
    return new TimeService(minutes);
});


builder.Services.AddControllers();
builder.Services.AddCors();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    // Add a Swagger definition
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "My API", Version = "v1" });

    // Add a JWT token scheme
    var securityScheme = new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter 'Bearer' followed by space and then your valid JWT token",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    };

    c.AddSecurityDefinition("Bearer", securityScheme);
    // Define a request for a JWT token
    var securityRequirement = new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new List<string>()
        }
    };
    c.AddSecurityRequirement(securityRequirement);
});

var conn = builder.Configuration.GetConnectionString("SalaryConversionContext");
builder.Services.AddDbContext<SalaryConversionContext>(opt => {
    opt.UseMySql(conn, ServerVersion.AutoDetect(conn));
});

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        // Add this line to include the "Authorize" button in the Swagger UI
        c.OAuthClientId("swagger");
        c.OAuthAppName("My Swagger UI");
    });
}
app.UseHttpsRedirection();
app.MapControllers();
app.UseRouting();
app.UseCors(o=>o.WithOrigins("https://localhost:4200", "http://localhost:4200")
                .AllowAnyMethod()
                .AllowAnyHeader());

app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();


