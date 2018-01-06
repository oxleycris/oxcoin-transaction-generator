using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace OxCoin.TransactionGenerator.Migrations
{
    public partial class DecimalUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "TransferedAmount",
                table: "Transactions",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "TransferFee",
                table: "Transactions",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "TransferedAmount",
                table: "Transactions",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TransferFee",
                table: "Transactions",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");
        }
    }
}
