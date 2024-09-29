using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CivicaBookLibraryApi.Migrations
{
    public partial class seedBook : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "BookId", "Title", "Author", "TotalQuantity", "AvailableQuantity", "IssuedQuantity", "PricePerBook" },
                values: new object[,]
                {
                   { 1, "To Kill a Mockingbird", "Harper Lee", 10, 10, 0, 19.99m },
                   { 2, "1984", "George Orwell", 8, 8, 0, 24.99m },
                   { 3, "The Great Gatsby", "F. Scott Fitzgerald", 15, 15, 0, 14.99m },
                  { 4, "Pride and Prejudice", "Jane Austen", 5, 5, 0, 29.99m },
                  { 5, "The Catcher in the Rye", "J.D. Salinger", 12, 12, 0, 17.99m }
                });

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
              table: "Books",
              keyColumn: "BookId",
              keyValues: new object[]
              {
                    1, 2, 3,4,5
              });

        }
    }
}
