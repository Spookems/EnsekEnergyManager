using EnsekEnergyManager.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<AccountContext>(options =>
{
    // 11:30 https://www.youtube.com/watch?v=ZX12X-ALwGA
    options.UseSqlServer("Data Source=Main-01\\ENTEKSERVER;Initial Catalog=ENSEKDB;Integrated Security=True;Trust Server Certificate=True");
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.Run();
