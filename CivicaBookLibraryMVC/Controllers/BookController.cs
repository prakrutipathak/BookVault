using CivicaBookLibraryMVC.Infrastructure;
using CivicaBookLibraryMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics.Metrics;

namespace CivicaBookLibraryMVC.Controllers
{
    public class BookController : Controller
    {
        private readonly IHttpClientService _httpClientService;
        private readonly IConfiguration _configuration;
        private string endPoint;
        public BookController(IHttpClientService httpClientService, IConfiguration configuration)
        {
            _httpClientService = httpClientService;
            _configuration = configuration;
            endPoint = _configuration["EndPoint:CivicaApi"];
        }

        [UserAuthorize]
        [HttpGet]
        public IActionResult BookIssue()
        {
            BookIssueViewModel viewModel = new BookIssueViewModel();
            viewModel.Books = GetBooks();
            return View(viewModel);
        }
        [HttpPost]
        public IActionResult BookIssue(BookIssueViewModel viewModel)
        {
            viewModel.Books = GetBooks();
            var userId = Convert.ToInt32(Request.Cookies["userId"]);
            // Retrieve userId from cookies
           
                viewModel.UserId = userId;
           
            if (ModelState.IsValid)
            {
                var apiUrl = $"{endPoint}Book/BookIssue";
                var response = _httpClientService.PostHttpResponseMessage<BookIssueViewModel>(apiUrl, viewModel, HttpContext.Request);
                if (response.IsSuccessStatusCode)
                {
                    string successResponse = response.Content.ReadAsStringAsync().Result;
                    var serviceResponse = JsonConvert.DeserializeObject<ServiceResponse<string>>(successResponse);
                    TempData["SuccessMessage"] = serviceResponse.Message;
                    return RedirectToAction("ShowUserbookReport", "Report");
                }
                else
                {
                    string errorData = response.Content.ReadAsStringAsync().Result;
                    var errorResponse = JsonConvert.DeserializeObject<ServiceResponse<string>>(errorData);
                    if (errorResponse != null)
                    {
                        TempData["ErrorMessage"] = errorResponse.Message;
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Something went wrong try after some time";
                        return RedirectToAction("Index", "Home");
                    }
                }
            }

            return View(viewModel);
        }

        [UserAuthorize]
        public IActionResult ReturnBook(int id)
        {
            var apiUrl = $"{endPoint}Book/BookReturn/" + id;
            var response = _httpClientService.ExecuteApiRequest<ServiceResponse<string>>
                   ($"{apiUrl}", HttpMethod.Put, HttpContext.Request);
            if (response.Success)
            {
                TempData["SuccessMessage"] = response.Message;
                return RedirectToAction("ShowUserbookReport", "Report");
            }
            else
            {
                TempData["ErrorMessage"] = response.Message;

            }
            return RedirectToAction("ReturnBook", "Book");
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

        [AdminAuthorize]
        public IActionResult Index(string? search, string? sortBy, string sortOrder="asc", int page = 1, int pageSize = 2)
        {
            var getBooksUrl = "";
            var getBooksCountUrl = "";
            ViewBag.Search = search;
            if (search != null  && sortBy != null && search.Length > 2)
            {
                getBooksUrl = endPoint+"Book/GetAllBooksByPagination?search="+search+"&sortBy="+sortBy+"&page="+page+"&pageSize="+pageSize+"&sortOrder="+sortOrder;
                getBooksCountUrl = $"{endPoint}Book/GetBooksCount?search={search}";
            }
            else if (search == null && sortBy != null)
            {
                getBooksUrl = endPoint+"Book/GetAllBooksByPagination?sortBy="+sortBy+"&page="+page+"&pageSize="+pageSize+"&sortOrder="+sortOrder;
                getBooksCountUrl = endPoint+"Book/GetBooksCount";
            }
            else if (search != null && search.Length > 2 && sortBy == null)
            {
                getBooksUrl = endPoint+"Book/GetAllBooksByPagination?search="+search+"&page="+page+"&pageSize="+pageSize+"&sortOrder="+sortOrder;
                getBooksCountUrl = endPoint+"Book/GetBooksCount?search="+search;
            }
            else
            {
                getBooksUrl = endPoint+"Book/GetAllBooksByPagination?page="+page+"&pageSize="+pageSize+"&sortOrder="+sortOrder;
                getBooksCountUrl = endPoint+"Book/GetBooksCount";
            }

            ServiceResponse<int> countOfBooks = new ServiceResponse<int>();

            countOfBooks = _httpClientService.ExecuteApiRequest<ServiceResponse<int>>
                (getBooksCountUrl, HttpMethod.Get, HttpContext.Request);

            int totalCount = countOfBooks.Data;

            if (totalCount == 0)
            {
                return View(new List<BooksViewModel>());
            }
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);


            if (page > totalPages)
            {
                return RedirectToAction("Index", new { search, sortBy, sortOrder, page = 1, pageSize });
            }
            ViewBag.SortBy = sortBy;
            ViewBag.SortOrder = sortOrder;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;
            ServiceResponse<IEnumerable<BooksViewModel>> response = new ServiceResponse<IEnumerable<BooksViewModel>>();
            response = _httpClientService.ExecuteApiRequest<ServiceResponse<IEnumerable<BooksViewModel>>>
                (getBooksUrl, HttpMethod.Get, HttpContext.Request);
            if (response.Success)
            {
                return View(response.Data);
            }
            return View(new List<BooksViewModel>());
        }

