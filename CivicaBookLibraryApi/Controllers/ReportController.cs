using CivicaBookLibraryApi.Dtos;
using CivicaBookLibraryApi.Models;
using CivicaBookLibraryApi.Services.Contract;
using CivicaBookLibraryApi.Services.Implementation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Metrics;

namespace CivicaBookLibraryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }


        [HttpGet("UserBookReport")]
        public IActionResult IssueBooksReport(int userId, DateTime? selectedDate, string type = "issue", int page = 1, int pageSize = 4)
        {

            var response = _reportService.GetIssuesAndReturnsForUser(userId, selectedDate, type, page, pageSize);

            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);

        }

        [HttpGet("GetBookCountForUser")]
        public IActionResult GetBookCountForUser(int userId, DateTime? selectedDate, string type = "issue")
        {
            var response = _reportService.TotalBookCountForUser(userId, selectedDate, type);
            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpGet("AdminBookReport")]
        public IActionResult GetIssueBookWithIssueDateOrUser(int? userId, DateTime? issuedate, int page = 1, int pageSize = 4)
        {
            var response = _reportService.IssueBookWithIssueDateOrUser(userId, issuedate, page, pageSize);

            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        [HttpGet("GetBookCountWithDateOrStudent")]
        public IActionResult GetBookCountWithDateOrStudent(int? userId, DateTime? issuedate)
        {
            var response = _reportService.TotalBookCountWithDateOrStudent(userId, issuedate);
            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpGet("AdminUserReport")]
        public IActionResult GetUserWithBook(int bookId, string type = "issue",int page = 1, int pageSize = 4)
        {

            var response = _reportService.GetUserWithBook(bookId, type, page, pageSize);

            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);

        }

        [HttpGet("GetUserCount")]
        public IActionResult TotalUserCountWithBook(int bookId, string type= "issue")
        {
            var response = _reportService.TotalUserCountWithBook(bookId, type);
            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }




    }
}
