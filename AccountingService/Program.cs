
using Microsoft.EntityFrameworkCore;
using AccountingService.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
 

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    
    options.AddPolicy(name: "CorsOriginPolicy",
                      builder =>
                      {
                          builder.WithOrigins("https://localhost:4200", "http://localhost:4200");
                          
                           

                      });
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<SalaryConversionContext>(opt => {
    var conn = "Server=localhost;Database=salaryconversion;Uid=root;Pwd=root;";


    opt.UseMySql(conn, ServerVersion.AutoDetect(conn));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseCors();

app.Run();

app.UseAuthentication();
app.UseRouting();
app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });




