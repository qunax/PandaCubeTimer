using Microsoft.Extensions.Logging;
using PandaCubeTimer.Models;
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
        
        
        
        public async Task SeedDefaultSession()
        {
            int count = await _connection.Table<Session>().CountAsync();
            if (count != 0)
                return;

            
            Session defaultSession = new Session();
            defaultSession.SessionName = "Default";
            await _connection.InsertAsync(defaultSession);
            
            _logger.LogInformation("Default session added.");
        }
    }
}
