using System.ComponentModel.DataAnnotations;

namespace CivicaBookLibraryApi.Models
{
    public class Book
    {
        [Key]
        public int BookId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        public int TotalQuantity { get; set; }
        [Required]
        public int AvailableQuantity { get; set; }
        [Required]
        public int IssuedQuantity { get; set; }
        [Required]
        public decimal PricePerBook { get; set; }
        public ICollection<BookIssue> BookIssues { get; set; }
    }
}
