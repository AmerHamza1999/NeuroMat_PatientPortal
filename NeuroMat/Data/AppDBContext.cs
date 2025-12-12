namespace NeuroMat.Data
{
    using Microsoft.EntityFrameworkCore;
    public class AppDBContext : DbContext   
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
        }
    }
}
