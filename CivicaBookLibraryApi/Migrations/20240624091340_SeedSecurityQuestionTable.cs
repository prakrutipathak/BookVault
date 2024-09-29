﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CivicaBookLibraryApi.Migrations
{
    public partial class SeedSecurityQuestionTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
               table: "SecurityQuestions",
               column: "Question",
               values: new object[]
               {
                    "What is your favourite colour?",
                    "What is your Birth City?",
                    "What is your Favourite holiday destination?",

               });

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
               table: "SecurityQuestions",
               keyColumn: "PasswordHint",
               keyValues: new object[]
               {
                    1, 2, 3
               });

        }
    }
}
