namespace PruebaDesempenio.Models;

public class Appointment
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly Hour { get; set; }
    public string Status { get; set; } = "Pending";
    public Patient Patient { get; set; }
    public Doctor Doctor { get; set; }
}