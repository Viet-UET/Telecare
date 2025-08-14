using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Telecare.Migrations
{
    /// <inheritdoc />
    public partial class appointment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CallAppointment",
                columns: table => new
                {
                    CallAppointmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Form = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DoctorId = table.Column<long>(type: "bigint", nullable: false),
                    PatientId = table.Column<long>(type: "bigint", nullable: false),
                    State = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CallAppointment", x => x.CallAppointmentId);
                    table.ForeignKey(
                        name: "FK_CallAppointment_Doctor_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctor",
                        principalColumn: "DoctorId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CallAppointment_Patient_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patient",
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HospitalAppointment",
                columns: table => new
                {
                    HospitalAppointmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HospitalId = table.Column<long>(type: "bigint", nullable: false),
                    PatientId = table.Column<long>(type: "bigint", nullable: false),
                    State = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HospitalAppointment", x => x.HospitalAppointmentId);
                    table.ForeignKey(
                        name: "FK_HospitalAppointment_Hospital_HospitalId",
                        column: x => x.HospitalId,
                        principalTable: "Hospital",
                        principalColumn: "HospitalId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HospitalAppointment_Patient_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patient",
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CallAppointment_DoctorId",
                table: "CallAppointment",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_CallAppointment_PatientId",
                table: "CallAppointment",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_HospitalAppointment_HospitalId",
                table: "HospitalAppointment",
                column: "HospitalId");

            migrationBuilder.CreateIndex(
                name: "IX_HospitalAppointment_PatientId",
                table: "HospitalAppointment",
                column: "PatientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CallAppointment");

            migrationBuilder.DropTable(
                name: "HospitalAppointment");
        }
    }
}
