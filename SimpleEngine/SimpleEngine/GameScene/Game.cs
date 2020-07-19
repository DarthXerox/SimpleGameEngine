using System;
using OpenTK;

namespace SimpleEngine.GameScene
{
    public class Game : IDisposable
    {
        private MainWindow Window { get; }

        public Game()
        {
            Window = new MainWindow();
        }

        public void Run()
        {
            Window.Run(1.0 / 60.0, 1.0 / 60.0);
        }

        public void Dispose()
        {
            Window?.Close();
            Window?.Dispose();
        }
    }
}
