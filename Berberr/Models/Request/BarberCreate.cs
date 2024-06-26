﻿using Barber.Models.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Barber.Models.Request
{
    public class BarberCreate
    {
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public string? UserName { get; set; }
        public string? WorkPlaceName { get; set; }
        public string? Mail { get; set; }
        public required string Password { get; set; } 
        public string? Phone { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string? Street { get; set; }
        public string? BuildingNo { get; set; }
        public string? DoorNumber { get; set; }
        public string? TaxNo { get; set; }
 
    }
}
