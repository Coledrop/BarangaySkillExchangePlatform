namespace BarangaySkillExchangePlaform.Server.Models
{
    public class SkillOffer
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Availability { get; set; } = string.Empty;
        public string LocationPreference { get; set; } = string.Empty;

        public string Status { get; set; } = "Active";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
