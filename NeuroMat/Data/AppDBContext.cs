namespace NeuroMat.Data
{
    using Microsoft.EntityFrameworkCore;
    using NeuroMat.Models;

    public class AppDBContext : DbContext   
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
        }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<PressureFrame> PressureFrames { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Alert> Alerts { get; set; }
    }
}
