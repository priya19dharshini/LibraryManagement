
using LibraryManagement.Menus;
using LibraryManagement.Repositories;
using LibraryManagement.Services;

var bookRepository = new BookRepository();
var memberRepository = new MemberRepository();

var bookService = new BookService(bookRepository);
var memberService = new MemberService(memberRepository);

var bookMenu = new BookMenu(bookService);
var memberMenu = new MemberMenu(memberService);

bool running = true;

while (running)
{
    Console.WriteLine("\n===== LIBRARY MANAGEMENT SYSTEM =====");
    Console.WriteLine("1. Book Management");
    Console.WriteLine("2. Member Management");
    Console.WriteLine("0. Exit");

    if (!int.TryParse(
        LibraryManagement.Helper.InputHelper
            .ReadRequiredInput("Enter choice: "),
        out int choice))
    {
        Console.WriteLine("Invalid input. Enter a number.");
        continue;
    }

    switch (choice)
    {
        case 1:
            await bookMenu.RunAsync();
            break;

        case 2:
            await memberMenu.RunAsync();
            break;


        case 0:
            running = false;
            Console.WriteLine("Exiting Library Management System...");
            break;

        default:
            Console.WriteLine("Invalid choice. Try again.");
            break;
    }
}
