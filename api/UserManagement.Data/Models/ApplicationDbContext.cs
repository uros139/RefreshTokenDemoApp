using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserManagement.Data.Models.Entities;

namespace UserManagement.Data.Models;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Student> Students { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
}
