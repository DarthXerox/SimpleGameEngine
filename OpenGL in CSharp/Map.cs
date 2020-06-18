using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK;
using OpenGL_in_CSharp.Utils;

namespace OpenGL_in_CSharp
{
    public class Map
    {
        public List<Terrain> Terrains { set; get; } = new List<Terrain>();
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
                        Translation = new Vector3(x * (HeightMap.Width - 1), 0, z * (HeightMap.Height - 1))
                    };
                    Terrains.Add(ter);
                }
            }
        }

        public void DrawMap(ShaderProgram program)
        {
            foreach (var t in Terrains)
            {
                program.AttachModelMatrix(t.GetModelMatrix());
                t.Draw();
            }
        }
    }
}
