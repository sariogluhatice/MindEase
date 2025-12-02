using Microsoft.EntityFrameworkCore;
using MindEase.Models;

namespace MindEase.Data
{
    public class MindEaseContext : DbContext
    {
        public MindEaseContext(DbContextOptions<MindEaseContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}
