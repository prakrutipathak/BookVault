using CivicaBookLibraryApi.Models;

namespace CivicaBookLibraryApi.Data.Contract
{
    public interface ISecurityQuestionRepository
    {
        IEnumerable<SecurityQuestion> GetAllSecurityQuestions();
    }
}
