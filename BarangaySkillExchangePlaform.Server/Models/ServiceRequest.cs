namespace BarangaySkillExchangePlaform.Server.Models
{
public class ServiceRequest
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string ExchangeOffer { get; set; } = string.Empty;
    public string PreferredSchedule { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;

    public string Status { get; set; } = "Open";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
}
