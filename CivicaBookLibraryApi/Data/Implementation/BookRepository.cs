using CivicaBookLibraryApi.Data.Contract;
using CivicaBookLibraryApi.Dtos;
using CivicaBookLibraryApi.Models;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CivicaBookLibraryApi.Data.Implementation
{
    public class BookRepository : IBookRepository
    {
        private readonly IAppDbContext _appDbContext;

        public BookRepository(IAppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public IEnumerable<Book> GetAllBooks()
        {
            List<Book> books = _appDbContext.Books.ToList();
            return books;
        }
        public Book? GetBookById(int id)
        {
            var book = _appDbContext.Books
                .FirstOrDefault(c => c.BookId == id);
            return book;
        }
        public IEnumerable<Book> GetPaginatedBooks(int page, int pageSize, string? search, string sortOrder,string? sortBy)
        {
            int skip = (page - 1) * pageSize;
            IQueryable<Book> books = _appDbContext.Books;
            if (!string.IsNullOrEmpty(search))
            {
                books = books.Where(c => c.Title.Contains(search) || c.Author.Contains(search));
            }
            switch (sortBy?.ToLower())
            {
                case "title":
                    books = sortOrder.ToLower() == "desc" ? books.OrderByDescending(b => b.Title) : books.OrderBy(b => b.Title);
                    break;
                case "author":
                    books = sortOrder.ToLower() == "desc" ? books.OrderByDescending(b => b.Author) : books.OrderBy(b => b.Author);
                    break;
                case "price":
                    books = sortOrder.ToLower() == "desc" ? books.OrderByDescending(b => b.PricePerBook) : books.OrderBy(b => b.PricePerBook);
                    break;
                default:
                    books = books.OrderBy(b => b.Title);
                    break;
            }
            return books
            .Skip(skip)
                .Take(pageSize)
                .ToList();
        }
        public int TotalBooks( string? search)
        {
            IQueryable<Book> books = _appDbContext.Books;

            if (!string.IsNullOrEmpty(search))
            {
                books = books.Where(c => c.Title.Contains(search) || c.Author.Contains(search));
            }
            return books.Count();
        }
        public bool InsertBook(Book book)
        {
            var result = false;
            if (book != null)
            {
                _appDbContext.Books.Add(book);
                _appDbContext.SaveChanges();
                result = true;
            }
            return result;
        }
        public bool UpdateBook(Book book)
        {
            var result = false;
            if (book != null)
            {
                _appDbContext.Books.Update(book);
                _appDbContext.SaveChanges();
                result = true;
            }
            return result;
        }
        public bool DeleteBook(int id)
        {
            var result = false;
            var book = _appDbContext.Books.Find(id);
            if (book != null)
            {
                _appDbContext.Books.Remove(book);
                _appDbContext.SaveChanges();
                result = true;
            }
            return result;
        }
        public bool BookExists(string title,string author)
        {
            var books = _appDbContext.Books.FirstOrDefault(c => c.Title.ToLower() == title.ToLower() && c.Author.ToLower() == author.ToLower());
            if (books != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool BookExists(int id,string title, string author)
        {
            var book = _appDbContext.Books.FirstOrDefault(c => c.Title.ToLower() == title.ToLower() && c.Author == author && (c.BookId != id));
            if (book != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool InsertBookIssue(BookIssue bookIssue)
        {
            var result = false;
            if (bookIssue != null)
            {
                bookIssue.IssueDate = DateTime.Now;
                _appDbContext.BookIssues.Add(bookIssue);
                    _appDbContext.SaveChanges();
                    var book = _appDbContext.Books.Find(bookIssue.BookId);
                    if (book != null)
                    {
                        book.AvailableQuantity--; // Decrease available quantity
                        book.IssuedQuantity++;    // Increase issued quantity
                        _appDbContext.SaveChanges();
                    }
                return true;
            }
         return result;
        }
        public bool SubmitBook(int id)
        {
            var result = false;
            var bookIssue = _appDbContext.BookIssues.FirstOrDefault(c => c.IssueId == id);
            if (bookIssue != null)
            {
                bookIssue.ReturnDate = DateTime.Now;
                _appDbContext.Update(bookIssue);
                _appDbContext.SaveChanges();
                var book = _appDbContext.Books.Find(bookIssue.BookId);
                if (book != null)
                {
                    book.AvailableQuantity++; // Decrease available quantity
                    book.IssuedQuantity--;    // Increase issued quantity
                    _appDbContext.SaveChanges();
                }
                result = true;

            }
            return result;
        }

        public bool BookIssueExists(int bookId, int userId)
        {
            return _appDbContext.BookIssues.Any(c => c.BookId == bookId && c.UserId == userId && c.ReturnDate == null);
        }

        public int GetIssuedBookCountForUser(int userId)
        {
            return _appDbContext.BookIssues.Count(b => b.UserId == userId && b.ReturnDate == null);
        }
    }
}
