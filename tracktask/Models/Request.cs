using SQLite;

namespace tracktask.Models
{
    public class Request
    {
        [PrimaryKey, AutoIncrement]
        public int RequestId { get; set; }

        public int TripId { get; set; }

        public string DriverName { get; set; }

        public string Message { get; set; }

        public string Status { get; set; } = "Pending";
    }
}