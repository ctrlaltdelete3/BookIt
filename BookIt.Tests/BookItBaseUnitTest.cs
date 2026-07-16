using BookIt.Application.Interfaces.Services;
using BookIt.DAL.Context;
using BookIt.DAL.Repositories;
using BookIt.Domain.Entities;
using BookIt.Domain.Enums;
using BookIt.Services.Implementations;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace BookIt.Tests
{
    public abstract class BookItBaseUnitTest
    {
        //TODO: Add unit tests for validators!!

        protected const string ValidPassword = "Password123!";
        protected BookItDbContext Context { get; }

        protected static readonly DateOnly FutureMonday = new DateOnly(2026, 6, 1);

        protected Mock<IJwtService> JwtServiceMock { get; }

        protected User OwnerUser { get; private set; } = null!;
        protected User ClientUser { get; private set; } = null!;
        protected User ClientUser2 { get; private set; } = null!;
        protected User OwnerUser2 { get; private set; } = null!;
        protected Tenant OwnerTenant { get; private set; } = null!;
        protected Tenant OtherTenant { get; private set; } = null!;
        protected Service ActiveService { get; private set; } = null!;
        protected Service InactiveService { get; private set; } = null!;
        protected Appointment PendingAppointment { get; private set; } = null!;
        protected Appointment ConfirmedAppointment { get; private set; } = null!;
        protected Appointment CanceledAppointment { get; private set; } = null!;

        // repositories
        protected UserRepository UserRepository { get; private set; } = null!;
        protected TenantRepository TenantRepository { get; private set; } = null!;
        protected ServiceRepository ServiceRepository { get; private set; } = null!;
        protected WorkingHourRepository WorkingHourRepository { get; private set; } = null!;
        protected AppointmentRepository AppointmentRepository { get; private set; } = null!;
        protected RefreshTokenRepository RefreshTokenRepository { get; private set; } = null!;

        // services
        protected UserService UserService { get; private set; } = null!;
        protected TenantService TenantService { get; private set; } = null!;
        protected ServiceService ServiceService { get; private set; } = null!;
        protected WorkingHourService WorkingHourService { get; private set; } = null!;
        protected AvailabilityService AvailabilityService { get; private set; } = null!;
        protected AppointmentService AppointmentService { get; private set; } = null!;
        protected RefreshTokenService RefreshTokenService { get; private set; } = null!;


        protected BookItBaseUnitTest()
        {
            Context = CreateContext();

            JwtServiceMock = new Mock<IJwtService>();
            JwtServiceMock.Setup(j => j.GenerateToken(It.IsAny<User>())).Returns("fake-token");

            SeedData();
            SetupRepositories();
            SetupServices();
        }

        private BookItDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<BookItDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new BookItDbContext(options);
        }

        private void SeedData()
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(ValidPassword);

            // users
            OwnerUser = new User { FirstName = "Owner", LastName = "User", Email = "owner@test.com", PasswordHash = passwordHash, IsTenantOwner = true };
            OwnerUser2 = new User { FirstName = "Other", LastName = "Owner", Email = "other@test.com", PasswordHash = passwordHash, IsTenantOwner = true };
            ClientUser = new User { FirstName = "Client", LastName = "User", Email = "client@test.com", PasswordHash = passwordHash, IsTenantOwner = false };
            ClientUser2 = new User { FirstName = "Client2", LastName = "User2", Email = "client2@test.com", PasswordHash = passwordHash, IsTenantOwner = false };

            Context.Users.AddRange(OwnerUser, OwnerUser2, ClientUser, ClientUser2);
            Context.SaveChanges();

            // tenants
            OwnerTenant = new Tenant
            {
                Name = "Test Salon",
                ActivityType = "Frizerski salon",
                Slug = "test-salon-abc123",
                OwnerUserId = OwnerUser.Id,
                IsActive = true,
                Configuration = new TenantConfiguration
                {
                    AllowCancellation = true,
                    CancellationHoursBefore = 24
                },
                WorkingHours = new List<WorkingHour>
                {
                    new WorkingHour { DayOfWeek = 1, IsWorkingDay = true, StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(16, 0) },
                    new WorkingHour { DayOfWeek = 2, IsWorkingDay = true, StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(16, 0) },
                    new WorkingHour { DayOfWeek = 3, IsWorkingDay = true, StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(16, 0) },
                    new WorkingHour { DayOfWeek = 4, IsWorkingDay = true, StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(16, 0) },
                    new WorkingHour { DayOfWeek = 5, IsWorkingDay = true, StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(16, 0) },
                }
            };

            OtherTenant = new Tenant
            {
                Name = "Other Salon",
                ActivityType = "Kozmeticki salon",
                Slug = "other-salon-xyz",
                OwnerUserId = OwnerUser2.Id,
                IsActive = true
            };

            Context.Tenants.AddRange(OwnerTenant, OtherTenant);
            Context.SaveChanges();

            OwnerUser.OwnedTenantId = OwnerTenant.Id;
            OwnerUser2.OwnedTenantId = OtherTenant.Id;
            Context.SaveChanges();

            // services
            ActiveService = new Service
            {
                Name = "Haircut",
                Description = "Standard haircut",
                DurationMinutes = 60,
                BreakMinutesAfterService = 0,
                Price = 20m,
                IsActive = true,
                TenantId = OwnerTenant.Id,
                TimeSlots = new List<ServiceTimeSlot>
                {
                    new ServiceTimeSlot { DayOfWeek = 1, StartTime = new TimeOnly(9, 0), IsActive = true },
                    new ServiceTimeSlot { DayOfWeek = 1, StartTime = new TimeOnly(10, 0), IsActive = true }
                }
            };

            InactiveService = new Service
            {
                Name = "Deleted Service",
                DurationMinutes = 30,
                BreakMinutesAfterService = 0,
                Price = 10m,
                IsActive = false,
                TenantId = OwnerTenant.Id
            };

            Context.Services.AddRange(ActiveService, InactiveService);
            Context.SaveChanges();

            //appointments
            PendingAppointment = new Appointment
            {
                UserId = ClientUser.Id,
                TenantId = OwnerTenant.Id,
                ServiceId = ActiveService.Id,
                Date = FutureMonday,
                StartTime = new TimeOnly(9, 0),
                EndTime = new TimeOnly(10, 0),
                Status = AppointmentStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            ConfirmedAppointment = new Appointment
            {
                UserId = ClientUser.Id,
                TenantId = OwnerTenant.Id,
                ServiceId = ActiveService.Id,
                Date = FutureMonday.AddDays(7),
                StartTime = new TimeOnly(9, 0),
                EndTime = new TimeOnly(10, 0),
                Status = AppointmentStatus.Confirmed,
                CreatedAt = DateTime.UtcNow
            };

            CanceledAppointment = new Appointment
            {
                UserId = ClientUser.Id,
                TenantId = OwnerTenant.Id,
                ServiceId = ActiveService.Id,
                Date = FutureMonday.AddDays(14),
                StartTime = new TimeOnly(9, 0),
                EndTime = new TimeOnly(10, 0),
                Status = AppointmentStatus.Canceled,
                CreatedAt = DateTime.UtcNow,
                CanceledAt = DateTime.UtcNow
            };

            Context.Appointments.AddRange(PendingAppointment, ConfirmedAppointment, CanceledAppointment);
            Context.SaveChanges();
        }

        private void SetupRepositories()
        {
            UserRepository = new UserRepository(Context);
            TenantRepository = new TenantRepository(Context);
            ServiceRepository = new ServiceRepository(Context);
            WorkingHourRepository = new WorkingHourRepository(Context);
            AppointmentRepository = new AppointmentRepository(Context);
            RefreshTokenRepository = new RefreshTokenRepository(Context);
        }

        private void SetupServices()
        {
            RefreshTokenService = new RefreshTokenService(RefreshTokenRepository, JwtServiceMock.Object);
            UserService = new UserService(UserRepository, JwtServiceMock.Object, RefreshTokenService);
            TenantService = new TenantService(TenantRepository, UserRepository);
            ServiceService = new ServiceService(ServiceRepository, TenantRepository);
            WorkingHourService = new WorkingHourService(WorkingHourRepository, TenantRepository);
            AvailabilityService = new AvailabilityService(AppointmentRepository, TenantRepository, ServiceRepository);
            AppointmentService = new AppointmentService(AppointmentRepository, TenantRepository, AvailabilityService);
        }
    }
}
