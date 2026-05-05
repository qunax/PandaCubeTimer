using PandaCubeTimer.Models;
using SQLite;

namespace PandaCubeTimer.Data.Repositories
{
    public class PuzzleSolveRepository 
    {
        private readonly SQLiteAsyncConnection _connection;



        public PuzzleSolveRepository(CubeTimerDb cubeTimerDb)
        {
            _connection = cubeTimerDb.Connection;
        }


        
        public async Task<PuzzleSolve?> GetPuzzleSolveAsync(Guid solveId)
        {
            return await _connection.FindAsync<PuzzleSolve>(solveId);
        }
        
        public async Task<List<PuzzleSolve>> GetSessionPuzzleSolvesAsync(Guid sessionId)
        {
            return await _connection.Table<PuzzleSolve>()
                                    .Where(ps => ps.SessionId == sessionId)
                                    .OrderByDescending(s => s.CreatedAt)
                                    .ToListAsync();
        }
        
        public async Task<List<PuzzleSolve>> GetUnsyncedSolvesAsync()
        {
            return await _connection.Table<PuzzleSolve>()
                .Where(s => s.IsSynced == false)
                .ToListAsync();
        }
        
        public async Task<int> InsertAsync(PuzzleSolve solveToCreate)
        {
            return await _connection.InsertAsync(solveToCreate);
        }
        
        /// <summary>
        /// user has to control UpdatedAt and IsSynced properties by himself
        /// </summary>
        /// <param name="puzzleSolve"></param>
        public async Task UpdateAsync(PuzzleSolve puzzleSolve)
        {
            await _connection.UpdateAsync(puzzleSolve);
        }
        
        public async Task MarkAsSyncedAsync(Guid id)
        {
            string sql = "UPDATE PuzzleSolve SET IsSynced = 1, UpdatedAt = ? WHERE Id = ?";
            await _connection.ExecuteAsync(sql, DateTime.UtcNow, id);
        }

        public async Task DeletePuzzleSolveAsync(Guid solveId)
        {
            string sql = "UPDATE PuzzleSolve SET IsDeleted = 1, IsSynced = 0, UpdatedAt = ? WHERE Id = ?";
            await _connection.ExecuteAsync(sql, DateTime.UtcNow, solveId);
        }
    }
}
