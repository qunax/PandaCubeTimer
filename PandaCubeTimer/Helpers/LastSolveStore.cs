using PandaCubeTimer.Models;

namespace PandaCubeTimer.Helpers;

public interface ILastSolveStore
{
    public PuzzleSolve? LastPuzzleSolve { get; set; }
    public SolvePenalty? InspectionPenalty { get; set; }
    public string? SolveScramble { get; set; }

    public void ClearData();

}

// using this because there were problems with implementation of navigating back to TimeView how i wanted
// so problems with passing parameters
public class LastSolveStore :  ILastSolveStore
{
    // 1) saving scramble from TimeVM
    // (will need it when saving to db in CountingTimerVM)
    // 2) saving inspection penalty from InspectionVM
    // (will need it in CountingVM when saving to DB
    // and then in TimerVM when showing penalties to apply after solve is finished)
    // 3) saving LastPuzzleSolve from CountingVM 
    // (will need it to display and operate later in TimerVM)
    // 4) Clear all information before starting timer in TimerVM
    
    public PuzzleSolve? LastPuzzleSolve { get; set; }
    public SolvePenalty? InspectionPenalty { get; set; }
    public string? SolveScramble { get; set; }

    
    
    public LastSolveStore()
    {
    }
    
    

    public void ClearData()
    {
        LastPuzzleSolve = null;
        InspectionPenalty = null;
        SolveScramble = null;
    }
}