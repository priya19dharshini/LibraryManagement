
using LibraryManagement.Data;
using LibraryManagement.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Repositories;

public class BookRepository
{

    private readonly AppDbContext _db;

    public BookRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<int> AddAsync(Book book)
    {
       
        _db.Books.Add(book);

        try
        {
            await _db.SaveChangesAsync();
            return book.BookId;
        }
        catch (DbUpdateException ex)
            when (ex.InnerException is SqlException sqlEx
                && (sqlEx.Number == 2601 || sqlEx.Number == 2627))
        {
            throw new InvalidOperationException(
                "A book with this ISBN already exists.", ex);
        }
    }

    public async Task<List<Book>> GetAllAsync()
    {
       
        return await _db.Books
            .AsNoTracking()
            .OrderBy(b => b.BookId)
            .ToListAsync();
    }

    public async Task<Book?> GetByIdAsync(int id)
    {

        return await _db.Books
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.BookId == id);
    }

    public async Task<bool> UpdateAsync(int id, Book updatedBook)
    {
       
        var book = await _db.Books.FindAsync(id);

        if (book == null)
            return false;

        int onLoan = book.TotalCopies - book.AvailableCopies;

        if (updatedBook.TotalCopies < onLoan)
        {
            throw new InvalidOperationException(
                $"{onLoan} copies are on loan. " +
                "Total copies cannot be lower than that.");
        }

        book.Title = updatedBook.Title;
        book.Author = updatedBook.Author;
        book.Isbn = updatedBook.Isbn;
        book.TotalCopies = updatedBook.TotalCopies;
        book.AvailableCopies = updatedBook.TotalCopies - onLoan;

        try
        {
            await _db.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException ex)
            when (ex.InnerException is SqlException sqlEx
                && (sqlEx.Number == 2601 || sqlEx.Number == 2627))
        {
            throw new InvalidOperationException(
                "A book with this ISBN already exists.", ex);
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
       
        var book = await _db.Books.FindAsync(id);

        if (book == null)
            return false;

        bool hasLoans = await _db.Loans
            .AnyAsync(l => l.BookId == id);

        if (hasLoans)
        {
            throw new InvalidOperationException(
                "Cannot delete this book because loan records exist.");
        }

        _db.Books.Remove(book);
        await _db.SaveChangesAsync();

        return true;
    }
}
