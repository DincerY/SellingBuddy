using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;

var builder = WebApplication.CreateBuilder(args);


builder.Configuration.AddJsonFile("ocelot.json").AddEnvironmentVariables();

builder.Services.AddOcelot().AddConsul();
// Add services to the container.

builder.Services.AddControllers();
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

await app.UseOcelot();

app.Run();
