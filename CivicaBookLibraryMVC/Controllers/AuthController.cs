using CivicaBookLibraryMVC.Infrastructure;
using CivicaBookLibraryMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
namespace CivicaBookLibraryMVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly IHttpClientService _httpClientService;
        private readonly IConfiguration _configuration;
        private readonly IJwtTokenHandler _tokenHandler;
        private string endPoint;
        public AuthController(IHttpClientService httpClientService, IConfiguration configuration, IJwtTokenHandler tokenHandler)
        {
            _httpClientService = httpClientService;
            _configuration = configuration;
            _tokenHandler = tokenHandler;
            endPoint = _configuration["EndPoint:CivicaApi"];
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ForgetPassword()
        {
            IEnumerable<SecurityQuestionViewModel> questions = GetQuestions();
            ViewBag.Questions = questions;
            return View();
        }
        [HttpPost]
        public IActionResult ForgetPassword(ForgetPasswordViewModel forgetPasswordViewModel)
        {
            if (ModelState.IsValid)
            {
                var apiUrl = $"{endPoint}User/ResetPassword";
                var response = _httpClientService.PutHttpResponseMessage(apiUrl, forgetPasswordViewModel, HttpContext.Request);
                if (response.IsSuccessStatusCode)
                {
                    string successResponse = response.Content.ReadAsStringAsync().Result;
                    var serviceResponse = JsonConvert.DeserializeObject<ServiceResponse<string>>(successResponse);
                    TempData["SuccessMessage"] = serviceResponse.Message;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    string errorResponse = response.Content.ReadAsStringAsync().Result;
                    var serviceResponse = JsonConvert.DeserializeObject<ServiceResponse<string>>(errorResponse);
                    if (serviceResponse != null)
                    {
                        TempData["ErrorMessage"] = serviceResponse.Message;
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Something went wrong please try after some time.";
                    }
                }
            }
            IEnumerable<SecurityQuestionViewModel> questions = GetQuestions();
            ViewBag.Questions = questions;
            return View(forgetPasswordViewModel);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult LoginUser()
        {
            return View();
        }

        [HttpPost]
        public IActionResult LoginUser(LoginViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                string apiUrl = $"{endPoint}User/Login";
                var response = _httpClientService.PostHttpResponseMessage(apiUrl, viewModel, HttpContext.Request);
                if (response.IsSuccessStatusCode)
                {
                    string successResponse = response.Content.ReadAsStringAsync().Result;
                    var serviceResponse = JsonConvert.DeserializeObject<ServiceResponse<string>>(successResponse);
                    string token = serviceResponse.Data;
                    Response.Cookies.Append("jwtToken", token, new CookieOptions
                    {
                        HttpOnly = false,
                        Secure = true,
                        SameSite = SameSiteMode.None,
                        Expires = DateTime.UtcNow.AddDays(1) //Set expiration time for cookie.
                    });
                    var jwtToken = _tokenHandler.ReadJwtToken(token);
                    var userId = jwtToken.Claims.First(claim => claim.Type == "UserId").Value;
                    var isAdmin = jwtToken.Claims.First(claim => claim.Type == "Admin").Value;
                    Response.Cookies.Append("userId", userId, new CookieOptions
                    {
                        HttpOnly = false,
                        Secure = true,
                        SameSite = SameSiteMode.None,
                        Expires = DateTime.UtcNow.AddDays(1),
                    }); 
                    //Response.Cookies.Append("admin", isAdmin, new CookieOptions
                    //{
                    //    HttpOnly = false,
                    //    Secure = true,
                    //    SameSite = SameSiteMode.None,
                    //    Expires = DateTime.UtcNow.AddDays(1),
                    //});
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    string errorResponse = response.Content.ReadAsStringAsync().Result;
                    var serviceResponse = JsonConvert.DeserializeObject<ServiceResponse<string>>(errorResponse);
                    if (serviceResponse != null)
                    {
                        TempData["ErrorMessage"] = serviceResponse.Message;
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Something went wrong, Please try after sometime.";
                    }
                }
            }
            return View(viewModel);
        }

        [AllowAnonymous]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwtToken");
            Response.Cookies.Delete("userId");
            return RedirectToAction("Index", "Home");
        }

        [UserAuthorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordViewModel changePasswordViewModel)
        {
            if (ModelState.IsValid)
            {
                var apiUrl = $"{endPoint}User/ChangePassword";
                var response = _httpClientService.PutHttpResponseMessage(apiUrl, changePasswordViewModel, HttpContext.Request);
                if (response.IsSuccessStatusCode)
                {
                    string successResponse = response.Content.ReadAsStringAsync().Result;
                    var serviceResponse = JsonConvert.DeserializeObject<ServiceResponse<string>>(successResponse);
                    Response.Cookies.Delete("jwtToken");
                    TempData["SuccessMessage"] = serviceResponse.Message;
                    return RedirectToAction("LoginUser", "Auth");
                }
                else
                {
                    string errorResponse = response.Content.ReadAsStringAsync().Result;
                    var serviceResponse = JsonConvert.DeserializeObject<ServiceResponse<string>>(errorResponse);
                    if (serviceResponse != null)
                    {
                        TempData["ErrorMessage"] = serviceResponse.Message;
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Something went wrong please try after some time.";
                    }
                }
            }
            return View(changePasswordViewModel);
        }
        
        private IEnumerable<SecurityQuestionViewModel> GetQuestions()
        {
            var apiUrl = $"{endPoint}SecurityQuestion/GetAllSecurityQuestions";
            ServiceResponse<IEnumerable<SecurityQuestionViewModel>> response = new ServiceResponse<IEnumerable<SecurityQuestionViewModel>>();
            response = _httpClientService.ExecuteApiRequest<ServiceResponse<IEnumerable<SecurityQuestionViewModel>>>
                (apiUrl, HttpMethod.Get, HttpContext.Request);
            return response.Data;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult RegisterUser()
        {
            IEnumerable<SecurityQuestionViewModel> securityQuestions = GetQuestions();
            ViewBag.SecurityQuestions = securityQuestions;
            return View();
        }
        [HttpPost]
        public IActionResult RegisterUser(RegisterViewModel registerViewModel)
        {
            if (ModelState.IsValid)
            {
                var apiUrl = $"{endPoint}User/Register";
                var response = _httpClientService.PostHttpResponseMessage(apiUrl, registerViewModel, HttpContext.Request);

                if (response.IsSuccessStatusCode)
                {

                    string data = response.Content.ReadAsStringAsync().Result;
                    var serviceResponse = JsonConvert.DeserializeObject<ServiceResponse<string>>(data);
                    TempData["SuccessMessage"] = serviceResponse.Message;
                    return RedirectToAction("RegisterSuccess", "Auth");
                    //var age = CalculateAge(registerViewModel.DateOfBirth);
                    //if (registerViewModel.DateOfBirth > DateTime.Now)
                    //{
                    //    TempData["ErrorMessage"] = "Date of birth cannot be in future.";
                    //    IEnumerable<SecurityQuestionViewModel> securityQuestions = GetQuestions();
                    //    ViewBag.SecurityQuestions = securityQuestions;
                    //    return View(registerViewModel);
                    //}
                    //if (age < 18 || age > 120)
                    //{
                    //    TempData["ErrorMessage"] = "Age cannot be less than 18 or greater than 120.";
                    //    IEnumerable<SecurityQuestionViewModel> securityQuestions = GetQuestions();
                    //    ViewBag.SecurityQuestions = securityQuestions;
                    //    return View(registerViewModel);
                    //}
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
                        TempData["ErrorMessage"] = "Something went wrong please try after some time.";
                    }
                }
            }
           
            IEnumerable<SecurityQuestionViewModel> securityQuestion = GetQuestions();
            ViewBag.SecurityQuestions = securityQuestion;
            return View(registerViewModel);
        }

        [AllowAnonymous]
        public IActionResult RegisterSuccess()
        {
            return View();
        }

        [AdminAuthorize]
        public IActionResult Index(string? search, int page = 1, int pageSize = 2, string sortOrder = "asc")
        {
            var apiGetUsersUrl = "";
            var apiGetCountUrl = "";
            ViewBag.Search = search;
            ViewBag.SortOrder = sortOrder;
            if (search != null && search.Length > 2)
            {
                apiGetUsersUrl = $"{endPoint}User/GetAllUsersByPagination?search={search}&page={page}&pageSize={pageSize}&sortOrder={sortOrder}";
                apiGetCountUrl = $"{endPoint}User/GetUsersCount?search={search}";
            }
            else
            {
                apiGetUsersUrl = $"{endPoint}User/GetAllUsersByPagination?page={page}&pageSize={pageSize}&sortOrder={sortOrder}";
                apiGetCountUrl = $"{endPoint}User/GetUsersCount";
            }
            ServiceResponse<int> countOfUser = new ServiceResponse<int>();
            countOfUser = _httpClientService.ExecuteApiRequest<ServiceResponse<int>>
                (apiGetCountUrl, HttpMethod.Get, HttpContext.Request);
            int totalCount = countOfUser.Data;
            if (totalCount == 0)
            {
                // Return an empty view
                return View(new List<UserViewModel>());
            }
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            if (page > totalPages)
            {
                // Redirect to the first page with the new page size
                return RedirectToAction("Index", new { page = 1, pageSize });
            }
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;
            ServiceResponse<IEnumerable<UserViewModel>> response = new ServiceResponse<IEnumerable<UserViewModel>>();
            response = _httpClientService.ExecuteApiRequest<ServiceResponse<IEnumerable<UserViewModel>>>
                (apiGetUsersUrl, HttpMethod.Get, HttpContext.Request);
            if (response.Success)
            {
                return View(response.Data);
            }
            return View(new List<UserViewModel>());
        }

        [AdminAuthorize]
        [HttpPost]  
        public IActionResult Delete(int id)
        {
            var apiUrl = $"{endPoint}User/DeleteUser/" + id;
            var response = _httpClientService.ExecuteApiRequest<ServiceResponse<string>>($"{apiUrl}", HttpMethod.Delete, HttpContext.Request);
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
        //private int CalculateAge(DateTime birthdate)
        //{
        //    DateTime today = DateTime.Today;
        //    int age = today.Year - birthdate.Year;
        //    return age;
        //}
    }
}
