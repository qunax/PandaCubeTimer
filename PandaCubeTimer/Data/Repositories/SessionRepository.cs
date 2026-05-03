using Microsoft.Extensions.Logging;
using PandaCubeTimer.Models;
using PandaCubeTimer.Models.DTOs;
using PandaCubeTimer.Services;
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
            int count = await _connection.Table<Session>().CountAsync(s => s.IsDeleted == false);
            if (count != 0)
                return;
            
            try
            {
                Session defaultSession = await _connection.GetAsync<Session>(Session.DefaultSessionId);
                string sql = "UPDATE Session SET IsDeleted = 0 WHERE Id = @Id";
                await _connection.ExecuteAsync(sql, defaultSession.Id);
                
                _logger.LogInformation("Default session restored from deleted.");
            }
            catch (Exception ex)
            {
                Session newDefaultSession = new Session();
                newDefaultSession.Id = Session.DefaultSessionId;
                newDefaultSession.Name = "Default";
                newDefaultSession.DisciplineId = WcaDisciplines.Cube3x3;
                newDefaultSession.IsDeleted = false;
                await _connection.InsertAsync(newDefaultSession);
                
                _logger.LogInformation("Default session added.");
            }
        }

        public async Task<List<PandaCubeTimer.Models.DTOs.SessionDTO>> GetAllSessionsDTOsAsync()
        {
            string sql = @"
        SELECT 
            s.Id, 
            s.Name, 
            s.DisciplineId, 
            d.Name AS DisciplineName
        FROM Session s
        INNER JOIN Discipline d ON s.DisciplineId = d.Id
        WHERE s.IsDeleted = 0";

            return await _connection.QueryAsync<SessionDTO>(sql);
        }

        public async Task<SessionDTO?> GetSessionDTOByIdAsync(Guid id)
        {
            string sql = @"
        SELECT 
            s.Id, 
            s.Name, 
            s.DisciplineId, 
            d.Name AS DisciplineName
        FROM Session s
        INNER JOIN Discipline d ON s.DisciplineId = d.Id
        WHERE s.Id = ? AND s.IsDeleted = 0"; 
    
            var sessionsListResult = await _connection.QueryAsync<SessionDTO>(sql, id);
            return sessionsListResult.FirstOrDefault();
        }

        public async Task<Session> GetSessionByIdAsync(Guid id)
        {
            return await _connection.GetAsync<Session>(id);
        }

        public async Task InsertAsync(Session session)
        {
            await _connection.InsertAsync(session);
        }

        public async Task DeleteAsync(Guid id)
        {
            string sql = "UPDATE Session SET IsDeleted = 1 WHERE Id = @Id";
            await _connection.ExecuteAsync(sql, id);
        }

        public async Task<List<SessionSyncDTO>> GetSessionsForSync()
        {
            return await _connection.QueryAsync<SessionSyncDTO>("SELECT * FROM Session");
        }
    }
}
