using CineBook.DTO;
using CineBook.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel;
using System.Net.Sockets;

namespace CineBook.Interface
{
    public interface IAdminService
    { 
        Task<int> GetTotalRevenue();

        Task<int> GetTotalTicketsSold();

        Task<List<User>> GetTotalUsers();

        Task DeleteUserAsync(string userId);
        Task AddNewAdmin(string UserName, string Gmail, string Role);

        Task<List<Seat>> SearchSeatsByMovies(int MovieId);
        Task<List<Movie>> GetAllMovies();

        Task<List<Booking>> GetAllBookings();

        Task<SalesReport> GenerateSalesReport(int lastNDays);

        Task RemoveBooking(int BookingId);

        Task<Booking> GetBookingById(int BookingId);

        Task RemoveUser(string UserId);
        Task<User> GetUserById(string UserId);

         Task SendVerificationEmail(string Gmail, string userId);

        Task SendResetPasswordEmail(string Gmail);

        Task UpdateChatStatus(string ChatId, Chat.ChatStatus Status);

        Task<List<Chat>> GetAllChats();

        Task JoinChat(string ChatId);

        Task<List<SupportTicket>> GetSupportTickets();

        Task<SupportTicket> GetSupportTicketById(string SupportTicketId);


        // used on a "SupportTicketsDetails" page to help user resolve the issue
        Task AddCommentChangeStatus(string Comment,string TicketId);
        Task<List<Message>> GetSupportTicketMessages(string TicketId);


        
        Task ResolveTicket(string TicketId);


    }
}
