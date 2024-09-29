namespace CivicaBookLibraryMVC.ViewModels
{
    public class AdminReportBookViewModel
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public DateTime IssueDate { get; set; }

        public int UserId { get; set; }

        public UserViewModel User { get; set; }
        public List<UserViewModel>? Users { get; set; }
        public List<BooksViewModel>? Books { get; set; }
    
}

}
