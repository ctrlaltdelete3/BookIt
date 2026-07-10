using BookIt.Application.DTOs.Appointment;
using BookIt.Domain.Entities;
using BookIt.Domain.Enums;

namespace BookIt.Tests.Services
{
    public class AppointmentServiceTests : BookItBaseUnitTest
    {
        #region CreateAppointment

        [Fact]
        public async Task CreateAppointment_ValidData_ReturnsAppointmentResponseDto()
        {
            // FutureMonday + 21 is a free Monday with no existing appointments
            var dto = new CreateAppointmentDto
            {
                TenantId = OwnerTenant.Id,
                ServiceId = ActiveService.Id,
                Date = FutureMonday.AddDays(21),
                StartTime = new TimeOnly(9, 0)
            };

            var result = await AppointmentService.CreateAppointmentAsync(dto, ClientUser.Id);

            Assert.NotNull(result);
            Assert.Equal(AppointmentStatus.Pending, result.Status);
            Assert.Equal(dto.StartTime, result.StartTime);
            Assert.Equal(ClientUser.Id, result.UserId);
        }

        [Fact]
        public async Task CreateAppointment_SlotNotAvailable_ThrowsInvalidOperationException()
        {
            // FutureMonday 09:00 is already taken by PendingAppointment
            var dto = new CreateAppointmentDto
            {
                TenantId = OwnerTenant.Id,
                ServiceId = ActiveService.Id,
                Date = FutureMonday,
                StartTime = new TimeOnly(9, 0)
            };

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                AppointmentService.CreateAppointmentAsync(dto, ClientUser2.Id));
        }

        #endregion

        #region GetAppointmentById

        [Fact]
        public async Task GetAppointmentById_AuthorizedUser_ReturnsAppointmentResponseDto()
        {
            var result = await AppointmentService.GetAppointmentByIdAsync(PendingAppointment.Id, ClientUser.Id);

            Assert.NotNull(result);
            Assert.Equal(PendingAppointment.Id, result.Id);
        }

