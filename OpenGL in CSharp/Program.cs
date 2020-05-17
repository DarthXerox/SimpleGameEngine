using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
//using System.Console;
using GameNamespace;
using OpenGL_in_CSharp.Utils;

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
            
            /*
            var x = MtlParser.ParseMtl("../../custom.mtl");
            Console.WriteLine(x.Count);
            Console.WriteLine(x.First().Name);

            var y = MtlParser.ParseMtl("../../test.mtl");
            Console.WriteLine(y.Count);
            Console.WriteLine(y.First().Name);
            Console.WriteLine(y[1].Name);
            Console.ReadKey();*/
        }
    }
}
