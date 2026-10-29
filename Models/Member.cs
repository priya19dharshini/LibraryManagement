public class Member
{
    public int MemberId { get; set; }
    public string MemberName { get; set; }
    public string MembershipType { get; set; }
    public int BorrowingLimit { get; set; }
    public decimal OutstandingFine { get; set; }

    public Member(int memberId, string memberName, string membershipType)
    {
        MemberId = memberId;
        MemberName = memberName;
        MembershipType = membershipType;
        BorrowingLimit = membershipType == "Standard" ? 3 : 5;
        OutstandingFine = 0;
    }
}