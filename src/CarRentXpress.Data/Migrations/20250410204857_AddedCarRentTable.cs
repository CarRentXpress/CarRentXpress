using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarRentXpress.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedCarRentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarRent_AspNetUsers_UserId",
                table: "CarRent");

            migrationBuilder.DropForeignKey(
                name: "FK_CarRent_Cars_CarId",
                table: "CarRent");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CarRent",
                table: "CarRent");

            migrationBuilder.RenameTable(
                name: "CarRent",
                newName: "CarsRents");

            migrationBuilder.RenameIndex(
                name: "IX_CarRent_UserId",
                table: "CarsRents",
                newName: "IX_CarsRents_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_CarRent_CarId",
                table: "CarsRents",
                newName: "IX_CarsRents_CarId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DeletedAt",
                table: "Cars",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DeletedAt",
                table: "CarsRents",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CarsRents",
                table: "CarsRents",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CarsRents_AspNetUsers_UserId",
                table: "CarsRents",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CarsRents_Cars_CarId",
                table: "CarsRents",
                column: "CarId",
                principalTable: "Cars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarsRents_AspNetUsers_UserId",
                table: "CarsRents");

            migrationBuilder.DropForeignKey(
                name: "FK_CarsRents_Cars_CarId",
                table: "CarsRents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CarsRents",
                table: "CarsRents");

            migrationBuilder.RenameTable(
                name: "CarsRents",
                newName: "CarRent");

            migrationBuilder.RenameIndex(
                name: "IX_CarsRents_UserId",
                table: "CarRent",
                newName: "IX_CarRent_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_CarsRents_CarId",
                table: "CarRent",
                newName: "IX_CarRent_CarId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DeletedAt",
                table: "Cars",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DeletedAt",
                table: "CarRent",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CarRent",
                table: "CarRent",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CarRent_AspNetUsers_UserId",
                table: "CarRent",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CarRent_Cars_CarId",
                table: "CarRent",
                column: "CarId",
                principalTable: "Cars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
