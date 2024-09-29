using CivicaBookLibraryApi.Models;

namespace CivicaBookLibraryApi.Dtos
{
    public class AdminReportUserDto
    {
        public int UserId { get; set; }

        public DateTime IssueDate { get; set; }
        public DateTime? ReturnDate { get; set; }

        public User User { get; set; }

    }

}
