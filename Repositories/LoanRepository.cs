
using LibraryManagement.Data;
using LibraryManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Repositories;

public class LoanRepository
{
    private readonly AppDbContext _db;

    public LoanRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<int> AddAsync(Loan loan)
    {
       
        await using var transaction =
            await _db.Database.BeginTransactionAsync();

        var book = await _db.Books
            .FirstOrDefaultAsync(b => b.BookId == loan.BookId);

        if (book == null)
            throw new InvalidOperationException("Book not found.");

        var member = await _db.Members
            .FirstOrDefaultAsync(m => m.MemberId == loan.MemberId);

        if (member == null)
            throw new InvalidOperationException("Member not found.");

        if (!member.IsActive)
            throw new InvalidOperationException(
                "This member is inactive and cannot borrow books.");

        if (book.AvailableCopies <= 0)
            throw new InvalidOperationException(
                "No copies of this book are currently available.");

        book.AvailableCopies--;

        loan.Book = book;
        loan.Member = member;
        loan.ReturnDate = null;

        _db.Loans.Add(loan);

        await _db.SaveChangesAsync();
        await transaction.CommitAsync();

        return loan.LoanId;
    }

    public async Task<List<Loan>> GetAllAsync()
    {

        return await _db.Loans
            .AsNoTracking()
            .Include(l => l.Book)
            .Include(l => l.Member)
            .OrderByDescending(l => l.LoanDate)
            .ToListAsync();
    }

    public async Task<Loan?> GetByIdAsync(int id)
    {
       
        return await _db.Loans
            .AsNoTracking()
            .Include(l => l.Book)
            .Include(l => l.Member)
            .FirstOrDefaultAsync(l => l.LoanId == id);
    }

    public async Task<bool> ReturnAsync(int id)
    {
        await using var transaction =
            await _db.Database.BeginTransactionAsync();

        var loan = await _db.Loans
            .Include(l => l.Book)
            .FirstOrDefaultAsync(l => l.LoanId == id);

        if (loan == null)
            return false;

        if (loan.ReturnDate.HasValue)
            throw new InvalidOperationException(
                "This loan has already been returned.");

        if (loan.Book == null)
            throw new InvalidOperationException(
                "The associated book could not be found.");

        if (loan.Book.AvailableCopies >= loan.Book.TotalCopies)
            throw new InvalidOperationException(
                "Available copies cannot exceed total copies.");

        loan.ReturnDate = DateTime.Now;
        loan.Book.AvailableCopies++;

        await _db.SaveChangesAsync();
        await transaction.CommitAsync();

        return true;
    }
}
