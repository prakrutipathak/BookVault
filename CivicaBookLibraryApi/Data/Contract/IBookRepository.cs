using CivicaBookLibraryApi.Dtos;
using CivicaBookLibraryApi.Models;

namespace CivicaBookLibraryApi.Data.Contract
{
    public interface IBookRepository
    {
        IEnumerable<Book> GetPaginatedBooks(int page, int pageSize, string? search, string sortOrder,string? sortBy);
        int TotalBooks(string? search);
        IEnumerable<Book> GetAllBooks();
        Book? GetBookById(int id);
        bool InsertBook(Book book);
        bool UpdateBook(Book book);
        bool DeleteBook(int id);
        bool BookExists(string title, string author);
        bool BookExists(int id, string title, string author);
        bool InsertBookIssue(BookIssue bookIssue);
        bool SubmitBook(int id);
        bool BookIssueExists(int bookId, int userId);
        int GetIssuedBookCountForUser(int userId);
    }
}
