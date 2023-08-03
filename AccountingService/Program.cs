
using Microsoft.EntityFrameworkCore;
using AccountingService.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(o =>
{
    o.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            context.Response.StatusCode = 401; 
            context.Response.ContentType = "text/plain";
            return context.Response.WriteAsync("Neautoriziran pristup"); 
        }
    };
    var Key = Encoding.UTF8.GetBytes("Jwt:key");
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "Jwt:Issuer",
        ValidAudience = "Jwt:Audience",
        IssuerSigningKey = new SymmetricSecurityKey(Key)
    };
});
builder.Services.AddMvc();
builder.Services.AddRazorPages();
builder.Services.AddControllers();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Dodajte Swagger definiciju
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "My API", Version = "v1" });

    // Dodajte JWT token shemu
    var securityScheme = new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter 'Bearer' followed by space and then your valid JWT token",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    };
    c.AddSecurityDefinition("Bearer", securityScheme);

    // Definirajte zahtjev za JWT token
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

builder.Services.AddDbContext<SalaryConversionContext>(opt => {
    var conn = "Server=localhost;Database=salaryconversion;Uid=root;Pwd=root;";
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
        // Dodajte ovu liniju kako biste uključili "Authorize" gumb u Swagger UI
        c.OAuthClientId("swagger");
        c.OAuthAppName("My Swagger UI");
    });
}

app.UseHttpsRedirection();
app.MapControllers();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapRazorPages();
});

app.Run();


