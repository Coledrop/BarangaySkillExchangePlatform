namespace BarangaySkillExchangePlaform.Server.DTOs
{
    public class CreateServiceRequestRequest
    {
        public int UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string ExchangeOffer { get; set; } = string.Empty;
        public string PreferredSchedule { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
    }
}
