using Microsoft.EntityFrameworkCore;
using PruebaDesempenio.Domain.Interfaces;
using PruebaDesempenio.Infrastructure.Data;
using PruebaDesempenio.Models;

namespace PruebaDesempenio.Infrastructure.Repositories;

public class DoctorRepository : ICreate<Doctor>, IGetAll<Doctor>, IGetOne<Doctor>, IUpdate<Doctor>, IDelete
{
    private AppDbContext _context;
    public DoctorRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task Create(Doctor obj)
    {
        var documentExists = await _context.Doctors.AnyAsync(d => d.Document == obj.Document);
        if (documentExists) throw new Exception("Document Already Exists");
        var patientExists = await _context.Patients.AnyAsync(p => p.Document == obj.Document && p.Name != obj.Name);
        if (patientExists) throw new Exception("Document Already Exists for a patient with other name");
        await _context.Doctors.AddAsync(obj);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Doctor>> GetAll()
    {
        return await _context.Doctors.ToListAsync();
    }

    public async Task<Doctor> GetOne(int id)
    {
        var doctor = await _context.Doctors
            .Include(d => d.Appointments)
            .ThenInclude(a => a.Patient)
            .FirstOrDefaultAsync(d => d.Id == id);
        return doctor;
    }

    public async Task Update(Doctor obj)
    {
        var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Id == obj.Id);
        if (doctor == null) throw new Exception("Doctor Not Found");
        var documentExists = await _context.Doctors.AnyAsync(d => d.Document == obj.Document && d.Id != obj.Id);
        if (documentExists) throw new Exception("Document Already Exists");
        doctor.Name = obj.Name;
        doctor.Document = obj.Document;
        doctor.Email = obj.Email;
        doctor.Phone = obj.Phone;
        doctor.Speciality = obj.Speciality;
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var doctor = await _context.Doctors.FirstOrDefaultAsync(p => p.Id == id);
        if (doctor == null) throw new Exception("Patient Not Found");
        _context.Doctors.Remove(doctor);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Doctor>> GetBySpeciality(string speciality)
    {
        return await _context.Doctors.Where(d => d.Speciality == speciality).ToListAsync();
    }
}