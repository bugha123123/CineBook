namespace CineBook.Models
{
    public class Booking
    {

        public  int Id { get; set; }

        public User user { get; set; }

        public string UserId { get; set; }

        public Movie movie { get; set; }

        public int MovieId { get; set; }


        public List<Seat> BookedSeats { get; set; }

        
    }
}
