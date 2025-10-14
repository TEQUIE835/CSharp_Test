using Microsoft.EntityFrameworkCore;
using PruebaDesempenio.Domain.Interfaces;
using PruebaDesempenio.Infrastructure.Data;
using PruebaDesempenio.Models;

namespace PruebaDesempenio.Infrastructure.Repositories;

public class PatientRepository : ICreate<Patient>, IGetAll<Patient>, IGetOne<Patient>, IUpdate<Patient>, IDelete
{
    private AppDbContext _context;
    public PatientRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task Create(Patient obj)
    {
        var documentExists = await _context.Patients.AnyAsync(p => p.Document == obj.Document);
        if (documentExists) throw new Exception("Document Already Exists");
        var doctorExists = await _context.Doctors.AnyAsync(d => d.Document == obj.Document && d.Name != obj.Name);
        if (doctorExists) throw new Exception("A doctor with that document already exists with a different name");
        await _context.Patients.AddAsync(obj);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Patient>> GetAll()
    {
        return await _context.Patients.ToListAsync();
    }

    public async Task<Patient> GetOne(int id)
    {
        var patient = await _context.Patients
            .Include(p => p.Appointments)
            .ThenInclude(a => a.Doctor)
            .FirstOrDefaultAsync(p => p.Id == id);
        return patient;
    }

    public async Task<Patient> Update(int id)
    {
        var patient = await _context.Patients.FirstOrDefaultAsync(p => p.Id == id);
        if (patient == null) throw new Exception("Patient Not Found");
        return patient;
    }

    public async Task Update(Patient obj)
    {
        var patient = await _context.Patients.FirstOrDefaultAsync(p => p.Id == obj.Id);
        if (patient == null) throw new Exception("Patient Not Found");
        var documentExists = await _context.Patients.AnyAsync(p => p.Document == obj.Document && p.Id != obj.Id);
        if (documentExists) throw new Exception("Document Already Exists");
        patient.Name = obj.Name;
        patient.Document = obj.Document;
        patient.Email = obj.Email;
        patient.Phone = obj.Phone;
        patient.BirthDate = obj.BirthDate;
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var patient = await _context.Patients.FirstOrDefaultAsync(p => p.Id == id);
        if (patient == null) throw new Exception("Patient Not Found");
        _context.Patients.Remove(patient);
        await _context.SaveChangesAsync();
    }
}