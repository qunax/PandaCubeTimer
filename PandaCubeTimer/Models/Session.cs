using SQLite;

namespace PandaCubeTimer.Models;

public class Session
{
    public static readonly Guid DefaultSessionId = new Guid("00000000-0000-0000-0000-000000000001");
    
    [PrimaryKey] public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = null!;
    
    public string DisciplineId { get; set; } = null!;
}