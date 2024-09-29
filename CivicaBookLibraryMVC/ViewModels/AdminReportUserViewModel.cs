namespace CivicaBookLibraryMVC.ViewModels
{
    public class AdminReportUserViewModel
    {
        public int UserId { get; set; }

        public DateTime IssueDate { get; set; }

        public DateTime? ReturnDate { get; set; }

        public UserViewModel User { get; set; }

        public List<UserViewModel>? Users { get; set; }
        public List<BooksViewModel>? Books { get; set; }

    }
}
