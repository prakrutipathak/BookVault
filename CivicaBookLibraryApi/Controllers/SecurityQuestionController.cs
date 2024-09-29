using CivicaBookLibraryApi.Services.Contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CivicaBookLibraryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class SecurityQuestionController : ControllerBase
    {
        private readonly ISecurityQuestionService _securityQuestionService;
        public SecurityQuestionController(ISecurityQuestionService securityQuestionService)
        {
            _securityQuestionService = securityQuestionService;
        }
        [HttpGet("GetAllSecurityQuestions")]
        public IActionResult GetAllSecurityQuestions()
        {
            var response = _securityQuestionService.GetAllSecurityquestions();
            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
    }
}
