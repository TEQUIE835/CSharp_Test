namespace PruebaDesempenio.Models;

public class AppointmentCreateDto
{
    public string PatientDocument { get; set; }
    public string DoctorDocument { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly Hour { get; set; }
}