using LibraryApi.Data;
using LibraryApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DatabaseURL")
    )
);

builder.Services.AddScoped<BookService>();

var app = builder.Build();

app.MapControllers();

app.Run();