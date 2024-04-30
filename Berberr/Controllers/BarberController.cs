﻿using Barber.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace Barber.Controllers
{
    [Authorize(Policy = "RequireBarberLoggedIn")]
    [ApiController]
    [Route("api/[controller]")]
    public class BarberController : ControllerBase
    {
        private readonly BarberDbContext _context;

        public BarberController(BarberDbContext context)
        {
            _context = context;
        }
        

        [HttpGet("get-barbers")]
        public IActionResult GetBarbers()
        {
            try
            {
                var barbers = _context.Barbers.ToList();
                return Ok(barbers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Hata: " + ex.Message);
            }
        }
        [HttpGet("get-barber/{id}")]
        public IActionResult GetBarberById(int id)
        {
            try
            {
                var barber = _context.Barbers.Find(id);
                
                return Ok(barber);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Hata: " + ex.Message);
            }
        }

        [HttpPost("create-barber")]
        public IActionResult CreateBarber([FromBody] Barbers barberData)
        {
            if (barberData == null)
            {
                return BadRequest("Geçersiz veri: BarberCreate verisi boş.");
            }
            try
            {
                var newBarber = new Barbers
                {
                    UserName = barberData.UserName,
                    WorkPlaceName = barberData.WorkPlaceName,
                    Mail = barberData.Mail,
                    Password = barberData.Password,
                    Phone = barberData.Phone,
                    City = barberData.City,
                    District = barberData.District,
                    Street = barberData.Street,
                    BuildingNo = barberData.BuildingNo,
                    DoorNumber = barberData.DoorNumber,
                    TaxNo = barberData.TaxNo
                };

                _context.Barbers.Add(newBarber);
                _context.SaveChanges();
                return Ok(newBarber);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Hata: " + ex.Message);
            }
        }

        [HttpPut("update-barber/{id}")]
        public IActionResult UpdateBarber(int id, [FromBody] Barbers barberData)
        {
            var existingBarber = _context.Barbers.Find(id);
            if (existingBarber == null)
            {
                return NotFound("Belirtilen kimlik numarasına sahip bir berber bulunamadı.");
            }
            existingBarber.UserName = barberData.UserName;
            existingBarber.WorkPlaceName = barberData.WorkPlaceName;
            existingBarber.Mail = barberData.Mail;
            existingBarber.Password = barberData.Password;
            existingBarber.Phone = barberData.Phone;
            existingBarber.City = barberData.City;
            existingBarber.District = barberData.District;
            existingBarber.Street = barberData.Street;
            existingBarber.BuildingNo = barberData.BuildingNo;
            existingBarber.DoorNumber = barberData.DoorNumber;
            existingBarber.TaxNo = barberData.TaxNo;

            try
            {
                _context.SaveChanges();
                return Ok(existingBarber);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Hata: " + ex.Message);
            }
        }

        [HttpDelete("delete-barber/{id}")]
        public IActionResult DeleteBarber(int id)
        {
            var existingBarber = _context.Barbers.Find(id);
            if (existingBarber == null)
            {
                return NotFound("Belirtilen kimlik numarasına sahip bir berber bulunamadı.");
            }

            try
            {
                _context.Barbers.Remove(existingBarber);
                _context.SaveChanges();
                return Ok("Berber başarıyla silindi.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Hata: " + ex.Message);
            }
        }
    }
}
