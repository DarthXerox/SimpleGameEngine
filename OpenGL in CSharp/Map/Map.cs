using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK;
using OpenGL_in_CSharp.Utils;
using System.Linq;
using System.ComponentModel.Design;
using System.IO;
using OpenGL_in_CSharp.InstancedDrawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using OpenGL_in_CSharp.Mesh_and_SceneObjects;

namespace OpenGL_in_CSharp
{
    public class Map
    {
        //public List<Terrain> Terrains { set; get; } = new List<Terrain>();

        //public Collidable Tree { set; get; }
        //public SceneObject TreeLeaves { set; get; }

        //public Collidable TallGrass { set; get; }

        public Terrain Terrain { private set; get; }
        public Collidable Trees { set; get; }
        public SceneObject TreeLeaves { set; get; }
        public SceneObject Borders { private set; get; }
        public Texture2D BordersNormalTexture { private set; get; }

        public SceneObject Key { private set; get; }

        public int Width { get; }
        public int Height { get; }

        public int MaxX { get => Width * HeightMap.Width; }
        public int MaxZ { get => Height * HeightMap.Height; }

        public Bitmap HeightMap { private set; get; }

        public Coins Coins { private set; get; }

        public Map(int width, int height, string heightMapFile)
        {
            Width = width;
            Height = height;
            HeightMap = new Bitmap(heightMapFile);
            Terrain = new Terrain(FilePaths.HeightMapPath, FilePaths.TextureGrass4,
                 FilePaths.BumpTexGrass4, FilePaths.MtlGold, Vector3.Zero);
            Trees = new Collidable(new NormalMappingMesh(FilePaths.ObjTreeTrunk, FilePaths.TextureTreeTrunk,
                FilePaths.MtlGold, FilePaths.BumpTexTrunk));
            TreeLeaves = new SceneObject(new NormalMappingMesh(FilePaths.ObjTreeLeaves,
                FilePaths.TextureTreeLeaves3, FilePaths.MtlGold, FilePaths.BumpTexTreeLeaves));

            Key = new SceneObject(new NormalMappingMesh(FilePaths.ObjCoin, FilePaths.TextureCoin,
                FilePaths.MtlGold, FilePaths.BumpTexCoin), new ModelTransformations()
                {
                    Position = new Vector3(30, 2, 30),
                    Scaling = new Vector3(0.25f, 0.25f, 0.25f),
                    RotX = 90
                }) ;
            /*
            TerrainInstanced = new InstancedSceneObject(Terrain.RawMesh);
            Trees = new InstancedCollidable(FilePaths.ObjTreeTrunk, FilePaths.TextureTreeTrunk);
            TreeLeaves = new InstancedSceneObject(FilePaths.ObjTreeLeaves, FilePaths.TextureTreeLeaves3);
            */

            for (int z = 0; z < Height; z++)
            {
                for (int x = 0; x < Width; x++)
                {
                    /*var ter = new Terrain(texture, HeightMap)
                    {
                        Position = new Vector3(x * (HeightMap.Width - 1), 0, z * (HeightMap.Height - 1))
                    };
                    Terrains.Add(ter);
                    */
                    Terrain.AddPosition(new Vector3(x * (HeightMap.Width - 1), 0, z * (HeightMap.Height - 1)));
                    
                }
            }

            for (int z = 0; z < HeightMap.Height; z++)
            {
                for (int x = 0; x < HeightMap.Width; x++)
                {
                    if (z % 20 == 0 && x % 20 == 0 && z > 3 && x > 3 && z < HeightMap.Height - 3 && x < HeightMap.Width - 3)
                    {
                        foreach (var sceneObject in Terrain.ModelTransformations)
                        {
                            Trees.AddPosition((new Vector4(x, GetHeight(x, z), z, 1) * sceneObject.GetModelMatrix()).Xyz);
                            TreeLeaves.AddPosition(( new Vector4(x, GetHeight(x, z), z, 1) * sceneObject.GetModelMatrix()).Xyz);
                        }
                    }
                }
            }

            List<ModelTransformations> transformations = new List<ModelTransformations>();
            for (int i = 0; i < 10; ++i)
            {
                transformations.Add(new ModelTransformations()
                {
                    Position = new Vector3(10 + i * 5, GetHeight(10 + i * 5, 5) + 3 , 5)
                });
            }
            Coins = new Coins(new NormalMappingMesh(FilePaths.ObjMossyRock1, 
                FilePaths.TextureMossyRock, FilePaths.MtlGold, FilePaths.BumpTexMossyRock), transformations);

            AddWall(MaxX - 1, 15, MaxX / 5, 3);
            Borders.AddPosition(new Vector3(0, 0, 0));
            Borders.ModelTransformations.Add(new ModelTransformations(0, -90, 0, 1, new Vector3(MaxX - 2, 0, 0)));
            Borders.ModelTransformations.Add(new ModelTransformations(0, -180, 0, 1, new Vector3(MaxX - 2, 0, MaxZ - 2)));
            Borders.ModelTransformations.Add(new ModelTransformations(0, -270, 0, 1, new Vector3(0, 0, MaxZ - 2)));
            BordersNormalTexture = new Texture2D(FilePaths.BumpTexBrickWall);

            /*
            Tree = new Collidable(FilePaths.ObjTreeTrunk, FilePaths.TextureTreeTrunk);
            Tree.Position = new Vector3(20, GetHeight(20, 20), 20);

            TreeLeaves = new SceneObject(FilePaths.ObjTreeLeaves, FilePaths.TextureTreeLeaves3);
            TreeLeaves.Position = Tree.Position;

            TallGrass = new Collidable(FilePaths.ObjMossyRock3, FilePaths.TextureMossyRock);
            TallGrass.Position = new Vector3(30, GetHeight(30, 5), 5);
            TallGrass.ScalingFactor = 2;
            */
        }

