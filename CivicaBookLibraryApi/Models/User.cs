using System.ComponentModel.DataAnnotations;

namespace CivicaBookLibraryApi.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(15)]
        public string LoginId { get; set; }

        [Required]
        public string Salutation { get; set; }

        [StringLength(50, ErrorMessage = "Name cannot be longer than 50 characters.")]
        [MinLength(5, ErrorMessage = "Name must be at least 5 characters long.")]
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [Required]
        public int Age { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        [StringLength(50)]
        public string Email { get; set; }

        [Required]
        [StringLength(12)]
        public string PhoneNumber { get; set; }

       
        [Required]
        public byte[] PasswordHash { get; set; }
        [Required]
        public byte[] PasswordSalt { get; set; }

        [Required]
        public int PasswordHint { get; set; }  
        
        [Required]
        public byte[] PasswordHintAnswerHash { get; set; } 
        
        [Required]
        public byte[] PasswordHintAnswerSalt { get; set; }

        [Required]
        public bool IsAdmin { get; set; } = false;

        public SecurityQuestion SecurityQuestion { get; set; }
        public ICollection<BookIssue> BookIssues { get; set; }


    }
}
