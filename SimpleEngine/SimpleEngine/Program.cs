using System;
using SimpleEngine.GameScene;

namespace SimpleEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            Game g = new Game();
            g.Run();
            g.Dispose();
            Console.ReadKey();
        }
    }
}
