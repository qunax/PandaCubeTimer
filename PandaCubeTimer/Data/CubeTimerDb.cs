using PandaCubeTimer.Models;
using SQLite;

namespace PandaCubeTimer.Data
{
    public class CubeTimerDb
    {
        private readonly SQLiteAsyncConnection _connection;
        public SQLiteAsyncConnection Connection => _connection;

        public CubeTimerDb()
        {
            _connection = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        }

        public async Task InitializeAsync()
        {
            // for migration while developing 🤷 
            // await _connection.DropTableAsync<PuzzleSolve>();
            // await _connection.DropTableAsync<Session>();
            // await _connection.DropTableAsync<Discipline>();

            await _connection.CreateTableAsync<Session>();
            await _connection.CreateTableAsync<PuzzleSolve>();
            await _connection.CreateTableAsync<Discipline>();
        }
    }
}
