using Microsoft.EntityFrameworkCore;
using MyLoto.Domain.Entities;
using MyLoto.Infrastructure.Persistence.Interceptors;

namespace MyLoto.Infrastructure.Persistence;

public class LotoDbContext : DbContext
{
    private readonly UpdateAuditableInterceptor _auditableInterceptor;

    public LotoDbContext(
        DbContextOptions<LotoDbContext> options,
        UpdateAuditableInterceptor auditableInterceptor) : base(options)
    {
        _auditableInterceptor = auditableInterceptor;
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Lottery> Lotteries => Set<Lottery>();
    public DbSet<Draw> Draws => Set<Draw>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<PrizeTier> PrizeTiers => Set<PrizeTier>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<AdminLog> AdminLogs => Set<AdminLog>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditableInterceptor);
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LotoDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}