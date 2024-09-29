using CivicaBookLibraryApi.Dtos;
using CivicaBookLibraryApi.Models;
using CivicaBookLibraryApi.Services.Contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CivicaBookLibraryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
  
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet("GetAllBooks")]
        public IActionResult GetAllBooks()
        {
            var response = _bookService.GetAllBooks();
            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpGet("GetAllBooksByPagination")]
        public IActionResult GetPaginatedBooks(string? search, string? sortBy, int page=1, int pageSize=4, string sortOrder="asc")
        {
            var response = new ServiceResponse<IEnumerable<BookDto>>();
            response = _bookService.GetPaginatedBooks(page, pageSize, search, sortOrder,sortBy);
            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }
       

        [HttpGet("GetBookById")]
        public IActionResult GetBookById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Please enter valid data.");
            }
            else
            {
                var response = _bookService.GetBookById(id);
                return response.Success ? Ok(response) : NotFound(response);
            }
                
        }

        [HttpPost("InsertBook")]
        public IActionResult AddBook(AddBookDto addBook)
        {
            var response = _bookService.AddBook(addBook);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        [HttpPut("ModifyBook")]
        public IActionResult UpdateBook(BookDto bookDto)
        {
            var response = _bookService.ModifyBook(bookDto);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("RemoveBook/{id}")]
        public IActionResult DeleteBook(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Please enter valid data.");
            }
            else
            {
                var response = _bookService.RemoveBook(id);
                return response.Success ? Ok(response) : BadRequest(response);
            }
           
        }
        [HttpGet("GetBooksCount")]
        public IActionResult GetTotalCountOfContacts(string? search)
        {
            var response = _bookService.TotalBooks(search);
            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
        [HttpPost("BookIssue")]
        public IActionResult AddBookIssue(BookIssueDto bookIssueDto)
        {
            if (ModelState.IsValid)
            {
                var bookIssue = new BookIssue()
                {
                    ReturnDate = bookIssueDto.ReturnDate,
                    UserId = bookIssueDto.UserId,
                    BookId = bookIssueDto.BookId,

                };
                var result = _bookService.AddBookIssue(bookIssue);
                return !result.Success ? BadRequest(result) : Ok(result);
            }
            else
            {
                return BadRequest();
            }

        }
        [HttpPut("BookReturn/{id}")]
        public IActionResult SubmitBook(int id)
        {
            if (id > 0)
            {
                var result = _bookService.SubmitBook(id);
                return !result.Success ? BadRequest(result) : Ok(result);
            }
            else
            {
                return BadRequest("Please enter proper data");
            }
        }

    }
}
