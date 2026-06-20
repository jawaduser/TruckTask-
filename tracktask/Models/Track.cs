using SQLite;
using System.ComponentModel;

namespace tracktask.Models
{
    public class Track
    {
        [PrimaryKey, AutoIncrement]
        public int TrackId { get; set; }

        public string Modules  { get; set; }
        public string Status { get; set; }
        
        public Boolean Availability { get; set; }
    }
}
