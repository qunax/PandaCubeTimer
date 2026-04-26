using Microsoft.Extensions.Logging;
using PandaCubeTimer.Models;
using PandaCubeTimer.Models.DTOs;
using SQLite;

namespace PandaCubeTimer.Data.Repositories
{
    public class SessionRepository
    {
        private readonly SQLiteAsyncConnection _connection;
        private readonly ILogger _logger;
        
        
        
        public SessionRepository(CubeTimerDb cubeTimerDb, ILogger<SessionRepository> logger)
        {
            _connection = cubeTimerDb.Connection;
            _logger = logger;
        }
        
        
        
        public async Task SeedDefaultSessionAsync()
        {
            int count = await _connection.Table<Session>().CountAsync();
            if (count != 0)
                return;
            
            Session defaultSession = new Session();
            defaultSession.Name = "Default";
            defaultSession.DisciplineId = WcaDisciplines.Cube3x3;
            await _connection.InsertAsync(defaultSession);
            
            _logger.LogInformation("Default session added.");
        }

        public async Task<List<SessionDTO>> GetAllSessionsAsync()
        {
            string sql = @"
        SELECT 
            s.Id, 
            s.Name, 
            s.DisciplineId, 
            d.Name AS DisciplineName
        FROM Session s
        INNER JOIN Discipline d ON s.DisciplineId = d.Id";

            // SQLite сама сделает джойн под капотом и вернет готовый плоский список
            return await _connection.QueryAsync<SessionDTO>(sql);
            //return await _connection.Table<Session>().ToListAsync();
        }

        public async Task InsertAsync(Session session)
        {
            await _connection.InsertAsync(session);
        }
    }
}
