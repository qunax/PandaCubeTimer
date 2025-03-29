using System.ComponentModel.DataAnnotations;
using TNoodle.Puzzles;

namespace PandaCubeTimer.Models;

public class PuzzleSolve
{   
    [Key]
    public int SolveId { get; set; }
    
    /// <summary>
    /// Puzzle to which this solve is belong
    /// </summary>
    public Discipline Discipline { get; set; } = null!;
    
    /// <summary>
    /// Session in which this solve was made
    /// </summary>
    public int SessionId { get; set; }
    public Session Session { get; set; } = null!;
    
    /// <summary>
    /// Time in which puzzle was solved
    /// </summary>
    public float Time { get; set; }
    
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
    public DateTime DateTime { get; set; }
    
    /// <summary>
    /// Text annotations by user
    /// </summary>
    public string? Comment { get; set; }
    // public Image ScrambledCube { get; set; }
}