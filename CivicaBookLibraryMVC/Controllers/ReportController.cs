using CivicaBookLibraryMVC.Infrastructure;
using CivicaBookLibraryMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace CivicaBookLibraryMVC.Controllers
{
    public class ReportController : Controller
    {

        private readonly IHttpClientService _httpClientService;
        private readonly IConfiguration _configuration;
        private string endPoint;

        public ReportController(IHttpClientService httpClientService, IConfiguration configuration)
        {
            _httpClientService = httpClientService;
            _configuration = configuration;
            endPoint = _configuration["EndPoint:CivicaApi"];
        }

        [UserAuthorize]
        [HttpGet]
        public IActionResult ShowUserbookReport(DateTime? selectedDate, string type = "issue", int page = 1, int pageSize = 4)
        {
            var apiGetBooksUrl = "";
            var apiGetBooksCountUrl = "";

            var userId = Convert.ToInt32(Request.Cookies["userId"]);


            if (selectedDate == null)
            {
                apiGetBooksUrl = $"{endPoint}Report/UserBookReport" + "?userId=" + userId + "&type=" + type + "&page=" + page + "&pageSize=" + pageSize;


                apiGetBooksCountUrl = $"{endPoint}Report/GetBookCountForUser" + "?userId=" + userId + "&type=" + type;

            }
            else
            {
                ViewBag.IssueDate = selectedDate;

                apiGetBooksUrl = $"{endPoint}Report/UserBookReport" + "?userId=" + userId + "&selectedDate=" + selectedDate + "&type=" + type + "&page=" + page + "&pageSize=" + pageSize;

                apiGetBooksCountUrl = $"{endPoint}Report/GetBookCountForUser" + "?userId=" + userId + "&selectedDate=" + selectedDate + "&type=" + type;

            }



            ServiceResponse<int> countOfBooks = new ServiceResponse<int>();

            countOfBooks = _httpClientService.ExecuteApiRequest<ServiceResponse<int>>
                (apiGetBooksCountUrl, HttpMethod.Get, HttpContext.Request);

            int totalCount = countOfBooks.Data;
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;
            ViewBag.Type = type;
            ViewBag.UserId = userId;

            if (totalCount == 0)
            {
                return View(new List<UserBookReportViewModel>());

            }
            else
            {
                if (page > totalPages)
                {
                    return RedirectToAction("ShowUserbookReport", new { userId, selectedDate = selectedDate?.ToString("yyyy-MM-dd"),type, page = 1, pageSize });
                }

                ServiceResponse<IEnumerable<UserBookReportViewModel>> response = new ServiceResponse<IEnumerable<UserBookReportViewModel>>();

                response = _httpClientService.ExecuteApiRequest<ServiceResponse<IEnumerable<UserBookReportViewModel>>>
                    (apiGetBooksUrl, HttpMethod.Get, HttpContext.Request);

              

                //if (response.Message.Equals("No record found !."))
                //{
                //    return View(new List<ContactbookViewModel>());
                //}
                //else if (response.Success)
                if (response.Success)
                {

                    return View(response.Data);
                }

            }

            return View(new List<UserBookReportViewModel>());
        }

        [AdminAuthorize]
        public IActionResult AdminPartialreport()
        {
            return PartialView("_AdminReportPartialView");
        }

        [AdminAuthorize]
        public IActionResult AdminReport(int? userId, DateTime? issuedate, int page = 1, int pageSize = 4)
        {
            var apiGetContactsUrl = "";
            var apiGetCountUrl = "";

            ServiceResponse<int> countOfContact = new ServiceResponse<int>();

            var users = GetUsers();
            var books = GetBooks();

            if (userId != null)
            {
                 apiGetContactsUrl = $"{endPoint}Report/AdminBookReport?userId={userId}&page={page}&pageSize={pageSize}";
                 apiGetCountUrl = $"{endPoint}Report/GetBookCountWithDateOrStudent?userId={userId}";
            }
            else if (issuedate.HasValue)
            {
                apiGetContactsUrl = $"{endPoint}Report/AdminBookReport?issuedate={issuedate}&page={page}&pageSize={pageSize}";
                apiGetCountUrl = $"{endPoint}Report/GetBookCountWithDateOrStudent?issuedate={issuedate}";
            }
            else
            {
                 apiGetContactsUrl = $"{endPoint}Report/AdminBookReport?page={page}&pageSize={pageSize}";
                 apiGetCountUrl = $"{endPoint}Report/GetBookCountWithDateOrStudent";

            }
             countOfContact = _httpClientService.ExecuteApiRequest<ServiceResponse<int>>(apiGetCountUrl, HttpMethod.Get, HttpContext.Request);
            int totalCount = countOfContact.Data;


            ViewBag.UserId = userId; // Pass userId to ViewBag for use in the view
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.IssueDate = issuedate;

            ViewBag.Users = users;
            ViewBag.Books = books;

            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            ViewBag.TotalPages = totalPages;


            if (totalCount == 0)
            {
               
                return View(new List<AdminReportBookViewModel>());
            }
            if (page > totalPages)
            {
                return RedirectToAction("AdminReport", new { userId, issuedate, page = 1, pageSize }); 
            }

            ServiceResponse<IEnumerable<AdminReportBookViewModel>> response = _httpClientService.ExecuteApiRequest<ServiceResponse<IEnumerable<AdminReportBookViewModel>>>(apiGetContactsUrl, HttpMethod.Get, HttpContext.Request);

            if (response.Success)
            {
                var model = response.Data.ToList();
               

                foreach (var item in model)
                {
                    item.Users = users; 
                    item.Books = books;
                }

                return View(model);
            }

            return View(new List<AdminReportBookViewModel>());
        }

        [AdminAuthorize]
        public IActionResult AdminUserReport(int bookId, string type = "issue", int page = 1, int pageSize = 4)
        {
            var apiGetUsersUrl = "";
            var apiGetCountUserUrl = "";

            var users = GetUsers();
            var books = GetBooks();

            apiGetUsersUrl = $"{endPoint}Report/AdminUserReport?bookId={bookId}&type={type}&page={page}&pageSize={pageSize}";
            apiGetCountUserUrl = $"{endPoint}Report/GetUserCount?bookId={bookId}&type={type}";


            ServiceResponse<int> countOfUsers = _httpClientService.ExecuteApiRequest<ServiceResponse<int>>(apiGetCountUserUrl, HttpMethod.Get, HttpContext.Request);
            int totalCount = countOfUsers.Data;


            ViewBag.BookId = bookId;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.Type = type;

            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            ViewBag.TotalPages = totalPages;

            ViewBag.Users = users;
            ViewBag.Books = books;

            if (totalCount == 0)
            {
                return View(new List<AdminReportUserViewModel>());
            }
            if (page > totalPages)
            {
                return RedirectToAction("AdminUserReport", new { bookId, type, page = 1, pageSize }); 
            }

            ServiceResponse<IEnumerable<AdminReportUserViewModel>> response = _httpClientService.ExecuteApiRequest<ServiceResponse<IEnumerable<AdminReportUserViewModel>>>(apiGetUsersUrl, HttpMethod.Get, HttpContext.Request);

            if (response.Success)
            {
                var model = response.Data.ToList();

                foreach (var item in model)
                {
                    item.Users = users;
                    item.Books = books;
                }

                return View(model);
            }

            return View(new List<AdminReportUserViewModel>());
        }

        private List<UserViewModel> GetUsers()
        {
            ServiceResponse<IEnumerable<UserViewModel>> response = new ServiceResponse<IEnumerable<UserViewModel>>();
            response = _httpClientService.ExecuteApiRequest<ServiceResponse<IEnumerable<UserViewModel>>>
                ($"{endPoint}User/GetAllUsers", HttpMethod.Get, HttpContext.Request);

            if (response.Success)
            {
                return response.Data.ToList();
            }
            return new List<UserViewModel>();
        }

        private List<BooksViewModel> GetBooks()
        {
            ServiceResponse<IEnumerable<BooksViewModel>> response = new ServiceResponse<IEnumerable<BooksViewModel>>();
            response = _httpClientService.ExecuteApiRequest<ServiceResponse<IEnumerable<BooksViewModel>>>
                ($"{endPoint}Book/GetAllBooks", HttpMethod.Get, HttpContext.Request);

            if (response.Success)
            {
                return response.Data.ToList();
            }
            return new List<BooksViewModel>();
        }
    }




}

