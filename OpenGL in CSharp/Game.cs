using System;

namespace GameNamespace
{
    /// <summary>
    /// This class might seem a little bit unnecessary, but I am planning to extend the project
    /// to be able to load all of the .obj and .mtl files in different threads and send this info to the window
    /// </summary>
    public class Game : IDisposable
    {
        private MainWindow Window { set;  get; }

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
            Window.Close();
        }
    }
}
