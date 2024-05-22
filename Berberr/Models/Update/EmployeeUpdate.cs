namespace Barber.Models.Update
{
    public class EmployeeUpdate
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public IFormFile EmployeeFile { get; set; }
    }
}
