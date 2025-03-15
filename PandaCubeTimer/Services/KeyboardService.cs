using SharpHook;

namespace PandaCubeTimer.Services
{
    class KeyboardService : IKeyboardService
    {
        private readonly IGlobalHook _hook;
        public IGlobalHook Hook { get => _hook; }

        public KeyboardService()
        {
            _hook = new TaskPoolGlobalHook();
            InitKeyboardHooks();
        }

        private async void InitKeyboardHooks()
        {
            await _hook.RunAsync();
        }
    }
}
