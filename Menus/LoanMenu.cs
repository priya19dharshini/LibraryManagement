
using LibraryManagement.Helper;
using LibraryManagement.Models;
using LibraryManagement.Services;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Menus;

public class LoanMenu
{
    private readonly LoanService _service;

    public LoanMenu(LoanService service)
    {
        _service = service;
    }

    public async Task RunAsync()
    {
        bool running = true;

        while (running)
        {
            Console.WriteLine("\n===== LOAN MENU =====");
            Console.WriteLine("1. Issue Book");
            Console.WriteLine("2. View All Loans");
            Console.WriteLine("3. View Loan By ID");
            Console.WriteLine("4. Return Book");
            Console.WriteLine("0. Back");

            if (!int.TryParse(
                InputHelper.ReadRequiredInput("Enter choice: "),
                out int choice))
            {
                Console.WriteLine("Invalid input.");
                continue;
            }

            try
            {
                switch (choice)
                {
                    case 1:
                        await IssueAsync();
                        break;
                    case 2:
                        await ViewAllAsync();
                        break;
                    case 3:
                        await ViewByIdAsync();
                        break;
                    case 4:
                        await ReturnAsync();
                        break;
                    case 0:
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch (DbUpdateException)
            {
                Console.WriteLine(
                    "Database operation failed. Check database constraints.");
            }
        }
    }

    private async Task IssueAsync()
    {
        int bookId = InputHelper.ReadInt("Book ID: ", 1);
        int memberId = InputHelper.ReadInt("Member ID: ", 1);

        DateTime loanDate = DateTime.Today;

        DateTime dueDate;

        while (true)
        {
            string input = InputHelper.ReadRequiredInput(
                "Due date (yyyy-MM-dd): ");

            if (DateTime.TryParseExact(
                input,
                "yyyy-MM-dd",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out dueDate)
                && dueDate.Date >= loanDate)
            {
                break;
            }

            Console.WriteLine(
                "Enter a valid due date on or after today (yyyy-MM-dd).");
        }

        var loan = new Loan
        {
            BookId = bookId,
            MemberId = memberId,
            LoanDate = loanDate,
            DueDate = dueDate.Date,
            ReturnDate = null
        };

        int id = await _service.AddAsync(loan);

        Console.WriteLine($"Book issued successfully. Loan ID: {id}");
    }

    private async Task ViewAllAsync()
    {
        var loans = await _service.GetAllAsync();

        if (loans.Count == 0)
        {
            Console.WriteLine("No loan records found.");
            return;
        }

        foreach (var loan in loans)
            Display(loan);
    }

    private async Task ViewByIdAsync()
    {
        int id = InputHelper.ReadInt("Loan ID: ", 1);
        var loan = await _service.GetByIdAsync(id);

        if (loan == null)
        {
            Console.WriteLine("Loan not found.");
            return;
        }

        Display(loan);
    }

    private async Task ReturnAsync()
    {
        int id = InputHelper.ReadInt("Loan ID to return: ", 1);

        var loan = await _service.GetByIdAsync(id);

        if (loan == null)
        {
            Console.WriteLine("Loan not found.");
            return;
        }

        if (loan.ReturnDate.HasValue)
        {
            Console.WriteLine("This book has already been returned.");
            return;
        }

        Display(loan);

        string confirm = InputHelper.ReadRequiredInput(
            "Confirm book return? (y/n): ");

        if (!confirm.Equals("y", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Return cancelled.");
            return;
        }

        bool result = await _service.ReturnAsync(id);

        Console.WriteLine(result
            ? "Book returned successfully."
            : "Loan not found.");
    }

    private static void Display(Loan loan)
    {
        Console.WriteLine($"""
            
            ----------------------------
            Loan ID: {loan.LoanId}
            Book ID: {loan.BookId}
            Book Title: {loan.Book?.Title ?? "Unknown"}
            Member ID: {loan.MemberId}
            Member Name: {loan.Member?.FullName ?? "Unknown"}
            Loan Date: {loan.LoanDate:yyyy-MM-dd}
            Due Date: {loan.DueDate:yyyy-MM-dd}
            Return Date: {(loan.ReturnDate.HasValue
                ? loan.ReturnDate.Value.ToString("yyyy-MM-dd")
                : "Not returned")}
            Status: {(loan.ReturnDate.HasValue
                ? "Returned"
                : "On loan")}
            ----------------------------
            """);
    }
}
