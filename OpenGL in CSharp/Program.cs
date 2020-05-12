using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
//using System.Console;
using GameNamespace;

namespace OpenGL_in_CSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            //new MainWindow().Run(60);
            Game g = new Game();
            //Console.ReadKey();
            //Thread.Sleep(5000);
            while (!g.IsEnd) { }
            Console.WriteLine("Disposing");
            Thread.Sleep(1000);

            g.Dispose();
            Console.Write("Done, closing...");
            Thread.Sleep(1000);


        }
    }
}
