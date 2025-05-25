using SQLite;

namespace PandaCubeTimer.Models;

public class Session
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string SessionName { get; set; } = null!;

    [Ignore]
    public ICollection<PuzzleSolve> Solves { get; set; } = null!;
}