using BookIt.Domain.Entities;
using BookIt.Domain.Enums;
using Xunit;

namespace BookIt.Tests.Services
{
    public class AvailabilityServiceTests : BookItBaseUnitTest
    {
        // FutureMonday: 09:00 is taken by PendingAppointment, 10:00 is free

        #region GetAvailableSlots

        [Fact]
        public async Task GetAvailableSlots_WorkingDayWithFreeSlots_ReturnsAvailableSlots()
        {
            var result = await AvailabilityService.GetAvailableSlotsAsync(OwnerTenant.Id, ActiveService.Id, FutureMonday);

            Assert.NotNull(result);
            Assert.Single(result); // 09:00 is taken, only 10:00 is free
            Assert.Equal(new TimeOnly(10, 0), result[0].StartTime);
            Assert.Equal(new TimeOnly(11, 0), result[0].EndTime); // 60 min duration
        }

        [Fact]
        public async Task GetAvailableSlots_ServiceNotFound_ThrowsKeyNotFoundException()
        {
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                AvailabilityService.GetAvailableSlotsAsync(OwnerTenant.Id, 9999, FutureMonday));
        }

        [Fact]
        public async Task GetAvailableSlots_TenantNotFound_ThrowsKeyNotFoundException()
        {
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                AvailabilityService.GetAvailableSlotsAsync(9999, ActiveService.Id, FutureMonday));
        }

        [Fact]
        public async Task GetAvailableSlots_NonWorkingDay_ThrowsInvalidOperationException()
        {
            // FutureMonday + 6 = Sunday (DayOfWeek = 0), not seeded as working day
            var sunday = FutureMonday.AddDays(6);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                AvailabilityService.GetAvailableSlotsAsync(OwnerTenant.Id, ActiveService.Id, sunday));
        }

        [Fact]
        public async Task GetAvailableSlots_NoTimeSlotsForDay_ThrowsInvalidOperationException()
        {
            // Wednesday (DayOfWeek=3) is a working day but ActiveService has no time slots for it
            var wednesday = FutureMonday.AddDays(2);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                AvailabilityService.GetAvailableSlotsAsync(OwnerTenant.Id, ActiveService.Id, wednesday));
        }

        [Fact]
        public async Task GetAvailableSlots_AllSlotsTaken_ReturnsEmptyList()
        {
            // FutureMonday + 21 is a free Monday - book both slots in this test
            var targetDate = FutureMonday.AddDays(21);

            Context.Appointments.AddRange(
                new Appointment { UserId = ClientUser.Id, TenantId = OwnerTenant.Id, ServiceId = ActiveService.Id, Date = targetDate, StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(10, 0), Status = AppointmentStatus.Confirmed, CreatedAt = DateTime.UtcNow },
                new Appointment { UserId = ClientUser.Id, TenantId = OwnerTenant.Id, ServiceId = ActiveService.Id, Date = targetDate, StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(11, 0), Status = AppointmentStatus.Pending, CreatedAt = DateTime.UtcNow }
            );
            Context.SaveChanges();

            var result = await AvailabilityService.GetAvailableSlotsAsync(OwnerTenant.Id, ActiveService.Id, targetDate);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        #endregion
    }
}
