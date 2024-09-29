using CivicaBookLibraryApi.Dtos;

namespace CivicaBookLibraryApi.Services.Contract
{
    public interface ISecurityQuestionService
    {
        ServiceResponse<IEnumerable<SecurityQuestionDto>> GetAllSecurityquestions();
    }
}
