using Microsoft.AspNetCore.Mvc;
using PruebaDesempenio.Infrastructure.Repositories;
using PruebaDesempenio.Models;

namespace PruebaDesempenio.Controllers;

public class PatientController : Controller
{
    private PatientRepository _repository;

    public PatientController(PatientRepository repository)
    {
        _repository = repository;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var patients = await _repository.GetAll();
            return View(patients);
        }
        catch (Exception e)
        {
            Console.WriteLine("Not handled exception: " + e.Message);
            return RedirectToAction("Index", "Home");
        }
        
    }

    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var patient = await _repository.GetOne(id);
            return View(patient);
        } catch (Exception e)
        {
            Console.WriteLine("Not handled exception: " + e.Message);
            return RedirectToAction("Index", "Home");
        }
    }

    public async Task<IActionResult> AddPatient(Patient patient)
    {
        try
        {
            if (patient == null)
            {
                throw new Exception("Patient needs to have all the parameters");
            } 
            else if (patient.BirthDate >= DateOnly.Parse(DateTime.Now.ToString("dd/MM/yyyy")))
            {
                throw new Exception("Invalid birth date");
            }
            await _repository.Create(patient);
            TempData["CreateSuccess"] = "Patient added";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception e)
        {
            TempData["CreateError"] = "Patient could not be added: " + e.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var patient = await _repository.Update(id);
            return Json(patient);
        }
        catch (Exception e)
        {
            TempData["EditError"] = "Error: " + e.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    public async Task<IActionResult> Edit([FromBody] Patient patient)
    {
        try
        {
            if (patient.BirthDate >= DateOnly.Parse(DateTime.Now.ToString("dd/MM/yyyy")))
            {
                throw new Exception("Invalid birth date");
            }
            await _repository.Update(patient);
            return Ok(new {success = true, message = "Patient updated"});
        }
        catch (Exception e)
        {
            return BadRequest(new {success = false, message = "There was an error updating" + e.Message});
        }
    }

    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _repository.Delete(id);
            TempData["DeleteSuccess"] = "Patient deleted";
            return RedirectToAction("Index");
        }
        catch (Exception e)
        {
            TempData["DeleteError"] = $"There was an error deleting: {e.Message}";
            return RedirectToAction("Index");
        }
    }
}