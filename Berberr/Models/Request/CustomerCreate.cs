﻿namespace Barber.Models.Request
{
    public class CustomerCreate
    {
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public string? Age { get; set; }
        public bool? Gender { get; set; }
        public string? UserName { get; set; }
        public string? Mail { get; set; }
        public string? Password { get; set; }
        public string? Phone { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string? Street { get; set; }
        //public IFormFile? CustomerFile { get; set; }
    }
}
