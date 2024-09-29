using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace CivicaBookLibraryApi.Dtos
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Login Id is required.")]
        [StringLength(15)]
        [DisplayName("Login Id")]
        public string LoginId { get; set; }

        [Required]
        public string Salutation { get; set; }

        [StringLength(50, ErrorMessage = "Name cannot be longer than 50 characters.")]
        [MinLength(5, ErrorMessage = "Name must be at least 5 characters long.")]
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }


        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public string Gender { get; set; }


      
        [Required(ErrorMessage = "Email address is required.")]
        [StringLength(50)]
        [EmailAddress]
        [RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$", ErrorMessage = "Invalid email format.")]
        [DisplayName("Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phonenumber is required.")]
        [StringLength(12)]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?\d{3}\)?[-.\s]?\d{3}[-.\s]?\d{4}$", ErrorMessage = "Invalid contact number.")]
        [DisplayName("PhoneNumber")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
ErrorMessage = "The password must be at least 8 characters long and contain at least 1 uppercase letter, 1 number, and 1 special character.")]
        [DisplayName("Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm password is required.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        [DisplayName("Confirm Password")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Password hint is required.")]
        public int PasswordHint { get; set; }

        [Required]
        public string PasswordHintAnswer { get; set; }

      


    }
}
