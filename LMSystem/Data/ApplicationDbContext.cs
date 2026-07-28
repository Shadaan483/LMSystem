using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LMSystem.Data
{
    // Dedicated DbContext for ASP.NET Core Identity (AspNetUsers, AspNetRoles, etc.).
    // Kept separate from LibraryContext on purpose: LibraryContext owns the
    // library domain tables (Books13, BorrowRecords13, Publications), while this
    // context owns authentication/authorization only. Both point at the same
    // physical "LMS" database via the same DefaultConnection connection string,
    // so no new database is required - just a second set of tables in it.
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
    }
}
