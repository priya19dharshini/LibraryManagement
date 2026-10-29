namespace LibraryManagement.Models;

public class Member
{
    public int MemberId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
}