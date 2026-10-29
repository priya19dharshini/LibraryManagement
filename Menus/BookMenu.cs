
using LibraryManagement.Helper;
using LibraryManagement.Models;
using LibraryManagement.Services;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Menus;

public class BookMenu
{
    private readonly BookService _service;

    public BookMenu(BookService service)
    {
        _service = service;
    }

    public async Task RunAsync()
    {
        bool running = true;

        while (running)
        {
            Console.WriteLine("\n===== BOOK MENU =====");
            Console.WriteLine("1. Add Book");
            Console.WriteLine("2. View All Books");
            Console.WriteLine("3. View Book By ID");
            Console.WriteLine("4. Update Book");
            Console.WriteLine("5. Delete Book");
            Console.WriteLine("0. Exit");

            if (!int.TryParse(
                InputHelper.ReadRequiredInput("Enter choice: "),
                out int choice))
            {
                Console.WriteLine("Invalid input. Enter a number.");
                continue;
            }

            try
            {
                switch (choice)
                {
                    case 1:
                        await AddBookAsync();
                        break;
                    case 2:
                        await ViewAllAsync();
                        break;
                    case 3:
                        await ViewByIdAsync();
                        break;
                    case 4:
                        await UpdateBookAsync();
                        break;
                    case 5:
                        await DeleteBookAsync();
                        break;
                    case 0:
                        running = false;
                        Console.WriteLine("Exiting book menu...");
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Try again.");
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

    private async Task AddBookAsync()
    {
        string title = InputHelper.ReadRequiredInput("Title: ");
        string author = InputHelper.ReadRequiredInput("Author: ");
        string isbn = InputHelper.ReadRequiredInput("ISBN: ");
        int totalCopies = InputHelper.ReadInt("Total copies: ");

        var book = new Book
        {
            Title = title,
            Author = author,
            Isbn = isbn,
            TotalCopies = totalCopies,
            AvailableCopies = totalCopies
        };

        int id = await _service.AddAsync(book);

        Console.WriteLine($"Book added successfully. ID: {id}");
    }

    private async Task ViewAllAsync()
    {
        var books = await _service.GetAllAsync();

        if (books.Count == 0)
        {
            Console.WriteLine("No books found.");
            return;
        }

        foreach (var book in books)
            DisplayBook(book);
    }

    private async Task ViewByIdAsync()
    {
        int id = InputHelper.ReadInt("Book ID: ", 1);
        var book = await _service.GetByIdAsync(id);

        if (book == null)
        {
            Console.WriteLine("Book not found.");
            return;
        }

        DisplayBook(book);
    }

    private async Task UpdateBookAsync()
    {
        int id = InputHelper.ReadInt("Book ID to update: ", 1);
        var existing = await _service.GetByIdAsync(id);

        if (existing == null)
        {
            Console.WriteLine("Book not found.");
            return;
        }

        Console.WriteLine("Press Enter to keep the existing value.");

        Console.Write($"Title ({existing.Title}): ");
        string? title = Console.ReadLine();

        Console.Write($"Author ({existing.Author}): ");
        string? author = Console.ReadLine();

        Console.Write($"ISBN ({existing.Isbn}): ");
        string? isbn = Console.ReadLine();

        int totalCopies = InputHelper.ReadOptionalInt(
            $"Total copies ({existing.TotalCopies}): ",
            existing.TotalCopies);

        var updatedBook = new Book
        {
            BookId = id,
            Title = string.IsNullOrWhiteSpace(title)
                ? existing.Title : title.Trim(),
            Author = string.IsNullOrWhiteSpace(author)
                ? existing.Author : author.Trim(),
            Isbn = string.IsNullOrWhiteSpace(isbn)
                ? existing.Isbn : isbn.Trim(),
            TotalCopies = totalCopies
        };

        bool updated = await _service.UpdateAsync(id, updatedBook);

        Console.WriteLine(updated
            ? "Book updated successfully."
            : "Book not found.");
    }

    private async Task DeleteBookAsync()
    {
        int id = InputHelper.ReadInt("Book ID to delete: ", 1);
        var book = await _service.GetByIdAsync(id);

        if (book == null)
        {
            Console.WriteLine("Book not found.");
            return;
        }

        DisplayBook(book);

        string confirmation = InputHelper.ReadRequiredInput(
            "Confirm deletion? (y/n): ");

        if (!confirmation.Equals(
            "y", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Deletion cancelled.");
            return;
        }

        bool deleted = await _service.DeleteAsync(id);

        Console.WriteLine(deleted
            ? "Book deleted successfully."
            : "Book not found.");
    }

    private static void DisplayBook(Book book)
    {
        Console.WriteLine($"""
            
            ----------------------------
            ID: {book.BookId}
            Title: {book.Title}
            Author: {book.Author}
            ISBN: {book.Isbn}
            Total Copies: {book.TotalCopies}
            Available Copies: {book.AvailableCopies}
            ----------------------------
            """);
    }
}
