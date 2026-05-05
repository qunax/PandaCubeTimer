using SQLite;
using TNoodle.Puzzles;

namespace PandaCubeTimer.Models;

public class PuzzleSolve
{
    [PrimaryKey] public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Puzzle to which this solve is belong
    /// </summary>
    //public string DisciplineId { get; set; } = null!;
    
    /// <summary>
    /// Session in which this solve was made
    /// </summary>
    public Guid SessionId { get; set; }

    /// <summary>
    /// Navigation property
    /// </summary>
    [Ignore]
    public Session Session { get; set; } = null!;
    
    /// <summary>
    /// Time in which puzzle was solved
    /// </summary>
    public double SolveTimeSeconds { get; set; }
    
    /// <summary>
    /// Penalty
    /// </summary>
    public bool IsPlusTwo { get; set; }
    
    /// <summary>
    /// Penalty
    /// </summary>
    public bool IsDNF { get; set; }
    
    /// <summary>
    /// How the cube was scrambled before solve
    /// </summary>
    public string Scramble { get; set; } = null!;
    
    /// <summary>
    /// When the puzzleSolve was made
    /// </summary>
    public DateTime CreatedAt { get; set; } =  DateTime.UtcNow;
    
    /// <summary>
    /// for detection of soft deletion
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Text annotations by user
    /// </summary>
    public string? Comment { get; set; }
    
    /// <summary>
    /// soft deletion
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// mark synchronization with API
    /// </summary>
    public bool IsSynced { get; set; } = false;

    // public Image ScrambledCube { get; set; }
}