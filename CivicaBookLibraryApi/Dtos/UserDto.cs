using CivicaBookLibraryApi.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CivicaBookLibraryApi.Dtos
{
    public class UserDto
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Login id is require")]
        [StringLength(15)]
        [DisplayName("Login Id")]
        public string LoginId { get; set; }

        [Required]
        public string Salutation { get; set; }

        [Required(ErrorMessage = "Name is require")]
        [StringLength(15)]
        [DisplayName("Name")]
        public string Name { get; set; }

        [Range(18, 120, ErrorMessage = "Age must be between 18 and 120.")]
        public int? Age { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Email address is require")]
        [StringLength(50)]
        [EmailAddress]
        [RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$", ErrorMessage = "Invalid email format.")]
        [DisplayName("Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is require")]
        [StringLength(15)]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?\d{3}\)?[-.\s]?\d{3}[-.\s]?\d{4}$", ErrorMessage = "Invalid contact number.")]
        [DisplayName("Phone Number")]
        public string PhoneNumber { get; set; }

        [Required]
        [DisplayName("PasswordHint")]
        public int PasswordHint { get; set; }

        [Required]
        public bool IsAdmin { get; set; }

        public SecurityQuestionDto SecurityQuestionDto { get; set; }

        public ICollection<BookIssue> BookIssues { get; set; }
    }
}
