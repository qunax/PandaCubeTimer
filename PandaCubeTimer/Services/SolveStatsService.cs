using Microsoft.Extensions.Logging;
using PandaCubeTimer.Data;
using PandaCubeTimer.Models;

namespace PandaCubeTimer.Services;

public class SolveStatsService : ISolveStatsService
{
    private readonly CubeTimerDb _db;
    private readonly ILogger _logger;
    
    
    public SolveStatsService(CubeTimerDb database, Logger<SolveStatsService> logger)
    {
        _db = database;
        _logger = logger;
    }


    public int GetAllSolvesCount(Discipline discipline, int sessionId)
    {
        return 0;
    }
}