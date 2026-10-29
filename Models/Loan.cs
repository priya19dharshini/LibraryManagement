public class Loan
{
    public int LoanId { get; set; }
    public int MemberId { get; set; }
    public int BookId { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public int OverdueDays { get; set; }
    public decimal FineAmount { get; set; }
    public decimal FinePaid { get; set; }

    public decimal OutstandingFine
    {
        get { return FineAmount - FinePaid; }
    }

    public Loan(int loanId, int memberId, int bookId)
    {
        LoanId = loanId;
        MemberId = memberId;
        BookId = bookId;
        IssueDate = DateTime.Today;
        DueDate = IssueDate.AddDays(-2);
        ReturnDate = null;
        OverdueDays = 0;
        FineAmount = 0;
        FinePaid = 0;
    }
}