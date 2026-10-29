using LibraryManagement.Data;
using LibraryManagement.Menus;
using LibraryManagement.Repositories;
using LibraryManagement.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var services = new ServiceCollection();

services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        configuration.GetConnectionString("DefaultConnection")));

services.AddScoped<BookRepository>();
services.AddScoped<MemberRepository>();
services.AddScoped<LoanRepository>();

services.AddScoped<BookService>();
services.AddScoped<MemberService>();
services.AddScoped<LoanService>();

services.AddScoped<BookMenu>();
services.AddScoped<MemberMenu>();
services.AddScoped<LoanMenu>();

await using var provider = services.BuildServiceProvider();
await using var scope = provider.CreateAsyncScope();

var bookMenu = scope.ServiceProvider.GetRequiredService<BookMenu>();
var memberMenu = scope.ServiceProvider.GetRequiredService<MemberMenu>();
var loanMenu = scope.ServiceProvider.GetRequiredService<LoanMenu>();
bool running = true;

while (running)
{
    Console.WriteLine("\n===== LIBRARY MANAGEMENT SYSTEM =====");
    Console.WriteLine("1. Book Management");
    Console.WriteLine("2. Member Management");
    Console.WriteLine("3. Loan Management");
    Console.WriteLine("0. Exit");

    if (!int.TryParse(
        LibraryManagement.Helper.InputHelper
            .ReadRequiredInput("Enter choice: "),
        out int choice))
    {
        Console.WriteLine("Invalid input. Enter a number.");
        continue;
    }

    switch (choice)
    {
        case 1:
            await bookMenu.RunAsync();
            break;

        case 2:
            await memberMenu.RunAsync();
            break;

        case 3:
            await loanMenu.RunAsync();
            break;

        case 0:
            running = false;
            Console.WriteLine("Exiting Library Management System...");
            break;

        default:
            Console.WriteLine("Invalid choice. Try again.");
            break;
    }
}
