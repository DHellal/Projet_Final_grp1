using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GrandHotel_WebApplication.Data.Migrations
{
    public partial class essai : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "Calendrier",
                columns: table => new
                {
                    Jour = table.Column<DateTime>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Calendrier", x => x.Jour);
                });

            migrationBuilder.CreateTable(
                name: "Chambre",
                columns: table => new
                {
                    Numero = table.Column<short>(nullable: false),
                    Bain = table.Column<bool>(nullable: false),
                    Douche = table.Column<bool>(nullable: true, defaultValueSql: "((1))"),
                    Etage = table.Column<byte>(nullable: false),
                    NbLits = table.Column<byte>(nullable: false),
                    NumTel = table.Column<short>(nullable: true),
                    WC = table.Column<bool>(nullable: true, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chambre", x => x.Numero);
                });

            migrationBuilder.CreateTable(
                name: "Client",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CarteFidelite = table.Column<bool>(nullable: false),
                    Civilite = table.Column<string>(unicode: false, maxLength: 4, nullable: false),
                    Email = table.Column<string>(maxLength: 40, nullable: false),
                    Nom = table.Column<string>(maxLength: 40, nullable: false),
                    Prenom = table.Column<string>(maxLength: 40, nullable: false),
                    Societe = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Client", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ModePaiement",
                columns: table => new
                {
                    Code = table.Column<string>(unicode: false, maxLength: 3, nullable: false),
                    Libelle = table.Column<string>(maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModePaiement", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Tarif",
                columns: table => new
                {
                    Code = table.Column<string>(unicode: false, maxLength: 20, nullable: false),
                    DateDebut = table.Column<DateTime>(type: "date", nullable: false),
                    Prix = table.Column<decimal>(type: "decimal(12, 3)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tarif", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Adresse",
                columns: table => new
                {
                    IdClient = table.Column<int>(nullable: false),
                    CodePostal = table.Column<string>(unicode: false, maxLength: 5, nullable: false),
                    Complement = table.Column<string>(maxLength: 40, nullable: true),
                    Rue = table.Column<string>(maxLength: 40, nullable: false),
                    Ville = table.Column<string>(maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Adresse", x => x.IdClient);
                    table.ForeignKey(
                        name: "FK_Adresse_Client",
                        column: x => x.IdClient,
                        principalTable: "Client",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reservation",
                columns: table => new
                {
                    NumChambre = table.Column<short>(nullable: false),
                    Jour = table.Column<DateTime>(type: "date", nullable: false),
                    HeureArrivee = table.Column<byte>(nullable: false, defaultValueSql: "((17))"),
                    IdClient = table.Column<int>(nullable: false),
                    NbPersonnes = table.Column<byte>(nullable: false),
                    Travail = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservation", x => new { x.NumChambre, x.Jour });
                    table.ForeignKey(
                        name: "FK_Reservation_Client",
                        column: x => x.IdClient,
                        principalTable: "Client",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reservation_Calendrier",
                        column: x => x.Jour,
                        principalTable: "Calendrier",
                        principalColumn: "Jour",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reservation_Chambre",
                        column: x => x.NumChambre,
                        principalTable: "Chambre",
                        principalColumn: "Numero",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Telephone",
                columns: table => new
                {
                    Numero = table.Column<string>(unicode: false, maxLength: 12, nullable: false),
                    CodeType = table.Column<string>(type: "char(1)", nullable: false),
                    IdClient = table.Column<int>(nullable: false),
                    Pro = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Telephone", x => x.Numero);
                    table.ForeignKey(
                        name: "FK_Telephone_Client",
                        column: x => x.IdClient,
                        principalTable: "Client",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Facture",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CodeModePaiement = table.Column<string>(unicode: false, maxLength: 3, nullable: false),
                    DateFacture = table.Column<DateTime>(type: "date", nullable: false),
                    DatePaiement = table.Column<DateTime>(type: "date", nullable: true),
                    IdClient = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Facture", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Facture_Paiement",
                        column: x => x.CodeModePaiement,
                        principalTable: "ModePaiement",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Facture_Client",
                        column: x => x.IdClient,
                        principalTable: "Client",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TarifChambre",
                columns: table => new
                {
                    NumChambre = table.Column<short>(nullable: false),
                    CodeTarif = table.Column<string>(unicode: false, maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TarifChambre", x => new { x.NumChambre, x.CodeTarif });
                    table.ForeignKey(
                        name: "FK_TarifChambre_Tarif",
                        column: x => x.CodeTarif,
                        principalTable: "Tarif",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TarifChambre_Chambre",
                        column: x => x.NumChambre,
                        principalTable: "Chambre",
                        principalColumn: "Numero",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LigneFacture",
                columns: table => new
                {
                    IdFacture = table.Column<int>(nullable: false),
                    NumLigne = table.Column<int>(nullable: false),
                    MontantHT = table.Column<decimal>(type: "decimal(12, 3)", nullable: false),
                    Quantite = table.Column<short>(nullable: false, defaultValueSql: "((1))"),
                    TauxReduction = table.Column<decimal>(type: "decimal(6, 3)", nullable: false, defaultValueSql: "((0))"),
                    TauxTVA = table.Column<decimal>(type: "decimal(6, 3)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LigneFacture", x => new { x.IdFacture, x.NumLigne });
                    table.ForeignKey(
                        name: "FK_LigneFacture_Facture",
                        column: x => x.IdFacture,
                        principalTable: "Facture",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Facture_CodeModePaiement",
                table: "Facture",
                column: "CodeModePaiement");

            migrationBuilder.CreateIndex(
                name: "IX_Facture_IdClient",
                table: "Facture",
                column: "IdClient");

            migrationBuilder.CreateIndex(
                name: "IDX_ReservationClient_FK",
                table: "Reservation",
                column: "IdClient");

            migrationBuilder.CreateIndex(
                name: "IX_Reservation_Jour",
                table: "Reservation",
                column: "Jour");

            migrationBuilder.CreateIndex(
                name: "IX_TarifChambre_CodeTarif",
                table: "TarifChambre",
                column: "CodeTarif");

            migrationBuilder.CreateIndex(
                name: "IX_Telephone_IdClient",
                table: "Telephone",
                column: "IdClient");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Adresse");

            migrationBuilder.DropTable(
                name: "LigneFacture");

            migrationBuilder.DropTable(
                name: "Reservation");

            migrationBuilder.DropTable(
                name: "TarifChambre");

            migrationBuilder.DropTable(
                name: "Telephone");

            migrationBuilder.DropTable(
                name: "Facture");

            migrationBuilder.DropTable(
                name: "Calendrier");

            migrationBuilder.DropTable(
                name: "Tarif");

            migrationBuilder.DropTable(
                name: "Chambre");

            migrationBuilder.DropTable(
                name: "ModePaiement");

            migrationBuilder.DropTable(
                name: "Client");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    PasswordHash = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    SecurityStamp = table.Column<string>(nullable: true),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_UserId",
                table: "AspNetUserRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);
        }
    }
}
