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


        public async Task<List<PuzzleSolve>> GetSessionPuzzleSolvesAsync(Guid sessionId)
        {
            return await _connection.Table<PuzzleSolve>()
                                    .Where(ps => ps.SessionId == sessionId)
                                    .OrderByDescending(s => s.DateTime)
                                    .ToListAsync();
        }

        public async Task<PuzzleSolve> GetPuzzleSolveAsync(Guid solveId)
        {
            return await _connection.Table<PuzzleSolve>().Where(x => x.Id == solveId).FirstAsync();
        }
        
        public async Task<int> CreatePuzzleSolveAsync(PuzzleSolve solveToCreate)
        {
            return await _connection.InsertAsync(solveToCreate);
        }

        public async Task DeletePuzzleSolveAsync(PuzzleSolve solveToDelete)
        {
            await _connection.DeleteAsync(solveToDelete);
        }
    }
}
