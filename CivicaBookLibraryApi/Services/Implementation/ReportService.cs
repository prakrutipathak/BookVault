using CivicaBookLibraryApi.Data.Contract;
using CivicaBookLibraryApi.Data.Implementation;
using CivicaBookLibraryApi.Dtos;
using CivicaBookLibraryApi.Models;
using CivicaBookLibraryApi.Services.Contract;

namespace CivicaBookLibraryApi.Services.Implementation
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;

        public ReportService(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public ServiceResponse<IEnumerable<ReportDto>> GetIssuesAndReturnsForUser(int userId, DateTime? selectedDate = null, string type = "issue", int page = 1, int pageSize = 4)
        {
            var response = new ServiceResponse<IEnumerable<ReportDto>>();
            var bookIssues = _reportRepository.GetIssuesAndReturnsForUser(userId, selectedDate, type, page, pageSize);

            if (bookIssues != null && bookIssues.Any())
            {
                List<ReportDto> reportDtos = new List<ReportDto>();

                foreach (var bi in bookIssues)
                {
                    reportDtos.Add(
                        new ReportDto()
                        {
                            IssueId = bi.IssueId,
                            Title = bi.Book.Title,
                            Author = bi.Book.Author,
                            IssueDate = bi.IssueDate,
                            ReturnDate = bi.ReturnDate,
                        });
                }
                response.Data = reportDtos;

            }
            else
            {
                response.Success = false;
                response.Message = "No record found !.";
            }

            return response;
        }

        public ServiceResponse<int> TotalBookCountForUser(int userId, DateTime? selectedDate, string type)
        {
            var response = new ServiceResponse<int>();
            int totalUsers = _reportRepository.TotalBookCountForUser(userId, selectedDate, type);

            response.Success = true;
            response.Data = totalUsers;
            return response;
        }

        public ServiceResponse<IEnumerable<AdminReportBookDto>> IssueBookWithIssueDateOrUser(int? userId, DateTime? selectedDate, int page = 1, int pageSize = 4)
        {
            var response = new ServiceResponse<IEnumerable<AdminReportBookDto>>();
            var bookIssues = _reportRepository.GetIssueBookWithDateOrStudent(userId, selectedDate, page, pageSize);

            if (bookIssues != null && bookIssues.Any())
            {
                List<AdminReportBookDto> reportDtos = new List<AdminReportBookDto>();

                foreach (var bi in bookIssues)
                {
                    reportDtos.Add(
                        new AdminReportBookDto()
                        {
                            Title = bi.Book.Title,
                            Author = bi.Book.Author,
                            IssueDate = bi.IssueDate,
                            UserId = bi.UserId,
                            User = new User
                            {
                                UserId = bi.UserId,
                                Name = bi.User.Name
                            }
                        });
                }
                response.Data = reportDtos;

            }
            else
            {
                response.Success = false;
                response.Message = "No record found !.";
            }

            return response;

        }

        public ServiceResponse<int> TotalBookCountWithDateOrStudent(int? userId, DateTime? issuedate)
        {
            var response = new ServiceResponse<int>();
            int totalUsers = _reportRepository.TotalBookCountWithDateOrStudent(userId, issuedate);

            response.Success = true;
            response.Data = totalUsers;
            return response;
        }

        public ServiceResponse<IEnumerable<AdminReportUserDto>> GetUserWithBook(int bookId, string type, int page, int pageSize)
        {
            var response = new ServiceResponse<IEnumerable<AdminReportUserDto>>();
            var users = _reportRepository.GetUserWithBook(bookId,type, page, pageSize);

            if (users != null && users.Any())
            {
                List<AdminReportUserDto> reportDtos = new List<AdminReportUserDto>();

                foreach (var bi in users)
                {
                    reportDtos.Add(
                        new AdminReportUserDto()
                        {
                            UserId = bi.UserId,
                            IssueDate = bi.IssueDate,
                            ReturnDate = bi.ReturnDate,
                            User = new User
                            {
                                UserId = bi.UserId,
                                Name = bi.User.Name
                            }
                        });
                }
                response.Data = reportDtos;

            }
            else
            {
                response.Success = false;
                response.Message = "No record found !.";
            }

            return response;

        }

        public ServiceResponse<int> TotalUserCountWithBook(int bookId, string type)
        {
            var response = new ServiceResponse<int>();
            int totalUsers = _reportRepository.TotalUserCountWithBook(bookId,type);

            response.Success = true;
            response.Data = totalUsers;
            return response;
        }
    }

}
