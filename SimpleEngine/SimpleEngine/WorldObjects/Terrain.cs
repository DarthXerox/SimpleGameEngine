using System;
using System.Drawing;
using System.Threading.Tasks;
using OpenTK;
using SimpleEngine.Utils;
using SimpleEngine.Data;
using SimpleEngine.GameScene;

namespace SimpleEngine.WorldObjects
{
    public class Terrain : WorldObject
    {
        public readonly float MaxHeight = 10f;

        public readonly int TexturesPerSide = 96;

        public int ShaderTextureSampler2 { get; } = 1;

        public int WidthX { get;} //{ get => HeightMap.Width; } 
        public int WidthZ { get; } //{ get => HeightMap.Height; }
        public Bitmap HeightMap { get; }

        private object Locker = new object();
        public Texture2D NormalTexture { get; }


        public Terrain(string bitmapFile, string colorTextureFile, string normalTextureFile, string materialFile, Vector3 position)
            : base(new ModelTransformations() { Position = position })
        {
            HeightMap = new Bitmap(bitmapFile);
            WidthX = HeightMap.Width;
            WidthZ = HeightMap.Height;
            RawMesh = new Mesh(CalculateMesh(), colorTextureFile, materialFile);
            NormalTexture = new Texture2D(normalTextureFile);
        }

        public Terrain(Bitmap heightMap, Bitmap colTexture, Bitmap normalTexture, Material material, Vector3 position)
            : base(new ModelTransformations() { Position = position })
        {
            HeightMap = heightMap;
            WidthX = HeightMap.Width;
            WidthZ = HeightMap.Height;
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
        
        public async Task<float> GetHeightAsync(int x, int z)
        {
            return await Task.Run(() => GetHeightFromMap(x, z));
            //return height * MaxHeight;
        }
        
        
        private async Task<Vector3> CalculateNormalAsync(int x, int z)
        {
            var heightLeftTask = GetHeightAsync(x - 1, z);
            var heightRightTask = GetHeightAsync(x + 1, z);
            var heightUpTask = GetHeightAsync(x, z + 1);
            var heightDownTask = GetHeightAsync(x, z - 1);
            var ret = new Vector3(await heightLeftTask - await heightRightTask,
                                    2.0f,
                                    await heightDownTask - await heightUpTask);
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

        private float Interpolate(float a, float b, float blend)
        {
            double theta = blend * Math.PI;
            float f = (float)(1f - Math.Cos(theta)) * 0.5f;
            return a * (1f - f) + b * f;
        }
        
        private ObjModel CalculateMesh()
        {
            ObjModel model = new ObjModel();
            for (int z = 0; z < WidthZ; z++)
            {
                for (int x = 0; x < WidthX; x++)
                {
                    //var calculteNormalTask = CalculateNormalAsync(x, z);
                    //var getHeightTask = GetHeightAsync(x, z);
                    if (z < WidthZ - 1 && x < WidthX - 1)
                    {
                        int topLeft = z * WidthX + x;
                        int bottomLeft = (z + 1) * WidthX + x;
                        int topRight = topLeft + 1;
                        int bottomRight = bottomLeft + 1;

                        // 1 square created from 2 triangles
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


            /*
            for (int z = 0; z < WidthZ - 1; z++)
            {
                for (int x = 0; x < WidthX - 1; x++)
                {
                    int topLeft = z * WidthX + x;
                    int bottomLeft = (z + 1) * WidthX + x;
                    int topRight = topLeft + 1;
                    int bottomRight = bottomLeft + 1;

                    // 1 square created from 2 triangles
                    model.Indices.Add((uint)topLeft);
                    model.Indices.Add((uint)bottomLeft);
                    model.Indices.Add((uint)bottomRight);

                    model.Indices.Add((uint)bottomRight);
                    model.Indices.Add((uint)topRight);
                    model.Indices.Add((uint)topLeft);
                }
            }
            */
            /*
            var y = GetHeightAsync(5, 5);
            float f = y.Result;
            var y2 = CalculateNormalAsync(5, 5);
            */
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
            lock (Locker)
            {
                if (x < 0 || z < 0 || x >= HeightMap.Width || z >= HeightMap.Height)
                {
                    return 0.0f;
                }
                return GetColorGreyScale(HeightMap.GetPixel(x, z)) * 2f - 1f;
            }
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
    