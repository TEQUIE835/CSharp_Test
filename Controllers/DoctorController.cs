using Microsoft.AspNetCore.Mvc;
using PruebaDesempenio.Infrastructure.Repositories;
using PruebaDesempenio.Models;

namespace PruebaDesempenio.Controllers;

public class DoctorController : Controller
{
    private DoctorRepository _repository;

    public DoctorController(DoctorRepository repository)
    {
        _repository = repository;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var doctors = await _repository.GetAll();
            return View(doctors);
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
            var doctor = await _repository.GetOne(id);
            return View(doctor);
        } catch (Exception e)
        {
            Console.WriteLine("Not handled exception: " + e.Message);
            return RedirectToAction("Index", "Home");
        }
    }

    public async Task<IActionResult> AddDoctor(Doctor doctor)
    {
        try
        {
            if (doctor == null)
            {
                throw new Exception("Patient needs to have all the parameters");
            } 
            await _repository.Create(doctor);
            TempData["CreateSuccess"] = "Doctor added";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception e)
        {
            TempData["CreateError"] = "Doctor could not be added: " + e.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var doctor = await _repository.GetOne(id);
            return Json(doctor);
        }
        catch (Exception e)
        {
            TempData["EditError"] = "Doctor could not be found: " + e.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    public async Task<IActionResult> Edit([FromBody] Doctor doctor)
    {
        try
        {
            await _repository.Update(doctor);
            return Ok(new {success = true, message = "Doctor updated"});
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
            TempData["DeleteSuccess"] = "Doctor deleted";
            return RedirectToAction("Index");
        }
        catch (Exception e)
        {
            TempData["DeleteError"] = $"There was an error deleting: {e.Message}";
            return RedirectToAction("Index");
        }
    }

    public async Task<IActionResult> Filter(string filter)
    {
        var doctors = await _repository.GetBySpeciality(filter);
        return View("Index", doctors);
    }
}