namespace EventManagement.Areas.System.View_Models
{
    public class UserTicketsVM
    {
        public string EventName { get; set; }
        public DateTime TicketExpireDate { get; set; }
        public DateTime EventStartDate { get; set; }
        public string TicketPrice { get; set; }
        public int NumberOfTickets { get; set; }
    }
}