        [AdminAuthorize]
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Add(AddBookViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var apiUrl = $"{endPoint}Book/InsertBook";
                var response = _httpClientService.PostHttpResponseMessage(apiUrl, viewModel, HttpContext.Request);
                if (response.IsSuccessStatusCode)
                {
                    string SuccessMessage = response.Content.ReadAsStringAsync().Result;
                    var serviceResponse = JsonConvert.DeserializeObject<ServiceResponse<string>>(SuccessMessage);

                    TempData["SuccessMessage"] = serviceResponse?.Message;
                    return RedirectToAction("Index");
                }
                else
                {
                    string errorMessage = response.Content.ReadAsStringAsync().Result;
                    var errorResponse = JsonConvert.DeserializeObject<ServiceResponse<string>>(errorMessage);
                    if (errorResponse != null)
                    {
                        TempData["ErrorMessage"] = errorResponse.Message;

                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Something went wrong. Please try after sometime";
                    }

                }

            }

            return View(viewModel);
        }

        [AdminAuthorize]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var apiUrl = $"{endPoint}Book/GetBookById?id=" + id;
            var response = _httpClientService.GetHttpResponseMessage<BooksViewModel>(apiUrl, HttpContext.Request);

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                var serviceResponse = JsonConvert.DeserializeObject<ServiceResponse<BooksViewModel>>(data);

                if (serviceResponse != null && serviceResponse.Success && serviceResponse.Data != null)
                {
                    BooksViewModel viewModel = serviceResponse.Data;
                    return View(viewModel);
                }
                else
                {
                    TempData["ErrorMessage"] = serviceResponse?.Message;
                    return RedirectToAction("Index");
                }
            }
            else
            {
                string errorData = response.Content.ReadAsStringAsync().Result;
                var errorResponse = JsonConvert.DeserializeObject<ServiceResponse<BooksViewModel>>(errorData);
                if (errorResponse != null)
                {
                    TempData["ErrorMessage"] = errorResponse.Message;

                }
                else
                {
                    TempData["ErrorMessage"] = "Something went wrong. Please try after sometime";
                }
                return RedirectToAction("Index");

            }

        }

        [HttpPost]
        public IActionResult Edit(BooksViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var apiUrl = $"{endPoint}Book/ModifyBook";
                HttpResponseMessage response = _httpClientService.PutHttpResponseMessage(apiUrl, viewModel, HttpContext.Request);
                if (response.IsSuccessStatusCode)
                {
                    string SuccessMessage = response.Content.ReadAsStringAsync().Result;
                    var serviceResponse = JsonConvert.DeserializeObject<ServiceResponse<string>>(SuccessMessage);

                    TempData["SuccessMessage"] = serviceResponse?.Message;
                    return RedirectToAction("Index");
                }
                else
                {
                    string errorMessage = response.Content.ReadAsStringAsync().Result;
                    var errorResponse = JsonConvert.DeserializeObject<ServiceResponse<string>>(errorMessage);
                    if (errorResponse != null)
                    {
                        TempData["ErrorMessage"] = errorResponse.Message;

                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Something went wrong. Please try after sometime";
                    }
                    return RedirectToAction("Index");
                }
            }
            return View(viewModel);
        }

        [AdminAuthorize]
        public IActionResult Details(int id)
        {
            var apiUrl = $"{endPoint}Book/GetBookById?id=" + id;
            var response = _httpClientService.GetHttpResponseMessage<BooksViewModel>(apiUrl, HttpContext.Request);

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                var serviceResponse = JsonConvert.DeserializeObject<ServiceResponse<BooksViewModel>>(data);

                if (serviceResponse != null && serviceResponse.Success && serviceResponse.Data != null)
                {
                    return View(serviceResponse.Data);
                }
                else
                {
                    TempData["ErrorMessage"] = serviceResponse?.Message;
                    return RedirectToAction("Index");
                }
            }
            else
            {
                string errorData = response.Content.ReadAsStringAsync().Result;
                var errorResponse = JsonConvert.DeserializeObject<ServiceResponse<BooksViewModel>>(errorData);
                if (errorResponse != null)
                {
                    TempData["ErrorMessage"] = errorResponse.Message;

                }
                else
                {
                    TempData["ErrorMessage"] = "Something went wrong. Please try after sometime";
                }
                return RedirectToAction("Index");

            }

        }

        [AdminAuthorize]
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var apiUrl = $"{endPoint}Book/RemoveBook/" + id;
            var response = _httpClientService.ExecuteApiRequest<ServiceResponse<string>>
                ($"{apiUrl}", HttpMethod.Delete, HttpContext.Request);

            if (response.Success)
            {
                TempData["SuccessMessage"] = response.Message;
                return RedirectToAction("Index");
            }
            else
            {
                TempData["ErrorMessage"] = response.Message;
                return RedirectToAction("Index");
            }
        }
    }
}
