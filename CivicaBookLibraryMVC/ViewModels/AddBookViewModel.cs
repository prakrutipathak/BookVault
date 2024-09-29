using System.ComponentModel.DataAnnotations;
using System.Xml.Schema;

namespace CivicaBookLibraryMVC.ViewModels
{
    public class AddBookViewModel
    {
        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; }
        [StringLength(25)]
        [Required(ErrorMessage = "Author is required.")]
        public string Author { get; set; }
        [Required(ErrorMessage = "Total quantity is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Total Quantity must be greater than or equal to 0")]
        public int TotalQuantity { get; set; }

        [Required(ErrorMessage = "Available quantity is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Available Quantity must be greater than or equal to 0")]
        public int AvailableQuantity { get; set; }
        [Required(ErrorMessage = "Available quantity is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Available Quantity must be greater than or equal to 0")]
        public int IssuedQuantity { get; set; }

        [Required(ErrorMessage = "Price per book is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Price Per Book must be greater than or equal to 0")]
        public decimal PricePerBook { get; set; }

    }
}
