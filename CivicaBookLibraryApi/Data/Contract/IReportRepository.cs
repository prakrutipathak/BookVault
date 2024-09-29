using CivicaBookLibraryApi.Models;

namespace CivicaBookLibraryApi.Data.Contract
{
    public interface IReportRepository
    {
        IEnumerable<BookIssue> GetIssuesAndReturnsForUser(int userId, DateTime? selectedDate, string type, int page, int pageSize);

        int TotalBookCountForUser(int userId, DateTime? selectedDate, string type);


        IEnumerable<BookIssue> GetIssueBookWithDateOrStudent(int? userId, DateTime? issuedate, int page, int pageSize);
        int TotalBookCountWithDateOrStudent(int? userId, DateTime? issuedate);

        IEnumerable<BookIssue> GetUserWithBook(int bookId, string type, int page, int pageSize);

        int TotalUserCountWithBook(int bookId, string type);

    }
}