        public void SignUpForCollisionChecking(CollisionManager collisionManager)
        {
            collisionManager.CollisionChecking += Trees.OnCollisionCheck;
            collisionManager.CollisionChecking += Coins.OnCollisionCheck;
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
            Key.Draw(normalMappingProg, player);
            Coins.Draw(normalMappingProg, player);
            /*
            program.AttachModelMatrix(Terrains.First().GetModelMatrix());
            Terrains.First().Draw();
            */
            /*
            foreach (var t in Terrains)
            {
                program.AttachModelMatrix(t.GetModelMatrix());
                t.Draw();
                var init = Tree.Position;

                program.AttachModelMatrix(Tree.GetModelMatrix() * t.GetModelMatrix());
                Tree.Draw();
                program.AttachModelMatrix(TreeLeaves.GetModelMatrix() * t.GetModelMatrix());
                TreeLeaves.Draw();

                float x = 15;
                
                while (Tree.Position.X  + x < HeightMap.Width)
                {
                    var nextPos = new Vector3(Tree.Position.X + x, GetHeight(Tree.Position.X + x, Tree.Position.Z + x) - 1, Tree.Position.Z + x);
                    Tree.Position = nextPos;
                    program.AttachModelMatrix(Tree.GetModelMatrix() * t.GetModelMatrix());
                    Tree.Draw();

                    TreeLeaves.Position = nextPos;
                    program.AttachModelMatrix(TreeLeaves.GetModelMatrix() * t.GetModelMatrix());
                    TreeLeaves.Draw();

                    x += 15;
                }
                Tree.Position = init;
                TreeLeaves.Position = init;

                program.AttachModelMatrix(TallGrass.GetModelMatrix() * t.GetModelMatrix());
                TallGrass.Draw();
            }
            */
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
            //HelpPrinter.PrintList(model.Vertices);
            for (int y = 0; y < height - 1; y++)
            {
                for (int x = 0; x < width - 1; x++)
                {
                    /*
                    int topLeft =  y * width + x;
                    int bottomLeft = (y + 1) * width + x;
                    int topRight = topLeft + 1;
                    int bottomRight = bottomLeft + 1;

                    // 1 square created from 2 triangles
                    model.Indices.Add(topLeft);
                    model.Indices.Add(bottomLeft);
                    model.Indices.Add(bottomRight);

                    model.Indices.Add(bottomRight);
                    model.Indices.Add(topRight);
                    model.Indices.Add(topLeft);
                    */
                    int bottomLeft = y * width + x;
                    int bottomRight = bottomLeft + 1;
                    int topLeft = (y + 1) * width + x;
                    int topRight = topLeft + 1;

                    model.Indices.Add(bottomLeft);
                    model.Indices.Add(bottomRight);
                    model.Indices.Add(topLeft);

                    model.Indices.Add(topLeft);
                    model.Indices.Add(bottomRight);
                    model.Indices.Add(topRight);
                }
            }

            //HelpPrinter.PrintList(model.Indices);


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

            Borders = new SceneObject(new Mesh(model, FilePaths.TextureBrickWall, FilePaths.MtlGold));
            //Borders = new SceneObject(new Mesh(model, new Texture2D(FilePaths.TextureBrickWall)));
            //Borders = new InstancedSceneObject(new Mesh(model, FilePaths.TextureBrickWall));
        }
    }
}
