using Microsoft.EntityFrameworkCore;
using PruebaDesempenio.Domain.Interfaces;
using PruebaDesempenio.Infrastructure.Data;
using PruebaDesempenio.Infrastructure.Services;
using PruebaDesempenio.Models;

namespace PruebaDesempenio.Infrastructure.Repositories;

public class AppointmentRepository : IGetAll<Appointment>, ICreate<Appointment>, IDelete
{
    private AppDbContext _context;
    private EmailService _emailService;
    public AppointmentRepository(AppDbContext context,  EmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    public async Task<IEnumerable<Appointment>> GetAll()
    {
        return await _context.Appointments.Include(a => a.Patient)
            .Include(a => a.Doctor).ToListAsync();
    }

    public async Task CreateAppointment(AppointmentCreateDto appointment)
    {
        var patient = await _context.Patients.FirstOrDefaultAsync(p => p.Document == appointment.PatientDocument);
        if (patient == null) throw new Exception("Patient not found");
        var doctor = await _context.Doctors.FirstOrDefaultAsync(p => p.Document == appointment.DoctorDocument);
        if (doctor == null) throw new Exception("Doctor not found");
        await Create(new Appointment()
        {
            PatientId = patient.Id,
            DoctorId = doctor.Id,
            Date = appointment.Date,
            Hour = appointment.Hour
        });
    }
    
    public async Task Create(Appointment appointment)
    {
        var patient = await _context.Patients.FirstOrDefaultAsync(p => p.Id == appointment.PatientId);
        if (patient == null) throw new Exception("Patient not found");
        var doctor = await _context.Doctors.FirstOrDefaultAsync(p => p.Id == appointment.DoctorId);
        if (doctor == null) throw new Exception("Doctor not found");
        var patientAvailable= await _context.Appointments.AnyAsync(a => a.PatientId == appointment.PatientId && a.Date == appointment.Date && a.Hour == appointment.Hour && a.Status =="Pending");
        if (patientAvailable) throw new Exception("Patient not available at this time");
        var doctorAvailable = await _context.Appointments.AnyAsync(a => a.DoctorId == appointment.DoctorId && a.Date == appointment.Date && a.Hour == appointment.Hour && a.Status == "Pending");
        if (doctorAvailable) throw new Exception("Doctor not available at this time");
        await _context.Appointments.AddAsync(appointment);
        await _context.SaveChangesAsync();
        string subject = "Medic appointment confirmation";
        string body = $@"
                <h3>Hello {patient.Name},</h3>
                <p>You're appointment have been successfully scheduled.</p>
                <p><b>Date:</b> {appointment.Date}</p>
                <p><b>Doctor:</b> {doctor.Name}</p>
                <p>Thanks for trusting in us.</p>
            ";
        await _emailService.SendEmailAsync(patient.Email, subject, body);
    }

    public async Task Complete(int id)
    {
        var appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.Id == id);
        if (appointment == null) throw new Exception("Appointment not found");
        appointment.Status = "Attended";
        await _context.SaveChangesAsync();
    }

    public async Task Cancel(int id)
    {
        var appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.Id == id);
        if (appointment == null) throw new Exception("Appointment not found");
        appointment.Status = "Canceled";
        await _context.SaveChangesAsync();
    }
    
    public async Task Delete(int id)
    {
        var appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.Id == id);
        if (appointment == null) throw new Exception("Appointment not found");
        _context.Appointments.Remove(appointment);
        await _context.SaveChangesAsync();
    }
}