using RAGENativeUI;
using Rage;

namespace EverydayCallouts.Engine
{
    public static class MenuManager
    {
        public static MenuPool Pool = new MenuPool();

        private static GameFiber _menuFiber;
        private static bool _isProcessing = false;

        public static void StartProcessing()
        {
            if (_isProcessing) return;

            _isProcessing = true;

            _menuFiber = GameFiber.StartNew(() =>
            {
                while (true)
                {
                    GameFiber.Yield();
                    Pool.ProcessMenus();
                }
            }, "[EverydayCallouts] MenuManager Fiber");
        }

        public static void StopProcessing()
        {
            if (_menuFiber != null && _menuFiber.IsAlive)
            {
                _menuFiber.Abort();
            }

            _menuFiber = null;
            _isProcessing = false;
        }
    }
}
