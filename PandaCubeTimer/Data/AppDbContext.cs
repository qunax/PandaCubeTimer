using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Maui.Storage;
using PandaCubeTimer.Models;
using TNoodle.Puzzles;

namespace PandaCubeTimer.Data;

public class AppDbContext : DbContext
{
    public DbSet<PuzzleSolve> PuzzleSolves { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<Discipline> Disciplines { get; set; }

    public AppDbContext()
    {
        SQLitePCL.Batteries_V2.Init();
        Database.Migrate();
        //Database.EnsureCreated();
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "pandacubetimer.db");
        optionsBuilder.UseSqlite($"Filename={dbPath}");
        base.OnConfiguring(optionsBuilder);
    }
}