using Microsoft.EntityFrameworkCore;
using GenerateData.Models;

namespace GenerateData.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<HobiModel> tblM_Hobi { get; set; }
        public DbSet<GenderModel> tblM_Gender { get; set; }
        public DbSet<PersonalDataModel> tblT_Personal { get; set; }
        public DbSet<UmurDataModel> tblT_Umur { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PersonalDataModel>().ToTable("tblT_Personal");
            modelBuilder.Entity<UmurDataModel>()
                .ToTable("tblT_Umur")
                .HasNoKey(); // Define as a keyless entity
        }
    }

}