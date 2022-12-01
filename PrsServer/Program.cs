using Microsoft.EntityFrameworkCore;
using PrsServer.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<PrsDbContext>(x => {
    string ConnectionKey = "Dev";

#if RELEASE
    ConnectionKey = "Prod";
#endif 

    x.UseSqlServer(builder.Configuration.GetConnectionString(ConnectionKey));
});

builder.Services.AddCors();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseCors(x => {
    x.AllowAnyOrigin()
    .AllowAnyHeader()
    .AllowAnyMethod();
});

app.UseAuthorization();

app.MapControllers();

using var scope = app.Services
                    .GetRequiredService<IServiceScopeFactory>()
                    .CreateScope();
scope.ServiceProvider
        .GetService<PrsDbContext>()!
        .Database.Migrate();

app.Run();
