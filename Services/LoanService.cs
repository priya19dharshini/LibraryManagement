
using LibraryManagement.Models;
using LibraryManagement.Repositories;

namespace LibraryManagement.Services;

public class LoanService
{
    private readonly LoanRepository _repository;

    public LoanService(LoanRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> AddAsync(Loan loan)
    {
        ArgumentNullException.ThrowIfNull(loan);

        if (loan.BookId <= 0)
            throw new InvalidOperationException(
                "Book ID must be greater than zero.");

        if (loan.MemberId <= 0)
            throw new InvalidOperationException(
                "Member ID must be greater than zero.");

        if (loan.DueDate.Date < loan.LoanDate.Date)
            throw new InvalidOperationException(
                "Due date cannot be before the loan date.");

        loan.ReturnDate = null;

        return await _repository.AddAsync(loan);
    }

    public Task<List<Loan>> GetAllAsync()
    {
        return _repository.GetAllAsync();
    }

    public async Task<Loan?> GetByIdAsync(int id)
    {
        ValidateId(id);
        return await _repository.GetByIdAsync(id);
    }

    public async Task<bool> ReturnAsync(int id)
    {
        ValidateId(id);
        return await _repository.ReturnAsync(id);
    }

    private static void ValidateId(int id)
    {
        if (id <= 0)
            throw new InvalidOperationException(
                "Loan ID must be greater than zero.");
    }
}
