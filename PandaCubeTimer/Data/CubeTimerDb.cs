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
            
            // for migration while developing 🤷 
            // _connection.DropTableAsync<PuzzleSolve>();
            // _connection.DropTableAsync<Session>();
            // _connection.DropTableAsync<Discipline>();

            _connection.CreateTableAsync<Session>();
            _connection.CreateTableAsync<PuzzleSolve>();
            _connection.CreateTableAsync<Discipline>();
        }
    }
}
