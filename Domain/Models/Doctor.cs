namespace PruebaDesempenio.Models;

public class Doctor : User
{
    public string Speciality { get; set; }
    public List<Appointment> Appointments { get; set; }
}