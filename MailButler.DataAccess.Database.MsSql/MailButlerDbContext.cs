using MailButler.DataAccess.Database.MsSql.Entities;
using Microsoft.EntityFrameworkCore;

namespace MailButler.DataAccess.Database.MsSql;

public class MailButlerDbContext : DbContext
{
	public DbSet<Account> Accounts { get; init; } = default!;
	public DbSet<Filter> Filters { get; init; } = default!;
	public DbSet<Rule> Rules { get; init; } = default!;
}