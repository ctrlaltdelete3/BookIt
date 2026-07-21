namespace BookIt.Domain.Exceptions
{
    public class DuplicateAppointmentException : Exception
    {
        public DuplicateAppointmentException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
