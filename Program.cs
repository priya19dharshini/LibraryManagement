
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using LibraryManagement.Data;
using LibraryManagement.Helper;
using LibraryManagement.Menus;
using LibraryManagement.Repositories;
using LibraryManagement.Services;


// 1. Create Host Builder
var builder = Host.CreateApplicationBuilder(args);

// 2. Get Connection String
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection");

// 3. Validate Connection String
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "DefaultConnection is missing from appsettings.json");
}

// 4. Register Database Context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

Console.WriteLine("Connection string loaded successfully!");


// 4. Register Repositories
builder.Services.AddScoped<BookRepository>();
builder.Services.AddScoped<MemberRepository>();
builder.Services.AddScoped<LoanRepository>();
builder.Services.AddScoped<AuditLogRepository>();

// 5. Register Services
builder.Services.AddScoped<BookService>();
builder.Services.AddScoped<MemberService>();
builder.Services.AddScoped<LoanService>();
builder.Services.AddScoped<AuditLogService>();

// 6. Register Menus
builder.Services.AddScoped<BookMenu>();
builder.Services.AddScoped<MemberMenu>();
builder.Services.AddScoped<LoanMenu>();

// 7. Build Host
using var host = builder.Build();

// 8. Start Host
await host.StartAsync();

bool running = true;
while (running)
{
    Console.WriteLine("\n===== LIBRARY MANAGEMENT SYSTEM =====");
    Console.WriteLine("1. Book Management");
    Console.WriteLine("2. Member Management");
    Console.WriteLine("3. Loan Management");
    Console.WriteLine("4. View Audit Logs");
    Console.WriteLine("0. Exit");

    if (!int.TryParse(
        InputHelper.ReadRequiredInput("Enter choice: "),
        out int choice))
    {
        Console.WriteLine("Invalid input. Enter a number.");
        continue;
    }

    // CREATE A NEW SCOPE FOR EACH ITERATION
    // This ensures AppDbContext and repositories are freshly created and disposed properly after each action
    using var scope = host.Services.CreateScope();
    var services = scope.ServiceProvider;

    var auditLogService =
    scope.ServiceProvider.GetRequiredService<AuditLogService>();

    switch (choice)
    {
        case 1:
            var bookMenu = services.GetRequiredService<BookMenu>();
            await bookMenu.RunAsync();
            break;

        case 2:
            var memberMenu = services.GetRequiredService<MemberMenu>();
            await memberMenu.RunAsync();
            break;

        case 3:
            var loanMenu = services.GetRequiredService<LoanMenu>();
            await loanMenu.RunAsync();
            break;

        case 4:
            var auditLogs = await auditLogService.GetAllLogsAsync();

            Console.WriteLine("\n===== AUDIT LOGS =====");

            foreach (var log in auditLogs)
            {
                Console.WriteLine($"Audit ID    : {log.AuditLogId}");
                Console.WriteLine($"Action      : {log.Action}");
                Console.WriteLine($"Entity      : {log.EntityName}");
                Console.WriteLine($"Entity ID   : {log.EntityId}");
                Console.WriteLine($"Description : {log.Description}");
                Console.WriteLine($"Performed By: {log.PerformedBy}");
                Console.WriteLine($"Date        : {log.PerformedAt}");
                Console.WriteLine("--------------------------------");
            }

            if (auditLogs.Count == 0)
            {
                Console.WriteLine("No audit logs found.");
            }

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

// Stop host gracefully on exit
await host.StopAsync();