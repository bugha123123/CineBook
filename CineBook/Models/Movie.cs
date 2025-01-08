namespace CineBook.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
        public string MovieImage { get; set; }
        public string Description { get; set; }


        public  Double Rating { get; set; }
        public  int RunTime { get; set; } 
        public DateTime ReleaseDate { get; set; }
        public ICollection<Seat> Seats { get; set; }
        public DateTime ShowTime { get; set; }

           
    }
}
