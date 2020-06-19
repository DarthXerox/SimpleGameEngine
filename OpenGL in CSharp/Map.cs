using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK;
using OpenGL_in_CSharp.Utils;
using System.Linq;
using System.ComponentModel.Design;
using System.IO;

namespace OpenGL_in_CSharp
{
    public class Map
    {
        public List<Terrain> Terrains { set; get; } = new List<Terrain>();

        public SceneObject Tree { set; get; }
        public SceneObject TreeLeaves { set; get; }

        public int Width { get; }
        public int Height { get; }
        public Bitmap HeightMap { private set; get; }

        public Map(int width, int height, string texture, string heightMapFile)
        {
            Width = width;
            Height = height;
            HeightMap = new Bitmap(heightMapFile);

            for (int z = 0; z < Height; z++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var ter = new Terrain(texture, HeightMap)
                    {
                        Position = new Vector3(x * (HeightMap.Width - 1), 0, z * (HeightMap.Height - 1))
                    };
                    Terrains.Add(ter);
                }
            }

            Tree = new SceneObject(FilePaths.ObjTreeTrunk, FilePaths.TextureTreeTrunk);
            Tree.Position = new Vector3(20, GetHeight(20, 20), 20);

            TreeLeaves = new SceneObject(FilePaths.ObjTreeLeaves, FilePaths.TextureTreeLeaves);
            TreeLeaves.Position = Tree.Position;
        }

        public float GetHeight(float x, float z)
        {
            x %= HeightMap.Width;
            z %= HeightMap.Height;

            try
            {
                return Terrains.First().GetInterpolatedHeight(x, z);
            } catch (InvalidOperationException)
            {
                return 0f;
            }
        }

        public void DrawMap(ShaderProgram program)
        {
            foreach (var t in Terrains)
            {
                program.AttachModelMatrix(t.GetModelMatrix());
                t.Draw();
                program.AttachModelMatrix(Tree.GetModelMatrix() * t.GetModelMatrix());
                Tree.Draw();
                program.AttachModelMatrix(TreeLeaves.GetModelMatrix() * t.GetModelMatrix());
                TreeLeaves.Draw();
            }
        }
    }
}
