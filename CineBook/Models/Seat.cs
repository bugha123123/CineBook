using CineBook.Models;

public class Seat
{
    public int Id { get; set; }
    public int Number { get; set; }
    public bool IsAvailable { get; set; }
    public int MovieId { get; set; }
    public Movie Movie { get; set; }

    public int SeatPrice { get; set; } = 10;




}
