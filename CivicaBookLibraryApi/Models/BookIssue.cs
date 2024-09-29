using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;

namespace CivicaBookLibraryApi.Models
{
    public class BookIssue
    {
        [Key]
        public int IssueId {  get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime? ReturnDate { get; set; }

        //Foreign Key
        public int UserId { get; set; }
        public int BookId { get; set; }
        //Navigation
        public Book Book { get; set; }
        public User User { get; set; }
    }
}
