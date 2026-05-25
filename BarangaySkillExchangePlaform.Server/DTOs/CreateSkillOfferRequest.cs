namespace BarangaySkillExchangePlaform.Server.DTOs
{
    public class CreateSkillOfferRequest
    {
        public int UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Availability { get; set; } = string.Empty;
        public string LocationPreference { get; set; } = string.Empty;
    }
}
