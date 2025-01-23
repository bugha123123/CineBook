using CineBook.Models;

namespace CineBook.Interface
{
    public interface ISupportService
    {

        Task SubmitSupportTicket(string subject, string email,string Category, string message);

        Task<SupportTicket> GetSupportTicketById(string ticketId);
    }
}
