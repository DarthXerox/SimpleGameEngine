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
    public class Terrain : SceneObject
    {
        public readonly int Seed = new Random().Next(); // maybe static if there are multiple terrains
        public readonly float Amplitude = 15.0f;
        public int WidthX { get; set; }
        public int WidthZ { get; set; }

        //public Dictionary<int, Dictionary<float>>
        public Terrain(int widthX, int widthZ, string texture) : base()
        {
            RawMesh = new Mesh(CalculateMesh(), new Texture2D(texture));
            //RawMesh.Texture = new Texture2D(texture);
            //RawMesh.Model
            WidthX = widthX;
            WidthZ = widthZ;
        }

        /// <summary>
        /// Returns result in interval 0.0 - 1.0
        /// </summary>
        public float GetHeight(int x, int z)
        {
            //return (x * 1181 * Seed + z * 101 * Seed) / MaxNoise;
            return 0;
        }

        private Vector3 CalculateNormal(int x, int z)
        {
            float heightLeft = GetHeight(x - 1, z);
            float heightRight = GetHeight(x + 1, z);
            float heightUp = GetHeight(x, z + 1);
            float heightDown = GetHeight(x, z - 1);
            var ret = new Vector3(heightLeft - heightRight, 
                                    2.0f, 
                                    heightDown - heightUp);
            ret.Normalize();
            return ret;
        }
        private ObjModel CalculateMesh()
        {
            ObjModel model = new ObjModel();
            for (int i = 0; i < WidthX; ++i)
            {
                for (int j = 0; j < WidthZ; ++j)
                    model.Vertices.Add(new Vector3(i, GetHeight(i, j) * Amplitude, j));
            }

            for (int i = 0; i < WidthX - 1; i++)
            {
                for (int j = 1; j < WidthZ; j++)
                {
                    // 1 square created from 2 triangles
                    model.Indices.Add(j + i * WidthZ);
                    model.Indices.Add(j + i * WidthZ - 1);
                    model.Indices.Add(j + (1 + i) * WidthZ - 1);

                    model.Indices.Add(j + i * WidthZ);
                    model.Indices.Add(j + (1 + i) * WidthZ - 1);
                    model.Indices.Add(j + (1 + i) * WidthZ);

                    // tex coords for each triangle
                    model.TextureCoordinates.Add(new Vector2(1, 1));
                    model.TextureCoordinates.Add(new Vector2(0, 1));
                    model.TextureCoordinates.Add(new Vector2(0, 0));

                    model.TextureCoordinates.Add(new Vector2(1, 1));
                    model.TextureCoordinates.Add(new Vector2(0, 0));
                    model.TextureCoordinates.Add(new Vector2(1, 0));

                    // not the most correct way but the fastest 
                    // https://www.youtube.com/watch?v=O9v6olrHPwI&list=PLRIWtICgwaX0u7Rf9zkZhLoLuZVfUksDP&index=21
                    model.Normals.Add(CalculateNormal(i, j));
                    model.Normals.Add(CalculateNormal(i, j - 1));
                    model.Normals.Add(CalculateNormal(i + 1, j - 1));

                    model.Normals.Add(CalculateNormal(i, j));
                    model.Normals.Add(CalculateNormal(i - 1, j - 1));
                    model.Normals.Add(CalculateNormal(i + 1, j));
                }
            }

            model.VerticesFloat = new float[model.Vertices.Count * 3]; // 3 flaots for each
            model.TextureCoordinatesFloat = new float[model.Vertices.Count * 2]; // 2 floats for each
            model.NormalsFloat = new float[model.Vertices.Count * 3]; //3 for each

            for (int i = 0; i < model.TextureCoordinates.Count; i++) 
            {
                model.TextureCoordinatesFloat[2 * i] = model.TextureCoordinates[i].X;
                model.TextureCoordinatesFloat[2 * i + 1] = model.TextureCoordinates[i].X;
            }

            for (int i = 0; i < model.Vertices.Count; i++)
            {
                model.VerticesFloat[3 * i] = model.Vertices[i].X;
                model.VerticesFloat[3 * i + 1] = model.Vertices[i].Y;
                model.VerticesFloat[3 * i + 2] = model.Vertices[i].Z;

                model.NormalsFloat[3 * i] = model.Normals[i].X;
                model.NormalsFloat[3 * i + 1] = model.Normals[i].Y;
                model.NormalsFloat[3 * i + 2] = model.Normals[i].Z;
            }

            return model;
        }
    }





    /*

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
        }
    }*/
}
