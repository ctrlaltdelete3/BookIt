using BookIt.Application.DTOs.Appointment;

namespace BookIt.Application.Interfaces.Services
{
    public interface IAppointmentService
    {
        Task<AppointmentResponseDto> CancelAppointmentAsync(int appointmentId, int userId, CancelAppointmentDto cancelAppointmentDto);
        Task<AppointmentResponseDto> CreateAppointmentAsync(CreateAppointmentDto appointmentDto, int userId);
        Task<AppointmentResponseDto> GetAppointmentByIdAsync(int appointmentId, int userId);
        Task<List<AppointmentResponseDto>> GetMyAppointmentsAsync(int userId);
        Task<List<AppointmentResponseDto>> GetTenantAppointmentsAsync(int userId);
        Task<AppointmentResponseDto> UpdateAppointmentStatusAsync(int appointmentId, int userId, UpdateAppointmentDto updateAppointmentDto);
    }
}