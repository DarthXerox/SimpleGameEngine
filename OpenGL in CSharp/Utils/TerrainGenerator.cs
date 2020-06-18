using System;
using System.Drawing;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace OpenGL_in_CSharp.Utils
{
    public class Terrain : SceneObject
    {
        /*
        public readonly int Seed = new Random().Next(); // maybe static if there are multiple terrains
        public readonly float Amplitude = 5.0f;
        
        public Random Rand { private set; get; } = new Random();

        public readonly float MaxColor = 256f * 256f * 256f;
        */
        public readonly float MaxHeight = 15f;

        public readonly int TexturesPerSide = 1;

        public int WidthX { get; set; }
        public int WidthZ { get; set; }
        public int PointsPerSide { get; }
        public Bitmap HeightMap { get; }

        /*
        public Terrain(int widthX, int widthZ, string texture) : base()
        {
            WidthX = widthX;
            WidthZ = widthZ;
            HeightMap = new Bitmap(FilePaths.HeightMapPath);
            RawMesh = new Mesh(CalculateMesh(), new Texture2D(texture));           
        }
        */
        public Terrain(string texture)
        {
            HeightMap = new Bitmap(FilePaths.HeightMapPath);
            WidthX = HeightMap.Width;
            WidthZ = HeightMap.Height;
            RawMesh = new Mesh(CalculateMesh(), new Texture2D(texture));
        }

        public Terrain(string texture, Bitmap heightMap)
        {
            HeightMap = heightMap;
            WidthX = HeightMap.Width;
            WidthZ = HeightMap.Height;
            RawMesh = new Mesh(CalculateMesh(), new Texture2D(texture));
        }

        public float GetHeight(int x, int z)
        {
            return GetHeightFromMap(x, z) * MaxHeight;
        }
        /*
        public float GetHeight(int x, int z)
        {
            return GetInterpolatedNoise(x / 4.0f, z / 4.0f) * Amplitude +
                GetInterpolatedNoise(x / 2.0f, z / 2.0f) * Amplitude / 3f +
                GetInterpolatedNoise(x , z) * Amplitude / 9f;
        }

        /// <summary>
        /// Returns result in interval 0.0 - 1.0
        /// </summary>
        public float GetNoise(int x, int z)
        {
            Rand = new Random(x * 49632 + z * 325176 + Seed);
            return (float)(Rand.NextDouble() * 2.0 - 1.0);
        }

        public float GetSmoothNoise(int x, int z)
        {
            float corners =( GetNoise(x +1,z+1 ) + GetNoise(x+1, z-1) + GetNoise(x - 1, z + 1) + GetNoise(x - 1, z - 1)) / 16.0f;
            float sides = (GetNoise(x+1, z) + GetNoise(x-1, z) + GetNoise(x, z+1) + GetNoise(x, z-1)) / 8.0f;
            return corners + sides + GetNoise(x, z) / 4.0f;
        }
        */
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

        /*
        private float GetInterpolatedNoise(float x, float z)
        {
            int intX = (int)x;
            int intZ = (int)z;
            float fracX = x - intX;
            float fracZ = z - intZ;

            float v1 = GetSmoothNoise(intX, intZ);
            float v2 = GetSmoothNoise(intX + 1, intZ);
            float v3 = GetSmoothNoise(intX, intZ + 1);
            float v4 = GetSmoothNoise(intX + 1, intZ + 1);
            float i1 = Interpolate(v1, v2, fracX);
            float i2 = Interpolate(v3, v4, fracX);
            return Interpolate(i1, i2, fracZ);
        }

        private float Interpolate(float a, float b, float blend)
        {
            double theta = blend * Math.PI;
            float f = (float)(1f - Math.Cos(theta)) * 0.5f;
            return a * (1f - f) + b * f;
        }*/

        /*
        private ObjModel CalculateMesh()
        {
            ObjModel model = new ObjModel();
            for (int i = 0; i < WidthX; ++i)
            {
                for (int j = 0; j < WidthZ; ++j)
                {
                    model.Vertices.Add(new Vector3(i, GetHeight(i, j), j));
                }
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
                   
                    // not the most correct way but the fastest 
                    // https://www.youtube.com/watch?v=O9v6olrHPwI&list=PLRIWtICgwaX0u7Rf9zkZhLoLuZVfUksDP&index=21
                }
            }

            for (int x = 0; x < WidthX; x++)
            {
                for (int z = 0; z < WidthZ; z++)
                {
                    model.Normals.Add(CalculateNormal(x, z));
                    if (x % 2 == 0)
                    {
                        if (z % 2 == 0)
                        {
                            model.TextureCoordinates.Add(new Vector2(0, 1));
                        } else
                        {
                            model.TextureCoordinates.Add(new Vector2(0, 0));
                        }

                    } else
                    {
                        if (z % 2 == 0)
                        {
                            model.TextureCoordinates.Add(new Vector2(1, 1));
                        }
                        else
                        {
                            model.TextureCoordinates.Add(new Vector2(1, 0));
                        }
                    }
                    //model.TextureCoordinates.Last() *= 40;
                }
            }

            model.VerticesFloat = new float[model.Vertices.Count * 3]; // 3 flaots for each
            model.TextureCoordinatesFloat = new float[model.Vertices.Count * 2]; // 2 floats for each
            model.NormalsFloat = new float[model.Vertices.Count * 3]; //3 for each
            
            
            for (int i = 0; i < model.TextureCoordinates.Count; i++) 
            {
                model.TextureCoordinatesFloat[2 * i] = model.TextureCoordinates[i].X;
                model.TextureCoordinatesFloat[2 * i + 1] = model.TextureCoordinates[i].Y;
            }

            for (int i = 0; i < model.Vertices.Count; i++)
            {
                model.VerticesFloat[3 * i] = model.Vertices[i].X;
                model.VerticesFloat[3 * i + 1] = model.Vertices[i].Y;
                model.VerticesFloat[3 * i + 2] = model.Vertices[i].Z;
            }

            for (int i = 0; i < model.Normals.Count; i++)
            {
                model.NormalsFloat[3 * i] = model.Normals[i].X;
                model.NormalsFloat[3 * i + 1] = model.Normals[i].Y;
                model.NormalsFloat[3 * i + 2] = model.Normals[i].Z;
            }
            return model;
        }
        */

        private ObjModel CalculateMesh()
        {
            ObjModel model = new ObjModel();
            for (int z = 0; z < WidthZ; z++)
            {
                for (int x = 0; x < WidthX; x++)
                {
                    model.Vertices.Add(new Vector3(x, GetHeight(x, z), z));
                    model.TextureCoordinates.Add(new Vector2(
                        (float)x / (WidthX + 1),
                        (float)z / (WidthZ + 1)) * TexturesPerSide);
                    model.Normals.Add(CalculateNormal(x, z));
                }
            }

            for (int z = 0; z < WidthZ - 1; z++)
            {
                for (int x = 0; x < WidthX - 1; x++)
                {
                    int topLeft = z * WidthX + x;
                    int bottomLeft = (z + 1) * WidthX + x;
                    int topRight = topLeft + 1;
                    int bottomRight = bottomLeft + 1;

                    // 1 square created from 2 triangles
                    model.Indices.Add(topLeft);
                    model.Indices.Add(bottomLeft);
                    model.Indices.Add(bottomRight);

                    model.Indices.Add(bottomRight);
                    model.Indices.Add(topRight);
                    model.Indices.Add(topLeft);
                }
            }

            

            model.VerticesFloat = new float[model.Vertices.Count * 3]; // 3 flaots for each
            model.TextureCoordinatesFloat = new float[model.Vertices.Count * 2]; // 2 floats for each
            model.NormalsFloat = new float[model.Vertices.Count * 3]; //3 for each


            for (int i = 0; i < model.TextureCoordinates.Count; i++)
            {
                model.TextureCoordinatesFloat[2 * i] = model.TextureCoordinates[i].X;
                model.TextureCoordinatesFloat[2 * i + 1] = model.TextureCoordinates[i].Y;
            }

            for (int i = 0; i < model.Vertices.Count; i++)
            {
                model.VerticesFloat[3 * i] = model.Vertices[i].X;
                model.VerticesFloat[3 * i + 1] = model.Vertices[i].Y;
                model.VerticesFloat[3 * i + 2] = model.Vertices[i].Z;
            }

            for (int i = 0; i < model.Normals.Count; i++)
            {
                model.NormalsFloat[3 * i] = model.Normals[i].X;
                model.NormalsFloat[3 * i + 1] = model.Normals[i].Y;
                model.NormalsFloat[3 * i + 2] = model.Normals[i].Z;
            }
            return model;
        }

        /// <summary>
        /// Gets given pixel's greyscale, which is then converted to interval [-1f, 1f]
        /// </summary>
        public float GetHeightFromMap(int x, int z)
        {
            if (x < 0 || z < 0 || x >= HeightMap.Width || z >= HeightMap.Height)
            {
                return 0.0f;
            }
            return GetColorGreyScale(HeightMap.GetPixel(x, z)) * 2f - 1f;
        }

        private float GetColorGreyScale(Color col)
        {
            return (col.R / 255f + col.G / 255f + col.B / 255f ) / 3.0f;
        }
    }

    /// <summary>
    /// Reads data for a heatmap from
    /// http://www.arendpeter.com/Perlin_Noise.html
    /// http://devmag.org.za/2009/04/25/perlin-noise/
    /// </summary>
    /// 

    
}
    