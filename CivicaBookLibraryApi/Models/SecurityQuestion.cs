using System.ComponentModel.DataAnnotations;

namespace CivicaBookLibraryApi.Models
{
    public class SecurityQuestion
    {
        [Key]
        public int PasswordHint { get; set; }

        public string Question { get; set; }

        public ICollection<User> Users { get; set; }

    }
}
