namespace LibraryManagement.Models;
public class Book { 
    public int BookId { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public int TotalCopies { get; set; }
    public int AvailableCopies { get; set; }

    public Book(int bookId,string title,string author,int totalCopies)
    {
        BookId = bookId;
        Title = title;
        Author = author;
        TotalCopies = totalCopies;
        AvailableCopies = totalCopies;
    }
}
