using Microsoft.VisualBasic;
using PandaCubeTimer.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PandaCubeTimer.Data
{
    public class CubeTimerDb
    {
        private readonly SQLiteAsyncConnection _connection;
        public SQLiteAsyncConnection Connection => _connection;

        public CubeTimerDb()
        {
            _connection = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);

            _connection.CreateTableAsync<Session>();
            _connection.CreateTableAsync<PuzzleSolve>();
            _connection.CreateTableAsync<Discipline>();
        }
    }
}
