namespace BarangaySkillExchangePlaform.Server.Data
{
    using BarangaySkillExchangePlaform.Server.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    public class AppDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<SkillOffer> SkillOffers => Set<SkillOffer>();
        public DbSet<ServiceRequest> ServiceRequests => Set<ServiceRequest>();
        public DbSet<Exchange> Exchanges => Set<Exchange>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.Property(u => u.ContactNumber).HasColumnName("ContactNumber");
                entity.HasIndex(u => u.Email).IsUnique();
            });

            modelBuilder.Entity<SkillOffer>()
                .HasOne(skillOffer => skillOffer.User)
                .WithMany(user => user.SkillOffers)
                .HasForeignKey(skillOffer => skillOffer.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ServiceRequest>()
                .HasOne(serviceRequest => serviceRequest.User)
                .WithMany(user => user.ServiceRequests)
                .HasForeignKey(serviceRequest => serviceRequest.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Exchange>()
                .HasOne(exchange => exchange.Requester)
                .WithMany()
                .HasForeignKey(exchange => exchange.RequesterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Exchange>()
                .HasOne(exchange => exchange.Provider)
                .WithMany()
                .HasForeignKey(exchange => exchange.ProviderId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
