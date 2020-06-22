using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK;
using OpenGL_in_CSharp.Utils;
using System.Linq;
using System.ComponentModel.Design;
using System.IO;
using OpenGL_in_CSharp.InstancedDrawing;
using static OpenGL_in_CSharp.InstancedDrawing.InstancedSceneObject;
using System.Runtime.InteropServices;

namespace OpenGL_in_CSharp
{
    public class Map
    {
        //public List<Terrain> Terrains { set; get; } = new List<Terrain>();

        //public Collidable Tree { set; get; }
        //public SceneObject TreeLeaves { set; get; }

        //public Collidable TallGrass { set; get; }

        public Terrain Terrain { private set; get; }
        public InstancedCollidable Trees { set; get; }
        public InstancedSceneObject TreeLeaves { set; get; }
        public InstancedSceneObject TerrainInstanced { set; get; }

        public SceneObject Wall { private set; get; }
        
        public InstancedSceneObject Borders { private set; get; }


        public int Width { get; }
        public int Height { get; }

        public int MaxX { get => Width * HeightMap.Width; }
        public int MaxZ { get => Height * HeightMap.Height; }

        public Bitmap HeightMap { private set; get; }

        public Map(int width, int height, string texture, string heightMapFile)
        {
            Width = width;
            Height = height;
            HeightMap = new Bitmap(heightMapFile);
            Terrain = new Terrain(texture, HeightMap);
            TerrainInstanced = new InstancedSceneObject(Terrain.RawMesh);
            Trees = new InstancedCollidable(FilePaths.ObjTreeTrunk, FilePaths.TextureTreeTrunk);
            TreeLeaves = new InstancedSceneObject(FilePaths.ObjTreeLeaves, FilePaths.TextureTreeLeaves3);

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
                    TerrainInstanced.AddPosition(new Vector3(x * (HeightMap.Width - 1), 0, z * (HeightMap.Height - 1)));
                    
                }
            }

            for (int z = 0; z < HeightMap.Height; z++)
            {
                for (int x = 0; x < HeightMap.Width; x++)
                {
                    if (z % 20 == 0 && x % 20 == 0 && z > 3 && x > 3 && z < HeightMap.Height - 3 && x < HeightMap.Width - 3)
                    {
                        foreach (var sceneObject in TerrainInstanced.ModelTransformations)
                        {
                            Trees.AddPosition((new Vector4(x, GetHeight(x, z), z, 1) * sceneObject.GetModelMatrix()).Xyz);
                            TreeLeaves.AddPosition(( new Vector4(x, GetHeight(x, z), z, 1) * sceneObject.GetModelMatrix()).Xyz);
                        }
                    }
                }
            }

            Console.WriteLine($"Trunks count: {Trees.Collidables.Count}");
            AddWall(MaxX - 1, 15, MaxX / 5, 3);
            //Wall.RotY = -45;
            Borders.AddPosition(new Vector3(0, 0, 0));
            Borders.ModelTransformations.Add(new ModelTransformations(0, -90, 0, 1, new Vector3(MaxX - 2, 0, 0)));
            Borders.ModelTransformations.Add(new ModelTransformations(0, -180, 0, 1, new Vector3(MaxX - 2, 0, MaxZ - 2)));
            Borders.ModelTransformations.Add(new ModelTransformations(0, -270, 0, 1, new Vector3(0, 0, MaxZ - 2)));


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
            Trees.AddToCollisionQueue(collisionManager);
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

        public void DrawMap(ShaderProgram program, ShaderProgram normalMapping)
        {
            TerrainInstanced.Draw(program);
            /*
            Trees.Draw(program);
            TreeLeaves.Draw(program);
            //program.AttachModelMatrix(Wall.GetModelMatrix());
            //Wall.Draw();
            Borders.Draw(program);
            */
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


            //Wall = new SceneObject(new Mesh(model, new Texture2D(FilePaths.TextureBrickWall)));
            Borders = new InstancedSceneObject(new Mesh(model, new Texture2D(FilePaths.TextureBrickWall)));
        }
    }
}
