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
            int size = System.Runtime.InteropServices.Marshal.SizeOf<Matrix4>();
            Console.WriteLine(size);
            Window.Run(1.0 / 60.0, 1.0 / 60.0);
            /*
            int count = 100;
            float sum = 0f;
            for (int i = 0; i < count; i++)
            {
                var watches = System.Diagnostics.Stopwatch.StartNew();
                //var tex = Texture2D.CreateTextureAsync(FilePaths.TextureBrickWall);
                //var mesh = new NormalMappingMesh(FilePaths.ObjTreeTrunk, FilePaths.BumpTexTrunk, FilePaths.MtlBronze, FilePaths.BumpTexTrunk);
                InitializeObjects3DParallel();
                //InitializeObjects3D();
                sum += watches.ElapsedMilliseconds;
            }
            Console.WriteLine($"Tree mesh with both textures took time: {sum / count}ms");
            */

            /*
            int count = 100;
            float sum = 0f;
            for (int i = 0; i < count; i++)
            {
                var watches = System.Diagnostics.Stopwatch.StartNew();
                var tex = new Texture2D(FilePaths.TextureBrickWall);
                tex.Dispose();
                sum += watches.ElapsedMilliseconds;
            }
            Console.WriteLine($"Classic textures time: {sum / count}ms");

            sum = 0f;
            for (int i = 0; i < count; i++)
            {
                var watches = System.Diagnostics.Stopwatch.StartNew();
                var tex = Texture2D.CreateTextureAsync(FilePaths.TextureBrickWall);
                tex.Result.Dispose();

                sum += watches.ElapsedMilliseconds;
            }
            Console.WriteLine($"Async textures time: {sum / count}ms");
            */

        }

        public void Dispose()
        {
            Window?.Close();
            Window?.Dispose();
        }
    }
}
