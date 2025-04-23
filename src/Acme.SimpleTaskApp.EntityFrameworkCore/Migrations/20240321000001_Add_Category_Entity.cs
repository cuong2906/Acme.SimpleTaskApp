using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Acme.SimpleTaskApp.EntityFrameworkCore.Migrations
{
    public partial class Add_Category_Entity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Tạo bảng Categories
            migrationBuilder.CreateTable(
                name: "AppCategories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 256, nullable: false),
                    Description = table.Column<string>(maxLength: 65536, nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppCategories", x => x.Id);
                });

            // Thêm một số danh mục mặc định
            migrationBuilder.InsertData(
                table: "AppCategories",
                columns: new[] { "Name", "Description", "CreationTime" },
                values: new object[,]
                {
                    { "Điện thoại", "Danh mục điện thoại di động", DateTime.Now },
                    { "Laptop", "Danh mục máy tính xách tay", DateTime.Now },
                    { "Phụ kiện", "Danh mục phụ kiện điện tử", DateTime.Now }
                });

            // Tạm thời cho phép Category_Id null
            migrationBuilder.AlterColumn<int>(
                name: "Category_Id",
                table: "AppProducts",
                nullable: true);

            // Cập nhật Category_Id cho các sản phẩm hiện có
            migrationBuilder.Sql(@"
                UPDATE AppProducts 
                SET Category_Id = (SELECT TOP 1 Id FROM AppCategories WHERE Name = 'Điện thoại')
                WHERE Category_Id IS NULL OR Category_Id NOT IN (SELECT Id FROM AppCategories)
            ");

            // Thêm ràng buộc khóa ngoại
            migrationBuilder.AddForeignKey(
                name: "FK_AppProducts_AppCategories_Category_Id",
                table: "AppProducts",
                column: "Category_Id",
                principalTable: "AppCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppProducts_AppCategories_Category_Id",
                table: "AppProducts");

            migrationBuilder.DropTable(
                name: "AppCategories");

            migrationBuilder.AlterColumn<int>(
                name: "Category_Id",
                table: "AppProducts",
                nullable: false);
        }
    }
} 