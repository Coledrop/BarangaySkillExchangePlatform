namespace BarangaySkillExchangePlaform.Server.Models
{
    using Microsoft.AspNetCore.Identity;
    public class User : IdentityUser<int>
    {
        public string FullName { get; set; }  = string.Empty;
        public string ContactNumber {get; set;} = string.Empty;
        public string Address {get; set;} = string.Empty;

        public string Role { get; set; } = "Resident";
        public string Status { get; set; } = "Active";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<SkillOffer> SkillOffers {get; set;} = new List<SkillOffer>();
        public ICollection<ServiceRequest> ServiceRequests {get; set;} = new List<ServiceRequest>();

    }
}
