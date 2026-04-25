using Microsoft.Extensions.Logging;
using PandaCubeTimer.Models;
using SQLite;

namespace PandaCubeTimer.Data.Repositories
{
    public class DisciplineRepository
    {
        private readonly SQLiteAsyncConnection _connection;
        private readonly ILogger _logger;
        
        
        
        public DisciplineRepository(CubeTimerDb cubeTimerDb, ILogger<DisciplineRepository> logger)
        {
            _connection = cubeTimerDb.Connection;
            _logger = logger;
        }
        
        
        
        public async Task SeedDisciplinesAsync()
        {
            _logger.LogInformation("Seeding disciplines.");
            
            int count = await _connection.Table<Discipline>().CountAsync();
            if (count != 0)
                return; 
            
            var defaultDisciplines = new List<Discipline>
            {
                new() { Id = WcaDisciplines.Cube3x3, Name = "3x3x3 Cube" },
                new() { Id = WcaDisciplines.Cube2x2, Name = "2x2x2 Cube" },
                new() { Id = WcaDisciplines.Cube4x4, Name = "4x4x4 Cube" },
                new() { Id = WcaDisciplines.Pyraminx, Name = "Pyraminx" },
                new() { Id = WcaDisciplines.Megaminx, Name = "Megaminx" },
                new() { Id = WcaDisciplines.Square1, Name = "Square-1" },
                new() { Id = WcaDisciplines.Clock, Name = "Rubik's Clock" },
                new() { Id = WcaDisciplines.OneHanded, Name = "3x3x3 One-Handed" }
            };
            await _connection.InsertAllAsync(defaultDisciplines);
            
            _logger.LogInformation("Seeding disciplines is finished.");
        }
    }
}
