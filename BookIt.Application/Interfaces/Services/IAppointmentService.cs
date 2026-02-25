using BookIt.Application.DTOs.Appointment;
using BookIt.Domain.Enums;

namespace BookIt.Application.Interfaces.Services
{
    public interface IAppointmentService
    {
        Task<AppointmentResponseDto> CancelAppointmentAsync(int appointmentId, int userId, string? cancelationMessage);
        Task<AppointmentResponseDto> CreateAppointmentAsync(CreateAppointmentDto appointmentDto, int userId);
        Task<AppointmentResponseDto> GetAppointmentByIdAsync(int id);
        Task<List<AppointmentResponseDto>> GetMyAppointmentsAsync(int userId);
        Task<List<AppointmentResponseDto>> GetTenantAppointmentsAsync(int tenantId);
        Task<AppointmentResponseDto> UpdateAppointmentStatusAsync(int appointmentId, int userId, AppointmentStatus status);
    }
}