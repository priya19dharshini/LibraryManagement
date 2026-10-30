
using LibraryManagement.Models;
using LibraryManagement.Repositories;

namespace LibraryManagement.Services;

public class LoanService
{
    private readonly LoanRepository _repository;
    private readonly AuditLogService _auditLogService;

    // Constructor Injection
    public LoanService(
        LoanRepository repository,
        AuditLogService auditLogService)
    {
        _repository = repository;
        _auditLogService = auditLogService;
    }

    // Add Loan
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

        int loanId = await _repository.AddAsync(loan);

        await _auditLogService.LogAsync(
            "INSERT",
            "Loan",
            loanId,
            "New loan created successfully");

        return loanId;
    }

    // Get All Loans
    public Task<List<Loan>> GetAllAsync()
    {
        return _repository.GetAllAsync();
    }

    // Get Loan By ID
    public async Task<Loan?> GetByIdAsync(int id)
    {
        ValidateId(id);

        return await _repository.GetByIdAsync(id);
    }

    // Return Book
    public async Task<bool> ReturnAsync(int id)
    {
        ValidateId(id);

        bool result = await _repository.ReturnAsync(id);

        if (result)
        {
            await _auditLogService.LogAsync(
                "UPDATE",
                "Loan",
                id,
                "Book returned successfully");
        }

        return result;
    }

    // Validate Loan ID
    private static void ValidateId(int id)
    {
        if (id <= 0)
            throw new InvalidOperationException(
                "Loan ID must be greater than zero.");
    }
}
