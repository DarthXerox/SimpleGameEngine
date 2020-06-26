using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenGL_in_CSharp.Utils;

namespace GameNamespace
{
    public class Game : IDisposable
    {
        private MainWindow Window { set;  get; }
        private Thread GraphicsThread { set;  get; }

        public bool IsEnd { set; get; } = false;


        public Game() 
        {
            //GraphicsThread = new Thread(() => w = new MainWindow()  w.Run(1.0 / 60.0, 1.0 / 60.0)); { IsBackground = true };
            GraphicsThread = new Thread(() =>
            {
                Window = new MainWindow();
                while (!Window.IsExiting)
                {
                    Window.Run(1.0 / 60.0, 1.0 / 60.0);
                }
                IsEnd = true;
            })
            { IsBackground = true };
            GraphicsThread.Start();
        }

        public void Dispose()
        {
            GraphicsThread.Abort(); // NOT GOOD
            Window.Close();
        }
    }
}
