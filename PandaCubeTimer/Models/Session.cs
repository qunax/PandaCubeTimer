using SQLite;

namespace PandaCubeTimer.Models;

public class Session
{
    [PrimaryKey] public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = null!;
    
    public string DisciplineId { get; set; } = null!;
}