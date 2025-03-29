using System.ComponentModel.DataAnnotations;

namespace PandaCubeTimer.Models;

public class Discipline
{
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// discipline name (should somehow correlate with TNoodle library ~ mapping?) 
    /// </summary>
    public string Name { get; set; } = null!;
}