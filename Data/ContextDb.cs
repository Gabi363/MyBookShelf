using Microsoft.EntityFrameworkCore;

public class ContextDb : DbContext
{
    public ContextDb(DbContextOptions<ContextDb> options)
        : base(options) { }

    public DbSet<Book> Book { get; set; } = default!;
    public DbSet<User> User { get; set; } = default!;
    public DbSet<BookStatus> BookStatuses { get; set; } = default!;
    public DbSet<Opinion> Opinion { get; set; } = default!;
}