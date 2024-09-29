using CivicaBookLibraryApi.Data.Contract;
using CivicaBookLibraryApi.Dtos;
using CivicaBookLibraryApi.Services.Contract;

namespace CivicaBookLibraryApi.Services.Implementation
{
    public class SecurityQuestionService : ISecurityQuestionService
    {
        private readonly ISecurityQuestionRepository _securityQuestionRepository;
        public SecurityQuestionService(ISecurityQuestionRepository securityQuestionRepository)
        {
            _securityQuestionRepository = securityQuestionRepository;
        }

        public ServiceResponse<IEnumerable<SecurityQuestionDto>> GetAllSecurityquestions()
        {
            var response = new ServiceResponse<IEnumerable<SecurityQuestionDto>>();
            var questions = _securityQuestionRepository.GetAllSecurityQuestions();

            if (questions != null && questions.Any())
            {
                var questionDtoList = new List<SecurityQuestionDto>();
                foreach (var question in questions)
                {
                    questionDtoList.Add(new SecurityQuestionDto()
                    {
                        PasswordHint = question.PasswordHint,
                        Question = question.Question,
                    });
                }
                response.Data = questionDtoList;
                response.Success = true;
                response.Message = "Success";
            }
            else
            {
                response.Success = false;
                response.Message = "No question found!";
            }
            return response;
        }
    }
}