        [Fact]
        public async Task GetAppointmentById_AppointmentNotFound_ThrowsKeyNotFoundException()
        {
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                AppointmentService.GetAppointmentByIdAsync(9999, ClientUser.Id));
        }

        [Fact]
        public async Task GetAppointmentById_NotAuthorized_ThrowsUnauthorizedAccessException()
        {
            // ClientUser2 is neither the appointment user (ClientUser) nor the tenant owner (OwnerUser)
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                AppointmentService.GetAppointmentByIdAsync(PendingAppointment.Id, ClientUser2.Id));
        }

        #endregion

        #region GetMyAppointments

        [Fact]
        public async Task GetMyAppointments_UserHasAppointments_ReturnsAppointmentList()
        {
            var result = await AppointmentService.GetMyAppointmentsAsync(ClientUser.Id);

            Assert.NotNull(result);
            Assert.True(result.Count > 0);
            Assert.DoesNotContain(result, a => a.Status == AppointmentStatus.Canceled);
        }

        [Fact]
        public async Task GetMyAppointments_NoAppointments_ReturnsEmptyList()
        {
            // ClientUser2 has no appointments
            var result = await AppointmentService.GetMyAppointmentsAsync(ClientUser2.Id);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        #endregion

        #region GetTenantAppointments

        [Fact]
        public async Task GetTenantAppointments_ValidOwner_ReturnsAppointmentList()
        {
            var result = await AppointmentService.GetTenantAppointmentsAsync(OwnerUser.Id);

            Assert.NotNull(result);
            Assert.True(result.Count > 0);
            Assert.DoesNotContain(result, a => a.Status == AppointmentStatus.Canceled);
        }

        [Fact]
        public async Task GetTenantAppointments_TenantNotFound_ThrowsKeyNotFoundException()
        {
            // ClientUser has no tenant
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                AppointmentService.GetTenantAppointmentsAsync(ClientUser.Id));
        }

        #endregion

        #region UpdateAppointmentStatus

        [Fact]
        public async Task UpdateAppointmentStatus_OwnerConfirmsAppointment_ReturnsConfirmedAppointment()
        {
            var dto = new UpdateAppointmentDto { Status = AppointmentStatus.Confirmed };

            var result = await AppointmentService.UpdateAppointmentStatusAsync(PendingAppointment.Id, OwnerUser.Id, dto);

            Assert.NotNull(result);
            Assert.Equal(AppointmentStatus.Confirmed, result.Status);
            Assert.NotNull(result.ConfirmedAt);
        }

        [Fact]
        public async Task UpdateAppointmentStatus_OwnerRejectsAppointment_ReturnsRejectedAppointment()
        {
            var dto = new UpdateAppointmentDto { Status = AppointmentStatus.Rejected, Note = "Not available" };

            var result = await AppointmentService.UpdateAppointmentStatusAsync(PendingAppointment.Id, OwnerUser.Id, dto);

            Assert.NotNull(result);
            Assert.Equal(AppointmentStatus.Rejected, result.Status);
            Assert.Equal(dto.Note, result.RejectionReason);
            Assert.NotNull(result.RejectedAt);
        }

        [Fact]
        public async Task UpdateAppointmentStatus_TenantNotFound_ThrowsKeyNotFoundException()
        {
            // ClientUser has no tenant
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                AppointmentService.UpdateAppointmentStatusAsync(PendingAppointment.Id, ClientUser.Id, new UpdateAppointmentDto { Status = AppointmentStatus.Confirmed }));
        }

        [Fact]
        public async Task UpdateAppointmentStatus_AppointmentBelongsToOtherTenant_ThrowsUnauthorizedAccessException()
        {
            // OtherOwnerUser's tenant doesn't own PendingAppointment
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                AppointmentService.UpdateAppointmentStatusAsync(PendingAppointment.Id, OwnerUser2.Id, new UpdateAppointmentDto { Status = AppointmentStatus.Confirmed }));
        }

        #endregion

        #region CancelAppointment

        [Fact]
        public async Task CancelAppointment_UserCancelsInTime_ReturnsCanceledAppointment()
        {
            // PendingAppointment is on FutureMonday (far in the future) - within cancellation window
            var dto = new CancelAppointmentDto { CancelationMessage = "Change of plans" };

            var result = await AppointmentService.CancelAppointmentAsync(PendingAppointment.Id, ClientUser.Id, dto);

            Assert.NotNull(result);
            Assert.Equal(AppointmentStatus.Canceled, result.Status);
            Assert.Equal(dto.CancelationMessage, result.CancellationReason);
        }

        [Fact]
        public async Task CancelAppointment_LateCancellation_SetsTenantNoteLateCancellation()
        {
            // Appointment in 1 hour - too late to cancel without fee (24h required)
            var soonAppointment = new Appointment
            {
                UserId = ClientUser.Id,
                TenantId = OwnerTenant.Id,
                ServiceId = ActiveService.Id,
                Date = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(1)),
                StartTime = TimeOnly.FromDateTime(DateTime.UtcNow.AddHours(1)),
                EndTime = TimeOnly.FromDateTime(DateTime.UtcNow.AddHours(2)),
                Status = AppointmentStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };
            Context.Appointments.Add(soonAppointment);
            Context.SaveChanges();

            var dto = new CancelAppointmentDto { CancelationMessage = "Emergency" };

            var result = await AppointmentService.CancelAppointmentAsync(soonAppointment.Id, ClientUser.Id, dto);

            Assert.Equal(AppointmentStatus.Canceled, result.Status);
            Assert.Equal("Late cancellation.", result.TenantNote);
        }

        [Fact]
        public async Task CancelAppointment_TenantOwnerCancels_ReturnsCanceledAppointment()
        {
            var dto = new CancelAppointmentDto { CancelationMessage = "Salon closed" };

            var result = await AppointmentService.CancelAppointmentAsync(PendingAppointment.Id, OwnerUser.Id, dto);

            Assert.NotNull(result);
            Assert.Equal(AppointmentStatus.Canceled, result.Status);
            Assert.Equal(dto.CancelationMessage, result.CancellationReason);
        }

        [Fact]
        public async Task CancelAppointment_UnauthorizedUser_ThrowsUnauthorizedAccessException()
        {
            // OtherOwnerUser owns a different tenant - not authorized to cancel this appointment
            var dto = new CancelAppointmentDto { CancelationMessage = "Whatever" };

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                AppointmentService.CancelAppointmentAsync(PendingAppointment.Id, OwnerUser2.Id, dto));
        }

        #endregion
    }
}
