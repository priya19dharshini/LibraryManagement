
using LibraryManagement.Models;
using LibraryManagement.Repositories;

namespace LibraryManagement.Services;

public class BookService
{
    private readonly BookRepository _repository;

    public BookService(BookRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> AddAsync(Book book)
    {
        ValidateBook(book);

        if (book.TotalCopies < 0)
        {
            throw new InvalidOperationException(
                "Total copies cannot be negative.");
        }

        book.AvailableCopies = book.TotalCopies;

        return await _repository.AddAsync(book);
    }

    public async Task<List<Book>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Book?> GetByIdAsync(int id)
    {
        ValidateId(id);
        return await _repository.GetByIdAsync(id);
    }

    public async Task<bool> UpdateAsync(int id, Book updatedBook)
    {
        ValidateId(id);
        ValidateBook(updatedBook);

        if (updatedBook.TotalCopies < 0)
        {
            throw new InvalidOperationException(
                "Total copies cannot be negative.");
        }

        return await _repository.UpdateAsync(id, updatedBook);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        ValidateId(id);
        return await _repository.DeleteAsync(id);
    }

    private static void ValidateId(int id)
    {
        if (id <= 0)
        {
            throw new InvalidOperationException(
                "Book ID must be greater than zero.");
        }
    }

    private static void ValidateBook(Book book)
    {
        ArgumentNullException.ThrowIfNull(book);

        if (string.IsNullOrWhiteSpace(book.Title))
        {
            throw new InvalidOperationException(
                "Book title is required.");
        }

        if (string.IsNullOrWhiteSpace(book.Author))
        {
            throw new InvalidOperationException(
                "Book author is required.");
        }

        if (string.IsNullOrWhiteSpace(book.Isbn))
        {
            throw new InvalidOperationException(
                "ISBN is required.");
        }
    }
}
