using Microsoft.AspNetCore.Mvc;
using PruebaDesempenio.Infrastructure.Repositories;
using PruebaDesempenio.Models;

namespace PruebaDesempenio.Controllers;

public class AppointmentController : Controller
{
    private AppointmentRepository _repository;
    public AppointmentController(AppointmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var appointments = await _repository.GetAll();
            return View(appointments);
        }
        catch (Exception e)
        {
            Console.WriteLine("Not handled exception: " + e.Message);
            return RedirectToAction("Index", "Home");
        }
    }

    public async Task<IActionResult> AddAppointment(AppointmentCreateDto appointment)
    {
        try
        {
            if (appointment.Date < DateOnly.Parse(DateTime.Now.ToString("dd/MM/yyyy")) ||
                appointment.Date == DateOnly.Parse(DateTime.Now.ToString("dd/MM/yyyy")) && appointment.Hour <= TimeOnly.Parse(DateTime.Now.ToString("HH:mm")))
            {
                throw new Exception("Invalid date");
            }
            await _repository.CreateAppointment(appointment);
            TempData["CreateSuccess"] = "Appointment created successfully";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception e)
        {
            TempData["CreateError"] = "Error creating: " + e.Message;
            return RedirectToAction("Index");
        }
    }

    public async Task<IActionResult> Complete(int id)
    {
        try
        {
            await _repository.Complete(id);
            TempData["EditSuccess"] = "Appointment completed successfully";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception e)
        {
            TempData["EditError"] = "Error completing: " + e.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    public async Task<IActionResult> Cancel(int id)
    {
        try
        {
            await _repository.Cancel(id);
            TempData["EditSuccess"] = "Appointment canceled successfully";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception e)
        {
            TempData["EditError"] = "Error canceling: " + e.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _repository.Delete(id);
            TempData["DeleteSuccess"] = "Appointment deleted successfully";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception e)
        {
            TempData["DeleteError"] = "Error deleting: " + e.Message;
            return RedirectToAction(nameof(Index));
        }
    }
}