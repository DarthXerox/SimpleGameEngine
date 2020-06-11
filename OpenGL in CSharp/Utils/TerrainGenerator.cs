using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace OpenGL_in_CSharp.Utils
{
    /// <summary>
    /// Reads data for a heatmap from
    /// http://www.arendpeter.com/Perlin_Noise.html
    /// http://devmag.org.za/2009/04/25/perlin-noise/
    /// </summary>
    public class TerrainGenerator
    {
        public readonly int Seed = new Random().Next(); // maybe static if there are multiple terrains
        public readonly float Amplitude = 30.0f;

        /// <summary>
        /// In points 
        /// </summary>
        public int Width { get; set; }
        public int Height { get; set; }
        public int MaxNoise { set;  get; }
        /*
        GameObject GenerateTerrain(int width, int height)
        {
            Width = width;
            Height = height;
            MaxNoise = Width * 1181 * Seed + Height * 101 * Seed;

            var grid = new List<List<Vector3d>>();
            for (int i = 0; i < Width; ++i)
            {
                var row = new List<Vector3d>();
                for (int j = 0; j < Height; ++j)
                {
                    row.Add(new Vector3d(i, CalculateNoise(i, j) * Amplitude, j));
                }
                grid.Add(row);
            }
        }

        /// <summary>
        /// Returns result in interval 0.0 - 1.0
        /// </summary>
        public float CalculateNoise(int x, int z)
        {
            return (x * 1181 * Seed + z * 101 * Seed) / MaxNoise;
        }

        /// <summary>
        /// Element buffer object contains indices 
        /// +-+-+
        /// |/|/|
        /// +-+-+
        /// |/|/|
        /// +-+-+
        /// </summary>
        public List<int> ComputeEBO(List<List<Vector3d>> grid)
        {
            List<int> ebo = new List<int>();
            List<Vector2> texCoords = new List<Vector2>();
            for (int i = 0; i < grid.Count() - 1; i++)
            {
                for (int j = 1; j < grid.First().Count(); j++)
                {
                    // 1 square created from 2 triangles
                    ebo.Add(j + i * grid.First().Count());
                    ebo.Add(j + i * grid.First().Count() - 1);
                    ebo.Add(j + (1 + i) * grid.First().Count() - 1);

                    ebo.Add(j + i * grid.First().Count());
                    ebo.Add(j + (1 + i) * grid.First().Count() - 1);
                    ebo.Add(j + (1 + i) * grid.First().Count());

                    // tex coords for each triangle
                    texCoords.Add(new Vector2(1, 1));
                    texCoords.Add(new Vector2(0, 1));
                    texCoords.Add(new Vector2(0, 0));

                    texCoords.Add(new Vector2(1, 1));
                    texCoords.Add(new Vector2(0, 0));
                    texCoords.Add(new Vector2(1, 0));

                   // Vector3.Cross()
                }
            }
        }*/
    }
}
