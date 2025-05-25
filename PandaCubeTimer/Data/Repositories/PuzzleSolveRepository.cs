using PandaCubeTimer.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PandaCubeTimer.Data.Repositories
{
    public class PuzzleSolveRepository 
    {
        private readonly SQLiteAsyncConnection _connection;



        public PuzzleSolveRepository(CubeTimerDb cubeTimerDb)
        {
            _connection = cubeTimerDb.Connection;
        }


        /// <summary>
        /// Gets all puzzle solves from local db
        /// </summary>
        /// <returns></returns>
        public async Task<IList<PuzzleSolve>> GetAllPuzzleSolvesAsync()
        {
            return await _connection.Table<PuzzleSolve>().ToListAsync();
        }
        
        /// <summary>
        /// Returns number of rows added to the table
        /// </summary>
        /// <param name="solveToCreate"></param>
        /// <returns></returns>
        public async Task<int> CreatePuzzleSolveAsync(PuzzleSolve solveToCreate)
        {
            return await _connection.InsertAsync(solveToCreate);
        }
    }
}
