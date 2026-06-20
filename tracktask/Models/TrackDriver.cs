using SQLite;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace tracktask.Models
{
    public class TrackDriver
    {
        [PrimaryKey, AutoIncrement]
        public int TrackDriverId { get; set; }

        public int UserId { get; set; }
        public int TrackId { get; set; }
    }
}