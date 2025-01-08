namespace CineBook.DTO
{
    public class SalesReport
    {
        public string DateRange { get; set; } 
        public int TotalBookedSeats { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
