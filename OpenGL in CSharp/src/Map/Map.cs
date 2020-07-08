using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK;
using OpenGL_in_CSharp.Utils;
using OpenGL_in_CSharp.Mesh_and_SceneObjects;

namespace OpenGL_in_CSharp
{
    public class Map
    {
        public Terrain Terrain { private set; get; }
        public Collidable Trees { set; get; }
        public SceneObject TreeLeaves { set; get; }
        public SceneObject Borders { private set; get; }
        public Texture2D BordersNormalTexture { private set; get; }

        public int Width { get; }
        public int Height { get; }

        public int MaxX { get => Width * HeightMap.Width; }
        public int MaxZ { get => Height * HeightMap.Height; }

        public Bitmap HeightMap { private set; get; }

        public FloatingStone Stones { private set; get; }

        /// <summary>
        /// This is the backbone for all the objects in the game
        /// It keeps track of all the trees, their leaves, stones, terrains...
        /// </summary>
        public Map(int width, int height, string heightMapFile)
        {
            Width = width;
            Height = height;
            HeightMap = new Bitmap(heightMapFile);
            Terrain = new Terrain(FilePaths.HeightMapPath, FilePaths.TextureGrass4,
                 FilePaths.BumpTexGrass4, FilePaths.MtlEmerald, Vector3.Zero);
            Trees = new Collidable(new NormalMappingMesh(FilePaths.ObjTreeTrunk, FilePaths.TextureTreeTrunk,
                FilePaths.MtlBronze, FilePaths.BumpTexTrunk));
            TreeLeaves = new SceneObject(new NormalMappingMesh(FilePaths.ObjTreeLeaves,
                FilePaths.TextureTreeLeaves3, FilePaths.MtlEmerald, FilePaths.BumpTexTreeLeaves));

            for (int z = 0; z < Height; z++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Terrain.AddPosition(new Vector3(x * (HeightMap.Width - 1), 0, z * (HeightMap.Height - 1)));
                }
            }

            for (int z = 0; z < HeightMap.Height; z++)
            {
                for (int x = 0; x < HeightMap.Width; x++)
                {
                    if (z % 35 == 0 && x % 35 == 0 && z > 3 && x > 3 && z < HeightMap.Height - 3 && x < HeightMap.Width - 3)
                    {
                        foreach (var sceneObject in Terrain.ModelTransformations)
                        {
                            // I move it a litte bit down, so the tree nicely "grows" from the ground
                            Trees.AddPosition((new Vector4(x, GetHeight(x, z), z, 1) * 
                                sceneObject.GetModelMatrix()).Xyz - Vector3.UnitY);
                            TreeLeaves.AddPosition((new Vector4(x, GetHeight(x, z), z, 1) * 
                                sceneObject.GetModelMatrix()).Xyz - Vector3.UnitY);
                        }
                    }
                }
            }

            // in middle of each terrain (2x2 map) and in the very middle of the map
            /*
            List<ModelTransformations> transformations = new List<ModelTransformations>()
            {
                new ModelTransformations() { Position = new Vector3(MaxX / 2, GetHeight(MaxX / 2, MaxZ / 2) + 3, MaxZ / 2) }, // the very middle
                new ModelTransformations() { Position = new Vector3(MaxX / 4, GetHeight(MaxX / 4, MaxZ / 4) + 3, MaxZ / 4) },
                new ModelTransformations() { Position = new Vector3( 3 * MaxX / 4, GetHeight(3 * MaxX / 4, 3 * MaxZ / 4) + 3, 3 * MaxZ / 4) },
                new ModelTransformations() { Position = new Vector3( 3 * MaxX / 4, GetHeight(3 * MaxX / 4, MaxZ / 4) + 3, MaxZ / 4) },
                new ModelTransformations() { Position = new Vector3(MaxX / 4, GetHeight(MaxX / 4, 3 * MaxZ / 4) + 3, 3 * MaxZ / 4) },
            };*/

            // for simplicity I put 4 of the stones in the map corners and one in its middle
            List<ModelTransformations> transformations = new List<ModelTransformations>()
            {
                new ModelTransformations() { Position = new Vector3(MaxX / 2, GetHeight(MaxX / 2, MaxZ / 2) + 3, MaxZ / 2) }, // the very middle
                new ModelTransformations() { Position = new Vector3(10, GetHeight(10, 10) + 3, 10) },
                new ModelTransformations() { Position = new Vector3(MaxX - 10, GetHeight(MaxX - 10, 10) + 3, 10) },
                new ModelTransformations() { Position = new Vector3(10, GetHeight(10, MaxZ - 10) + 3, MaxZ - 10) },
                new ModelTransformations() { Position = new Vector3(MaxX - 7, GetHeight(MaxX - 7, MaxZ - 7) + 3, MaxZ - 7) }
            };

