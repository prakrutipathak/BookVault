using CivicaBookLibraryApi.Dtos;
using CivicaBookLibraryApi.Models;

namespace CivicaBookLibraryApi.Services.Contract
{
    public interface IBookService
    {
        ServiceResponse<IEnumerable<BookDto>> GetPaginatedBooks(int page, int pageSize, string? search, string sortOrder, string? sortBy);
        ServiceResponse<int> TotalBooks(string? search);
        ServiceResponse<IEnumerable<BookDto>> GetAllBooks();
        ServiceResponse<string> AddBook(AddBookDto bookDto);
        ServiceResponse<string> ModifyBook(BookDto bookDto);
        ServiceResponse<BookDto> GetBookById(int id);
        ServiceResponse<string> RemoveBook(int id);
        ServiceResponse<string> AddBookIssue(BookIssue bookIssue);
        ServiceResponse<string> SubmitBook(int id);


    }
}
