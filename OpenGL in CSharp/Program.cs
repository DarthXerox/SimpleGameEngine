using System;
using System.Threading;
using GameNamespace;

namespace OpenGL_in_CSharp
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
