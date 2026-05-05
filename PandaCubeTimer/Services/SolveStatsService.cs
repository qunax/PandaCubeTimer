using Microsoft.Extensions.Logging;
using PandaCubeTimer.Data;
using PandaCubeTimer.Data.Repositories;
using PandaCubeTimer.Helpers;
using PandaCubeTimer.Models;

namespace PandaCubeTimer.Services;

public class SolveStatsService : ISolveStatsService
{
    private readonly PuzzleSolveRepository _puzzleSolveRepository;
    private readonly ILogger _logger;
    
    
    
    public SolveStatsService(PuzzleSolveRepository solveRepository, ILogger<ISolveStatsService> logger)
    {
        _puzzleSolveRepository = solveRepository;
        _logger = logger;
    }


    
    public string CalculateCurrentAverageOf(List<PuzzleSolve> solves, int count)
    {
        if (solves.Count < count) return "-";
        var window = solves.Skip(solves.Count - count).Take(count).ToList();
        return CalculateStandardAverage(window);
    }
    
    public string CalculateBestAverageOf(List<PuzzleSolve> solves, int count)
    {
        if (solves.Count < count) return "-";

        double bestAverage = double.MaxValue;
        bool foundValidAverage = false;

        for (int i = 0; i <= solves.Count - count; i++)
        {
            var window = solves.Skip(i).Take(count).ToList();
            
            if (window.Count(s => s.IsDNF) > 1) 
                continue;

            var times = window.Select(s => s.IsDNF ? double.MaxValue : s.SolveTimeSeconds).OrderBy(t => t).ToList();
            var trimmedTimes = times.Skip(1).Take(times.Count - 2).ToList();
            double currentAverage = trimmedTimes.Average();
            
            if (currentAverage < bestAverage)
            {
                bestAverage = currentAverage;
                foundValidAverage = true;
            }
        }

        return foundValidAverage ? bestAverage.FormatTime() : "-";
    }
    
    

    private string CalculateStandardAverage(List<PuzzleSolve> window)
    {
        if (window.Count(s => s.IsDNF) > 1) return "DNF";

        var times = window.Select(s => s.IsDNF ? double.MaxValue : s.SolveTimeSeconds).OrderBy(t => t).ToList();
        var trimmedTimes = times.Skip(1).Take(times.Count - 2);
        
        return trimmedTimes.Average().FormatTime();
    }
}