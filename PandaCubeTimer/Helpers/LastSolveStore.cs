using PandaCubeTimer.Models;

namespace PandaCubeTimer.Helpers;

// using this because there were problems with implementation of navigating back to TimeView how i wanted
// so problems with passing parameters
public static class LastSolveStore
{
    public static PuzzleSolve? LastPuzzleSolve { get; set; } = null;
}