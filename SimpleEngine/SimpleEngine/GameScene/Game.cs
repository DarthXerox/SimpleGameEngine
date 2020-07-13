using System;
using OpenTK;

namespace SimpleEngine.GameScene
{
    /// <summary>
    /// This class might seem a little bit unnecessary, but I am planning to extend the project
    /// to be able to load all of the .obj and .mtl files in different threads and send this info to the window
    /// </summary>
    public class Game : IDisposable
    {
        private MainWindow Window { set; get; }

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

        /// <summary>
        /// Loads all necessary textures, 3D models and materials asychronously 
        /// AND initi
        /// </summary>
        /*
        public void InitializeObjects3DParallel()
        {
            var objects3D = new Dictionary<Object3DType, Tuple<ObjModel, Material, Bitmap, Bitmap>>();

            var loadTreeTrunkTextureTask = Task.Run(() => new Bitmap(FilePaths.TextureTreeTrunk));
            var loadTreeTrunkNormalTextureTask = Task.Run(() => new Bitmap(FilePaths.BumpTexTrunk));
            var loadTreeLeavesTextureTask = Task.Run(() => new Bitmap(FilePaths.TextureTreeLeaves3));
            var loadTreeLeavesNormalTextureTask = Task.Run(() => new Bitmap(FilePaths.BumpTexTreeLeaves));
            var loadRockTextureTask = Task.Run(() => new Bitmap(FilePaths.TextureMossyRock));
            var loadRockNormalTextureTask = Task.Run(() => new Bitmap(FilePaths.BumpTexMossyRock));
            var loadWallTextureTask = Task.Run(() => new Bitmap(FilePaths.TextureBrickWall));
            var loadWallNormalTextureTask = Task.Run(() => new Bitmap(FilePaths.BumpTexBrickWall));
            var loadGrassTextureTask = Task.Run(() => new Bitmap(FilePaths.TextureGrass4));
            var loadGrassNormalTextureTask = Task.Run(() => new Bitmap(FilePaths.BumpTexGrass4));
            var loadHeightMapTextureTask = Task.Run(() => new Bitmap(FilePaths.HeightMapPath));

            var loadTreeTrunkTask = ObjModel.LoadWithTangentsAsync(FilePaths.ObjTreeTrunk);
            var loadTreeLeavesTask = ObjModel.LoadWithTangentsAsync(FilePaths.ObjTreeLeaves);
            var loadRockTask = ObjModel.LoadWithTangentsAsync(FilePaths.ObjMossyRock1);

            var loadTreeTrunkMtl = MtlParser.ParseMtlAsync(FilePaths.MtlBronze); //used for wall too
            var loadGrassMtl = MtlParser.ParseMtlAsync(FilePaths.MtlEmerald); //used for tree leaves too
            var loadRockMtl = MtlParser.ParseMtlAsync(FilePaths.MtlChrome);

            objects3D = new Dictionary<Object3DType, Tuple<ObjModel, Material, Bitmap, Bitmap>>()
            {
                { Object3DType.TreeTrunk,
                    new Tuple<ObjModel, Material, Bitmap, Bitmap>(loadTreeTrunkTask.Result, loadTreeTrunkMtl.Result[0],
                    loadTreeTrunkTextureTask.Result, loadTreeTrunkNormalTextureTask.Result) },
                { Object3DType.TreeLeaves,
                    new Tuple<ObjModel, Material, Bitmap, Bitmap>(loadTreeLeavesTask.Result, loadGrassMtl.Result[0],
                    loadTreeLeavesTextureTask.Result, loadTreeLeavesNormalTextureTask.Result) },
                { Object3DType.Rock,
                    new Tuple<ObjModel, Material, Bitmap, Bitmap>(loadRockTask.Result, loadRockMtl.Result[0],
                    loadRockTextureTask.Result, loadRockNormalTextureTask.Result) },
                { Object3DType.Ground,
                    new Tuple<ObjModel, Material, Bitmap, Bitmap>(loadTreeTrunkTask.Result, loadGrassMtl.Result[0],
                    loadRockTextureTask.Result, loadRockNormalTextureTask.Result) },
                { Object3DType.Wall,
                    new Tuple<ObjModel, Material, Bitmap, Bitmap>(loadTreeTrunkTask.Result, loadTreeTrunkMtl.Result[0],
                    loadWallTextureTask.Result, loadWallNormalTextureTask.Result) }
            };
        }

        public void InitializeObjects3D()
        {
            var loadTreeTrunkTextureTask =  new Bitmap(FilePaths.TextureTreeTrunk);
            var loadTreeTrunkNormalTextureTask =  new Bitmap(FilePaths.BumpTexTrunk);
            var loadTreeLeavesTextureTask =  new Bitmap(FilePaths.TextureTreeLeaves3);
            var loadTreeLeavesNormalTextureTask =  new Bitmap(FilePaths.BumpTexTreeLeaves);
            var loadRockTextureTask =  new Bitmap(FilePaths.TextureMossyRock);
            var loadRockNormalTextureTask =  new Bitmap(FilePaths.BumpTexMossyRock);
            var loadWallTextureTask =  new Bitmap(FilePaths.TextureBrickWall);
            var loadWallNormalTextureTask =  new Bitmap(FilePaths.BumpTexBrickWall);
            var loadGrassTextureTask =  new Bitmap(FilePaths.TextureGrass4);
            var loadGrassNormalTextureTask =  new Bitmap(FilePaths.BumpTexGrass4);
            var loadHeightMapTextureTask =  new Bitmap(FilePaths.HeightMapPath);

            var loadTreeTrunkTask = ObjModel.LoadWithTangentsAsync(FilePaths.ObjTreeTrunk).Result;
            var loadTreeLeavesTask = ObjModel.LoadWithTangentsAsync(FilePaths.ObjTreeLeaves).Result;
            var loadRockTask = ObjModel.LoadWithTangentsAsync(FilePaths.ObjMossyRock1).Result;

            var loadTreeTrunkMtl = MtlParser.ParseMtlAsync(FilePaths.MtlBronze).Result; //used for wall too
            var loadGrassMtl = MtlParser.ParseMtlAsync(FilePaths.MtlEmerald).Result; //used for tree leaves too
            var loadRockMtl = MtlParser.ParseMtlAsync(FilePaths.MtlChrome).Result;
        }

        */
    }
}
