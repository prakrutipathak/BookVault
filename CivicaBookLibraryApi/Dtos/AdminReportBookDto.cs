using CivicaBookLibraryApi.Models;

namespace CivicaBookLibraryApi.Dtos
{
    public class AdminReportBookDto
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public DateTime IssueDate { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }
    }
}
