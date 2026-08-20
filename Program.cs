using LibraryApi.Data;
using LibraryApi.Exceptions;
using LibraryApi.Services;
using LibraryApi.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddProblemDetails();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddIdentityCore<ApplicationUser>().AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DatabaseURL")
    )
);

builder.Services.AddScoped<IBookService, BookService>();

var app = builder.Build();

app.UseExceptionHandler();

app.UseExceptionHandler();

app.MapControllers();

app.Run();