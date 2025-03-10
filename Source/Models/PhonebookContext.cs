using System.Configuration;
using Microsoft.EntityFrameworkCore;

public class PhonebookContext : DbContext
{
    public DbSet<Contact> Contacts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string? connectionString = ConfigurationManager.AppSettings.Get("connectionString");
        optionsBuilder.UseSqlServer(connectionString);
    }
}