
using LibraryManagement.Menus;
using LibraryManagement.Repositories;
using LibraryManagement.Services;

var repository = new BookRepository();
var service = new BookService(repository);
var menu = new BookMenu(service);

await menu.RunAsync();
