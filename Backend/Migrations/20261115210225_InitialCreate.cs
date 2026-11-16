using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    match_key = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    brand = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    category = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_products", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "retailers",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    url = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_retailers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "product_weights",
                columns: table => new
                {
                    product_id = table.Column<int>(type: "integer", nullable: false),
                    size = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    weight = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_product_weights", x => new { x.product_id, x.size });
                    table.ForeignKey(
                        name: "fk_product_weights_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "retailer_products",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    sku = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    product_url = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    product_image_url = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    price_cents = table.Column<int>(type: "integer", nullable: false),
                    rating = table.Column<double>(type: "double precision", nullable: false),
                    review_count = table.Column<int>(type: "integer", nullable: false),
                    weight_raw = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    category_raw = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    product_id = table.Column<int>(type: "integer", nullable: false),
                    retailer_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_retailer_products", x => x.id);
                    table.ForeignKey(
                        name: "fk_retailer_products_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_retailer_products_retailers_retailer_id",
                        column: x => x.retailer_id,
                        principalTable: "retailers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_retailer_products_product_id",
                table: "retailer_products",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "ix_retailer_products_retailer_id_product_url",
                table: "retailer_products",
                columns: new[] { "retailer_id", "product_url" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "product_weights");

            migrationBuilder.DropTable(
                name: "retailer_products");

            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "retailers");
        }
    }
}
