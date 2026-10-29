namespace LibraryManagement.Models {

    public class Book
    {
        public int BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Isbn { get; set; } = string.Empty;
        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }
        public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    }

}
