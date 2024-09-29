using CivicaBookLibraryApi.Data.Contract;
using CivicaBookLibraryApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CivicaBookLibraryApi.Data.Implementation
{
    public class ReportRepository : IReportRepository
    {
        private readonly IAppDbContext _appDbContext;

        public ReportRepository(IAppDbContext context)
        {
            _appDbContext = context;
        }

        public IEnumerable<BookIssue> GetIssuesAndReturnsForUser(int userId, DateTime? selectedDate, string type, int page, int pageSize)
        {
            int skip = (page - 1) * pageSize;

            IQueryable<BookIssue> query = _appDbContext.BookIssues
            .Include(bi => bi.Book)
            .Include(bi => bi.User)
            .Where(bi => bi.UserId == userId);

            if (selectedDate != null)
            {
                var truncatedDate = selectedDate.Value.Date;
                if (type == "issue")
                {

                    query = query.Where(bi => bi.IssueDate.Date <= truncatedDate && bi.ReturnDate == null).OrderBy(bi => bi.IssueDate);
                }
                else if (type == "return")
                {
                    query = query.Where(bi => bi.ReturnDate.Value.Date <= truncatedDate).OrderBy(bi => bi.ReturnDate);
                }
            }
            else
            {
                if (type == "issue")
                {

                    //query = query.OrderBy(bi => bi.ReturnDate.HasValue).ThenBy(bi => bi.IssueDate);
                    query = query.Where(bi => bi.ReturnDate == null).OrderBy(bi => bi.IssueDate);
                }
                if (type == "return")
                {
                    query = query.Where(bi => bi.ReturnDate != null).OrderBy(bi => bi.ReturnDate);
                }
            }

            return query.
               Skip(skip)
               .Take(pageSize).
               ToList();
        }

        public int TotalBookCountForUser(int userId, DateTime? selectedDate, string type)
        {
            IQueryable<BookIssue> query = _appDbContext.BookIssues.Where(bi => bi.UserId == userId); ;

            if (selectedDate != null)
            {
                var truncatedDate = selectedDate.Value.Date;
                if (type == "issue")
                {

                    query = query.Where(bi => bi.IssueDate.Date <= truncatedDate).OrderBy(bi => bi.IssueDate);
                }
                else if (type == "return")
                {
                    query = query.Where(bi => bi.ReturnDate.Value.Date <= truncatedDate).OrderBy(bi => bi.ReturnDate);
                }
            }
            else
            {
                if (type == "issue")
                {
                    query = query.OrderBy(bi => bi.IssueDate);
                }
                if (type == "return")
                {
                    query = query.Where(bi => bi.ReturnDate != null).OrderBy(bi => bi.ReturnDate);
                }
            }


            return query.Count();
        }


        public IEnumerable<BookIssue> GetIssueBookWithDateOrStudent(int? userId, DateTime? issuedate, int page, int pageSize)
        {
            int skip = (page - 1) * pageSize;

            IQueryable<BookIssue> query = _appDbContext.BookIssues
            .Include(bi => bi.Book)
            .Include(bi => bi.User);

            if (userId != null)
            {
                query = query.Where(bi => bi.UserId == userId);

            }

            if (issuedate != null)
            {
                var truncatedDate = issuedate.Value.Date;
                query = query.Where(bi => bi.IssueDate.Date == truncatedDate);
            }

            return query.
               Skip(skip)
               .Take(pageSize).
               ToList();
        }

        public int TotalBookCountWithDateOrStudent(int? userId, DateTime? issuedate)
        {
            IQueryable<BookIssue> query = _appDbContext.BookIssues;


            if (userId != null)
            {
                query = query.Where(bi => bi.UserId == userId);

            }

            if (issuedate != null)
            {
                var truncatedDate = issuedate.Value.Date;
                query = query.Where(bi => bi.IssueDate.Date == truncatedDate);
            }



            return query.Count();
        }


        public IEnumerable<BookIssue> GetUserWithBook(int bookId, string type, int page, int pageSize)
        {
            int skip = (page - 1) * pageSize;
            IQueryable<BookIssue> query = _appDbContext.BookIssues
            .Include(bi => bi.Book)
            .Include(bi => bi.User)
            .Where(bi => bi.BookId == bookId);

            if (type == "issue")
            {
                query = query.Where(bi => bi.ReturnDate == null);
            }
            if (type == "return")
            {
                query = query.Where(bi => bi.ReturnDate != null);
            }

            return query.
                Skip(skip)
                .Take(pageSize).
                ToList();


        }

        public int TotalUserCountWithBook(int bookId, string type)
        {
            IQueryable<BookIssue> query = _appDbContext.BookIssues.Where(bi => bi.BookId == bookId);

            if (type == "issue")
            {
                query = query.Where(bi => bi.ReturnDate == null);
            }
            if (type == "return")
            {
                query = query.Where(bi => bi.ReturnDate != null);
            }


            return query.Count();
        }
    }
}