            Stones = new FloatingStone(new NormalMappingMesh(FilePaths.ObjMossyRock1, 
                FilePaths.TextureMossyRock, FilePaths.MtlChrome, FilePaths.BumpTexMossyRock), transformations);

            AddWall(MaxX - 1, 15, MaxX / 5, 3);
            Borders.AddPosition(new Vector3(0, 0, 0));
            Borders.ModelTransformations.Add(new ModelTransformations(0, -90, 0, 1, new Vector3(MaxX - 2, 0, 0)));
            Borders.ModelTransformations.Add(new ModelTransformations(0, -180, 0, 1, new Vector3(MaxX - 2, 0, MaxZ - 2)));
            Borders.ModelTransformations.Add(new ModelTransformations(0, -270, 0, 1, new Vector3(0, 0, MaxZ - 2)));
            BordersNormalTexture = new Texture2D(FilePaths.BumpTexBrickWall);
        }

        public void SignUpForCollisionChecking(CollisionManager collisionManager)
        {
            collisionManager.CollisionChecking += Trees.OnCollisionCheck;
            collisionManager.CollisionChecking += Stones.OnCollisionCheck;
        }

        public float GetHeight(Vector2 pos)
        {
            return GetHeight(pos.X, pos.Y);
        }

        public float GetHeight(float x, float z)
        {
            x %= HeightMap.Width;
            z %= HeightMap.Height;

            try
            {
                return Terrain.GetInterpolatedHeight(x, z);
            } catch (InvalidOperationException)
            {
                return 0f;
            }
        }

        /// <summary>
        /// Both programs need to have been attached a camera position, all lights and fog before they are drawn
        /// </summary>
        /// <param name="normalMappingProg"></param>
        /// <param name="fakeNormalMappingProg"></param>
        public void DrawMap(LightsProgram normalMappingProg, LightsProgram fakeNormalMappingProg, 
            Player player, int uniformSampler2 = 1)
        {
            fakeNormalMappingProg.Use();
            BordersNormalTexture.Use(uniformSampler2);
            fakeNormalMappingProg.AttachUniformVector3(Vector3.UnitX, "tangent");
            fakeNormalMappingProg.AttachUniformVector3(Vector3.UnitY, "biTangent");
            Borders.Draw(fakeNormalMappingProg, player, Math.Max(MaxZ, MaxX));
            Terrain.Draw(fakeNormalMappingProg, player, Terrain.GetDiagonalLength() + 100);

            normalMappingProg.Use();
            Trees.Draw(normalMappingProg, player);
            TreeLeaves.Draw(normalMappingProg, player);

            Stones.Draw(normalMappingProg, player);
        }

        /// <summary>
        /// Map is expected to have all vertices at level 0 at its edges
        /// </summary>
        public void AddWall(int width, int height, int texPerWidth, int texPerHeight)
        {
            ObjModel model = new ObjModel();
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    model.Vertices.Add(new Vector3(x, y, 0));
                    model.TextureCoordinates.Add(new Vector2(
                        (float)x / (width + 1) * texPerWidth,
                        (float)y / (height + 1) * texPerHeight) );
                    model.Normals.Add(new Vector3(0, 0, 1));
                }
            }
            for (int y = 0; y < height - 1; y++)
            {
                for (int x = 0; x < width - 1; x++)
                {
                    int bottomLeft = y * width + x;
                    int bottomRight = bottomLeft + 1;
                    int topLeft = (y + 1) * width + x;
                    int topRight = topLeft + 1;

                    model.Indices.Add((uint)bottomLeft);
                    model.Indices.Add((uint)bottomRight);
                    model.Indices.Add((uint)topLeft);

                    model.Indices.Add((uint)topLeft);
                    model.Indices.Add((uint)bottomRight);
                    model.Indices.Add((uint)topRight);
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
            
            Borders = new SceneObject(new Mesh(model, FilePaths.TextureBrickWall, FilePaths.MtlBronze));
        }
    }
}
