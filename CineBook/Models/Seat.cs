namespace CineBook.Models
{
    public class Seat
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public bool IsAvailable { get; set; }
        public int MovieId { get; set; }
        public Movie Movie { get; set; }

        public int SeatPrice { get; set; } = 10;

        public int? BookingId { get; set; } // Nullable to avoid constraint issues
        public Booking? Booking { get; set; } // Reference to Booking (optional)
    }
}
