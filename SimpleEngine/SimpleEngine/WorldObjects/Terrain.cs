using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK;
using SimpleEngine.Data;
using SimpleEngine.GameScene;
using SimpleEngine.Utils;

namespace SimpleEngine.WorldObjects
{
    /// <summary>
    /// Terrain is a worldobject with mesh calculated from a heatmap
    /// </summary>
    public class Terrain : WorldObject
    {
        public readonly float MaxHeight = 10f;
        public readonly int TexturesPerSide = 32;
        public int ShaderTextureSampler2 { get; } = 1;
        public Bitmap HeightMap { get; }
        public int WidthX  => HeightMap.Width; 
        public int WidthZ => HeightMap.Height;
        public Texture2D NormalTexture { get; }

        public Terrain(Bitmap heightMap, Bitmap colTexture, Bitmap normalTexture, Material material, List<Transformations> transformations)
            : base(new Transformations())
        {
            ModelTransformations = transformations;
            HeightMap = heightMap;
            RawMesh = new Mesh(CalculateMesh(), colTexture, material);
            NormalTexture = new Texture2D(normalTexture);
        }

        public float GetDiagonalLength()
        {
            return Vector2.Distance(new Vector2(WidthX, 0), new Vector2(0, WidthZ));
        }

        public float GetHeight(int x, int z)
        {
            return GetHeightFromMap(x, z) * MaxHeight;
        }
        
        /// <summary>
        /// This is not the most correct way to calculate normals for a given point
        /// but is much easier and very accurate
        /// </summary>
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

       
        public float GetInterpolatedHeight(float x, float z)
        {
            int intX = (int)x;
            int intZ = (int)z;
            float fracX = x - intX;
            float fracZ = z - intZ;

            float v1 = GetHeight(intX, intZ);
            float v2 = GetHeight(intX + 1, intZ);
            float v3 = GetHeight(intX, intZ + 1);
            float v4 = GetHeight(intX + 1, intZ + 1);
            float i1 = Interpolate(v1, v2, fracX);
            float i2 = Interpolate(v3, v4, fracX);
            return Interpolate(i1, i2, fracZ);
        }

        /// <summary>
        /// Finds interpolated value between 2 values based on the given blend factor
        /// </summary>
        /// <param name="a">First value</param>
        /// <param name="b">Second value</param>
        /// <param name="blend">Blend factor</param>
        private float Interpolate(float a, float b, float blend)
        {
            // maps the blend factor to interval [-1, 0] 
            float f = (float)(1f - Math.Cos(blend * Math.PI)) * 0.5f;
            return a * (1f - f) + b * f;
        }
        
        private ObjModel CalculateMesh()
        {
            ObjModel model = new ObjModel();
            for (int z = 0; z < WidthZ; z++)
            {
                for (int x = 0; x < WidthX; x++)
                {
                    if (z < WidthZ - 1 && x < WidthX - 1)
                    {
                        int topLeft = z * WidthX + x;
                        int bottomLeft = (z + 1) * WidthX + x;
                        int topRight = topLeft + 1;
                        int bottomRight = bottomLeft + 1;

                        // 1 square is created from 2 triangles
                        // indices must be in counter clockwise order, so the faceculling works correctly
                        model.Indices.Add((uint)topLeft);
                        model.Indices.Add((uint)bottomLeft);
                        model.Indices.Add((uint)bottomRight);

                        model.Indices.Add((uint)bottomRight);
                        model.Indices.Add((uint)topRight);
                        model.Indices.Add((uint)topLeft);
                    } 

                    model.TextureCoordinates.Add(new Vector2(
                        (float)x / (WidthX + 1),
                        (float)z / (WidthZ + 1)) * TexturesPerSide);
                    model.Vertices.Add(new Vector3(x, GetHeight(x, z), z));
                    model.Normals.Add(CalculateNormal(x, z));
                }
            }

            model.VerticesFloat = new float[model.Vertices.Count * 3]; // 3 floats for each
            model.TextureCoordinatesFloat = new float[model.Vertices.Count * 2]; // 2 floats for each
            model.NormalsFloat = new float[model.Vertices.Count * 3]; // 3 for each


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

        public override void Draw(LightsProgram lightsProgram, Player player, float maxDistance = 100)
        {
            NormalTexture.Use(ShaderTextureSampler2);
            lightsProgram.AttachUniformVector3(-Vector3.UnitZ, "tangent");
            lightsProgram.AttachUniformVector3(Vector3.UnitX, "biTangent");
            base.Draw(lightsProgram, player, maxDistance);
        }

        public override void Dispose()
        {
            base.Dispose();
            HeightMap.Dispose();
            NormalTexture.Dispose();
        }
    }
}
    