using System;
using System.Collections.Generic;
using System.Drawing;
using SimpleEngine.Data;
using SimpleEngine.GameScene;
using SimpleEngine.Utils;
using OpenTK;
using SimpleEngine.Collisions;

namespace SimpleEngine.WorldObjects
{
    /// <summary>
    /// This is the backbone for all the objects in the game
    /// It keeps track of all the trees, their leaves, stones, terrains...
    /// </summary>
    public class World : IDisposable
    {
        public int Width { get; }
        public int Height { get; }
        public Terrain Terrain { get; }
        public Collidable Trees { get; }
        public WorldObject TreeLeaves { get; }
        public WorldObject Borders { get; }
        public Texture2D BordersNormalTexture { get; }
        public FloatingStone Stones { get; }
        // when disposing of terrain this is disposed too!
        public Bitmap HeightMap { private set; get; }
        public int MaxX => Width * HeightMap.Width;
        public int MaxZ => Height * HeightMap.Height;

        public World(int width, int height, IDictionary<string, Bitmap> textures, IDictionary<string, ObjModel> models,
            IDictionary<string, Material> materials)
        {
            Width = width;
            Height = height;

            var terrainPositions = new List<Transformations>();
            var treeLeavesPositions = new List<Transformations>();
            var treeTrunksPositions = new List<Transformations>();

            HeightMap = textures[FilePaths.TextureHeightMap];
            ObjModel bordersModel = CalculateBordersObjModel(MaxX - 1, 15, MaxX / 5, 3);
            var bordersPositions = new List<Transformations>()
            {
                new Transformations() { Position = new Vector3(0, 0, 0) },
                new Transformations(0, -90, 0, 1, new Vector3(MaxX - 2, 0, 0)),
                new Transformations(0, -180, 0, 1, new Vector3(MaxX - 2, 0, MaxZ - 2)),
                new Transformations(0, -270, 0, 1, new Vector3(0, 0, MaxZ - 2))
            };
            Borders = new WorldObject(new Mesh(bordersModel, textures[FilePaths.TextureBrickWall], materials[FilePaths.MtlBronze]), 
                bordersPositions);


            for (int z = 0; z < Height; z++)
            {
                for (int x = 0; x < Width; x++)
                {
                    terrainPositions.Add(new Transformations()
                    { 
                        Position = new Vector3(x * (HeightMap.Width - 1), 0, z * (HeightMap.Height - 1)) 
                    });

                }
            }

            Material mat = materials[FilePaths.MtlEmerald];
            //mat.Shininess = 300;
            Terrain = new Terrain(HeightMap, textures[FilePaths.TextureGrass4],
                 textures[FilePaths.BumpTexGrass4], mat, terrainPositions);
            
            // in middle of each terrain (2x2 map) and in the very middle of the map
            List<Transformations> stonesPositions = new List<Transformations>()
            {
                new Transformations() { Position = new Vector3(MaxX / 2, GetHeight(MaxX / 2, MaxZ / 2) + 3, MaxZ / 2) }, // the very middle
                new Transformations() { Position = new Vector3(MaxX / 4, GetHeight(MaxX / 4, MaxZ / 4) + 3, MaxZ / 4) },
                new Transformations() { Position = new Vector3( 3 * MaxX / 4, GetHeight(3 * MaxX / 4, 3 * MaxZ / 4) + 3, 3 * MaxZ / 4) },
                new Transformations() { Position = new Vector3( 3 * MaxX / 4, GetHeight(3 * MaxX / 4, MaxZ / 4) + 3, MaxZ / 4) },
                new Transformations() { Position = new Vector3(MaxX / 4, GetHeight(MaxX / 4, 3 * MaxZ / 4) + 3, 3 * MaxZ / 4) },
            };

            var rand = new Random();
            for (int z = 0; z < MaxZ; z++)
            {
                for (int x = 0; x < MaxX; x++)
                {
                    if (z % 20 == 0 && x % 20 == 0 && z > 3 && x > 3 && z < MaxZ - 3 && x < MaxX - 3)
                    {
                        if (rand.NextDouble() <= 0.9)
                        {
                            // I move it a litte bit down, so the tree nicely "grows" from the ground
                            treeTrunksPositions.Add(new Transformations()
                            {
                                Position = (new Vector4(x, GetHeight(x, z), z, 1)).Xyz - Vector3.UnitY
                            });
                            treeLeavesPositions.Add(new Transformations()
                            {
                                Position = (new Vector4(x, GetHeight(x, z), z, 1)).Xyz - Vector3.UnitY
                            });
                        }
                    }
                }
            }

            BordersNormalTexture = new Texture2D(textures[FilePaths.BumpTexBrickWall]);
            Trees = new Collidable(new NormalMappingMesh(models[FilePaths.ObjTreeTrunk], textures[FilePaths.TextureTreeTrunk],
                textures[FilePaths.BumpTexTrunk], materials[FilePaths.MtlTreeTrunk]), treeTrunksPositions);
            TreeLeaves = new WorldObject(new NormalMappingMesh(models[FilePaths.ObjTreeLeaves], textures[FilePaths.TextureTreeLeaves3],
                textures[FilePaths.BumpTexTreeLeaves], materials[FilePaths.MtlEmerald]), treeLeavesPositions);
            Stones = new FloatingStone(new NormalMappingMesh(models[FilePaths.ObjMossyRock1], textures[FilePaths.TextureMossyRock],
                textures[FilePaths.BumpTexMossyRock], materials[FilePaths.MtlMossyRock1]), stonesPositions);
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
        /// Both programs need to have been attached a camera position, all lights and a fog before they are drawn
        /// </summary>
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
        /// Terrain is expected to have all vertices at level 0 at its edges
        /// So we can nicely put them next to each other
        /// </summary>
        public ObjModel CalculateBordersObjModel(int width, int height, int texPerWidth, int texPerHeight)
        {
            ObjModel model = new ObjModel();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    model.Vertices.Add(new Vector3(x, y, 0));
                    model.TextureCoordinates.Add(new Vector2(
                        (float)x / (width + 1) * texPerWidth,
                        (float)y / (height + 1) * texPerHeight));
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

            return model;   
        }

        public void Dispose()
        {
            Trees.Dispose();
            TreeLeaves.Dispose();
            Borders.Dispose();
            Terrain.Dispose();
            HeightMap = null; // this invalidates MaxX and MaxZ
            Stones.Dispose();
            BordersNormalTexture.Dispose();
        }
    }
}
