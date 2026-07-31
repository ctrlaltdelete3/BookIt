using BookIt.Domain.Entities;
using BookIt.Domain.Enums;
using Xunit;

namespace BookIt.Tests.Services
{
    public class AvailabilityServiceTests : BookItBaseUnitTest
    {
        // Working hours are 08:00-16:00 (no pause), FutureMonday: 09:00-10:00 is taken by PendingAppointment

        #region GetAvailableSlots

        [Fact]
        public async Task GetAvailableSlots_WorkingDayWithFreeSlots_ReturnsAvailableSlots()
        {
            var result = await AvailabilityService.GetAvailableSlotsAsync(OwnerTenant.Id, ActiveService.Id, FutureMonday);

            Assert.NotNull(result);
            Assert.Equal(7, result.Count); // whole 08:00-16:00 day minus the 09:00-10:00 taken hour, 60 min slots
            Assert.DoesNotContain(result, s => s.StartTime == new TimeOnly(9, 0)); // taken
            Assert.Contains(result, s => s.StartTime == new TimeOnly(8, 0));
            Assert.Contains(result, s => s.StartTime == new TimeOnly(10, 0));
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
        public async Task GetAvailableSlots_WorkingDayWithNoAppointments_ReturnsFullDayOfSlots()
        {
            // Wednesday (DayOfWeek=3) is a working day with no appointments booked for ActiveService
            var wednesday = FutureMonday.AddDays(2);

            var result = await AvailabilityService.GetAvailableSlotsAsync(OwnerTenant.Id, ActiveService.Id, wednesday);

            Assert.NotNull(result);
            Assert.Equal(8, result.Count); // whole 08:00-16:00 working day, nothing booked, 60 min slots
            Assert.Contains(result, s => s.StartTime == new TimeOnly(8, 0));
            Assert.Contains(result, s => s.StartTime == new TimeOnly(15, 0));
        }

        [Fact]
        public async Task GetAvailableSlots_AllSlotsTaken_ReturnsEmptyList()
        {
            // FutureMonday + 21 is a free Monday - book the entire 08:00-16:00 working day in this test
            var targetDate = FutureMonday.AddDays(21);

            for (var hour = 8; hour < 16; hour++)
            {
                Context.Appointments.Add(new Appointment
                {
                    UserId = ClientUser.Id,
                    TenantId = OwnerTenant.Id,
                    ServiceId = ActiveService.Id,
                    Date = targetDate,
                    StartTime = new TimeOnly(hour, 0),
                    EndTime = new TimeOnly(hour + 1, 0),
                    Status = AppointmentStatus.Confirmed,
                    CreatedAt = DateTime.UtcNow
                });
            }
            Context.SaveChanges();

            var result = await AvailabilityService.GetAvailableSlotsAsync(OwnerTenant.Id, ActiveService.Id, targetDate);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        #endregion
    }
}
