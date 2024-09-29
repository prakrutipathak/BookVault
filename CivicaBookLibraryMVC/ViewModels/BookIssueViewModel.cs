using System.ComponentModel.DataAnnotations;

namespace CivicaBookLibraryMVC.ViewModels
{
    public class BookIssueViewModel
    {
        public DateTime? ReturnDate { get; set; }

        //Foreign Key
        public int UserId { get; set; }
        [Required(ErrorMessage = "Book is required.")]
        public int BookId { get; set; }
        public List<BooksViewModel>? Books { get; set; }
    }
}
