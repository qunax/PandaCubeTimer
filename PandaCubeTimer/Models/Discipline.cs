using SQLite;

namespace PandaCubeTimer.Models;

public class Discipline
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    /// <summary>
    /// discipline name (should somehow correlate with TNoodle library ~ mapping?) 
    /// </summary>
    public string Name { get; set; } = null!;
}