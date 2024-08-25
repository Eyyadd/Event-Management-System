namespace EventManagement.Areas.System.View_Models
{
    public class PaymentCheckoutViewModel
    {
        public string EventName { get; set; }
        public decimal TicketPrice { get; set; }
        public int? EventId { get; set; } // Ensure you also have this property for the event ID

    }
}
