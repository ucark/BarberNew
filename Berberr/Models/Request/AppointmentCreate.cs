namespace Barber.Models.Request
{
    public class AppointmentCreate
    {
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public int BarberID { get; set; }
        public int CustomerID { get; set; }
    }
}
