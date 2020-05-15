﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenTK;

namespace OpenGL_in_CSharp
{
    public class ObjModel
    {
        public List<Vector3> Vertices = new List<Vector3>();
        public List<Vector2> TextureCoordinates = new List<Vector2>();
        public List<Vector3> Normals = new List<Vector3>();

        public List<int> Indices = new List<int>();

        public float[] VerticesFloat;
        public float[] TextureCoordinatesFloat;
        public float[] NormalsFloat;
    }

    public static class ObjParser
    {
        public static ObjModel ParseObjFile(string path)
        {
            bool parsedAllCoords = false;
            ObjModel result = new ObjModel();

            foreach (string line in File.ReadAllLines(path))
            {
                string[] parts = line.Split(' ');
                switch(parts[0])
                {
                    case "v":
                        ParseVector3(ref result.Vertices, ref parts);
                        break;
                    case "vt":
                        ParseTextureCoords(result, ref parts);
                        break;
                    case "vn":
                        ParseVector3(ref result.Normals, ref parts);
                        break;
                    case "f":
                        if (!parsedAllCoords)
                        {
                            result.VerticesFloat = new float[result.Vertices.Count * 3]; // 3 flaots for each
                            result.TextureCoordinatesFloat = new float[result.Vertices.Count * 2]; // 2 floats for each
                            result.NormalsFloat = new float[result.Vertices.Count * 3]; //3 for each
                            parsedAllCoords = true;
                        }
                        ParseFaces(result, ref parts);
                        break;
                }
            }

            /*
            Console.Write("Indices: ");
            foreach (var x in result.Indices)
            {
                Console.Write(x + " ");
            }
            Console.WriteLine();

            Console.Write("Len: " + result.VerticesFloat.Length + " ");
            Console.Write("Vertices: ");
            foreach (var x in result.VerticesFloat)
            {
                Console.Write(x + " ");
            }
            Console.WriteLine();

            Console.Write("Len:" + result.TextureCoordinatesFloat.Length + " ");
            Console.Write("Textures: ");
            foreach (var x in result.TextureCoordinatesFloat)
            {
                Console.Write(x + " ");
            }
            Console.WriteLine();
            */

            return result;
        }

        private static void ParseVector3(ref List<Vector3> lst, ref string[] parts)
        {
            lst.Add(new Vector3(
                float.Parse(parts[1]),
                float.Parse(parts[2]),
                float.Parse(parts[3])
                ));
        }




        private static void ParseTextureCoords(ObjModel model, ref string[] parts)
        {
            model.TextureCoordinates.Add(new Vector2(
                float.Parse(parts[1]),
                float.Parse(parts[2])
                ));
        }

        /*
        private static void ParseVertex(ObjModel model, ref string[] parts)
        {
            model.Vertices.Add(new Vector3(
                float.Parse(parts[1]),
                float.Parse(parts[2]),
                float.Parse(parts[3])
                ));
        }

        private static void ParseNormals(ObjModel model, ref string[] parts)
        {
            model.Normals.Add(new Vector3(
                float.Parse(parts[1]),
                float.Parse(parts[2]),
                float.Parse(parts[3])
                ));
        }
        */

        /// <summary>
        /// The face states which vertices (and their textures, normals) form a certain geometry primitive
        /// The primitive is most often a triangle, this parser can only parse triangles so far
        /// 
        /// This is how a face of a triangle (3 vertices) looks like in an .obj file:
        /// "f v/t/n v/t/n v/t/n"
        /// v - vertex index, t - texture index, n - normal index
        /// </summary>
        private static void ParseFaces(ObjModel model, ref string[] parts)
        {
            for (int i = 1; i < 4; i++)
            {
                string[] indices = parts[i].Split('/');

                // Blender starts indexing from 1 (why though?)
                int vertIndex = int.Parse(indices[0]) - 1;
                int texIndex = int.Parse(indices[1]) - 1;
                int normalIndex = int.Parse(indices[2]) - 1;
                model.Indices.Add(vertIndex);

                model.VerticesFloat[vertIndex * 3] = model.Vertices[vertIndex].X;
                model.VerticesFloat[vertIndex * 3 + 1] = model.Vertices[vertIndex].Y;
                model.VerticesFloat[vertIndex * 3 + 2] = model.Vertices[vertIndex].Z;

                model.TextureCoordinatesFloat[vertIndex * 2] = model.TextureCoordinates[texIndex].X;
                model.TextureCoordinatesFloat[vertIndex * 2 + 1] = model.TextureCoordinates[texIndex].Y;

                model.NormalsFloat[vertIndex * 3] = model.Normals[normalIndex].X;
                model.NormalsFloat[vertIndex * 3 + 1] = model.Normals[normalIndex].Y;
                model.NormalsFloat[vertIndex * 3 + 2] = model.Normals[normalIndex].Z;
            }
        }
    }
}
 