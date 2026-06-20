using SQLite;

namespace tracktask.Models
{
    public class TripTask
    {
        [PrimaryKey, AutoIncrement]
        public int TripId { get; set; }

        public int UserId { get; set; }
        public int TrackId { get; set; }
        public bool IsDone { get; set; }
        public string Product { get; set; }
        public string Location { get; set; }
        public DateTime Date { get; set; }

        [Ignore]
        public string UserName { get; set; }

        [Ignore]
        public string TrackName { get; set; }
    }
}