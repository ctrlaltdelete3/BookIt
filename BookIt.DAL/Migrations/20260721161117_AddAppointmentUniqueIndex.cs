using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookIt.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddAppointmentUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Appointments_TenantId",
                table: "Appointments");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_TenantId_ServiceId_Date_StartTime_EndTime",
                table: "Appointments",
                columns: new[] { "TenantId", "ServiceId", "Date", "StartTime", "EndTime" },
                unique: true,
                filter: "[Status] in (0,1)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Appointments_TenantId_ServiceId_Date_StartTime_EndTime",
                table: "Appointments");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_TenantId",
                table: "Appointments",
                column: "TenantId");
        }
    }
}
