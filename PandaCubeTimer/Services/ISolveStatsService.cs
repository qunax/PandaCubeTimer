using PandaCubeTimer.Models;

namespace PandaCubeTimer.Services;

public interface ISolveStatsService
{
    public string CalculateCurrentAverageOf(List<PuzzleSolve> solves, int count);
    public string CalculateBestAverageOf(List<PuzzleSolve> solves, int count);
}