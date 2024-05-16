using Barber.Models.DTO;
using Barber.Models.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Barber.Controllers
{
    [Route("API/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly BarberDbContext _context;

        public AppointmentController(BarberDbContext context)
        {
            _context = context;
        }

        [HttpGet("Get-Appointment")]
        public IActionResult GetAppointment()
        {
            try
            {
                var appointments = _context.Appointments.ToList();
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Hata: " + ex.Message);
            }
        }

        [HttpGet("Get-Appointment/{id}")]
        public IActionResult GetAppointmentById(int id)
        {
            try
            {
                var appointment = _context.Appointments.Find(id);
                return Ok(appointment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Hata: " + ex.Message);
            }
        }

        [HttpPost("Create-Appointment")]
        public IActionResult CreateAppointment([FromBody] AppointmentCreate appointmentCreate)
        {
            if (appointmentCreate == null)
            {
                return BadRequest("Geçersiz veri. Randevu verisi boş.");
            }
            var newAppointment = new Appointments
            {
                Date = appointmentCreate.Date,
                Time = appointmentCreate.Time,
                BarberID = appointmentCreate.BarberID,
                CustomerID = appointmentCreate.CustomerID,
            };
            try
            {
                _context.Appointments.Add(newAppointment);
                _context.SaveChanges();
                return Ok(newAppointment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Hata: " + ex.Message);
            }

        }
        [HttpPut("Update-Appointment")]
        IActionResult UpdateAppointment (int id, [FromBody] Appointments appointmentsUpdate)
        {
            var existingAppointment = _context.Appointments.Find(id);
            if (existingAppointment == null)
            {
                return BadRequest("Belirtilen türde randevu bulunamadı.");
            }
            existingAppointment.Time = appointmentsUpdate.Time;
            existingAppointment.Date = appointmentsUpdate.Date;
            existingAppointment.BarberID = appointmentsUpdate.BarberID;
            existingAppointment.CustomerID = appointmentsUpdate.CustomerID;

            try
            {
                _context.SaveChanges();
                return Ok(existingAppointment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Hata: " + ex.Message);
            }
        }
        [HttpDelete("Delete-Appointment/{id}")]
        public IActionResult DeleteAppointment(int id)
        {
            var existingAppointment = _context.Appointments.Find(id);
                {
                if (existingAppointment == null)
                {
                    return BadRequest("Belirtilen türde randevu bulunamadı.");
                }
                try
                {
                    _context.Appointments.Remove(existingAppointment);
                    _context.SaveChanges();
                    return Ok("Randevu başarıyla silindi.");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "Hata: " + ex.Message);
                }
            }
        }

    }
}
