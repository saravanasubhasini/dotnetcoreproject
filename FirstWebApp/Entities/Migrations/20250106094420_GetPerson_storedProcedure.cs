using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class GetPerson_storedProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string sp_GetAllPersons = @"CREATE PROCEDURE [dbo].[GetAllPersons]
                AS BEGIN
                SELECT * FROM [dbo].[Persons]
                END";
            migrationBuilder.Sql(sp_GetAllPersons);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string sp_DropAllPerson = @"CREATE PROCEDURE [dbo].[DropAllPerson] AS BEGIN
                DROP TABLE [dbo].[Persons] END";
            migrationBuilder.Sql(sp_DropAllPerson);
        }
    }
}
