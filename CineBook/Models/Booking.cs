using CineBook.Models;
using System.ComponentModel.DataAnnotations.Schema;

public class Booking
{
    public int Id { get; set; }

    public User user { get; set; }
    public string UserId { get; set; }

    public Movie movie { get; set; }
    public int MovieId { get; set; }

    // A booking can have multiple seats
    public List<Seat> BookedSeats { get; set; }

    
    public List<int> BookedSeatNumbers { get; set; }

    public bool IsCompleted { get; set; }

    public DateTime BookedAt { get; set; }


}
