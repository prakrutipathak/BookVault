namespace CivicaBookLibraryApi.Dtos
{
    public class BookIssueDto
    {
        public DateTime? ReturnDate { get; set; }

        //Foreign Key
        public int UserId { get; set; }
        public int BookId { get; set; }
    }
}
