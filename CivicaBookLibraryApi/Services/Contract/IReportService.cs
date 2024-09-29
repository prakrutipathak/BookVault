using CivicaBookLibraryApi.Dtos;
using CivicaBookLibraryApi.Models;

namespace CivicaBookLibraryApi.Services.Contract
{
    public interface IReportService
    {
        ServiceResponse<IEnumerable<ReportDto>> GetIssuesAndReturnsForUser(int userId, DateTime? selectedDate = null, string type = "issue", int page = 1, int pageSize = 4);


        ServiceResponse<int> TotalBookCountForUser(int userId, DateTime? selectedDate, string type);

        ServiceResponse<IEnumerable<AdminReportBookDto>> IssueBookWithIssueDateOrUser(int? userId, DateTime? selectedDate, int page = 1, int pageSize = 4);

        ServiceResponse<int> TotalBookCountWithDateOrStudent(int? userId, DateTime? issuedate);

        ServiceResponse<IEnumerable<AdminReportUserDto>> GetUserWithBook(int bookId, string type, int page, int pageSize);
        ServiceResponse<int> TotalUserCountWithBook(int bookId, string type);
    }
}
