namespace Barber.Models.Request
{
    public class AppointmentCreate
    {
        public DateTime Date { get; set; }
        public string Time { get; set; }
        public int BarberID { get; set; }
        public int CustomerID { get; set; }
        public int ServiceID { get; set; }
        public string State { get; set; }

    }
}
