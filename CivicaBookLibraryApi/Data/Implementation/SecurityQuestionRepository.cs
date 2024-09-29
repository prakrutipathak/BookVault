using CivicaBookLibraryApi.Data.Contract;
using CivicaBookLibraryApi.Models;

namespace CivicaBookLibraryApi.Data.Implementation
{
    public class SecurityQuestionRepository : ISecurityQuestionRepository
    {
        private readonly IAppDbContext _appDbContext;
        public SecurityQuestionRepository(IAppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public IEnumerable<SecurityQuestion> GetAllSecurityQuestions()
        {
            List<SecurityQuestion> questions = _appDbContext.SecurityQuestions.ToList();
            return questions;
        }   
    }
}
