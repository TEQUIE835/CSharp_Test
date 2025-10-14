namespace PruebaDesempenio.Models;

public class Patient : User
{
    public DateOnly BirthDate { get; set; } = DateOnly.Parse(DateTime.Now.ToString("dd/MM/yyyy"));
    
    public List<Appointment> Appointments { get; set; }
}