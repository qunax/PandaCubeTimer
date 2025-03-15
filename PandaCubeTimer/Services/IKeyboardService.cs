using SharpHook;

namespace PandaCubeTimer.Services
{
    public interface IKeyboardService
    {
        IGlobalHook Hook { get; }
    }
}
