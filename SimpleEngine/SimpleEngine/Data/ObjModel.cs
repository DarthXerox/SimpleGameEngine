using System;
using System.Collections.Generic;
using Assimp;
using OpenTK;

namespace SimpleEngine.Data
{
    /// <summary>
    /// Simple class to keep all relevant data related to an .obj 3D model
    /// </summary>
    public class ObjModel
    {
        public List<Vector3> Vertices = new List<Vector3>();
        public List<Vector2> TextureCoordinates = new List<Vector2>();
        public List<Vector3> Normals = new List<Vector3>();

        public float MaxX { set; get; }
        public float MaxY { set; get; }
        public float MaxZ { set; get; }

        public float MinX { set; get; }
        public float MinY { set; get; }
        public float MinZ { set; get; }

        public List<uint> Indices { set; get; } = new List<uint>();

        public float[] VerticesFloat;
        public float[] TextureCoordinatesFloat;
        public float[] NormalsFloat;
        public float[] Tangents;
        public float[] BiTangents;

        public static ObjModel LoadWithTangents(string objFile)
        {
            Scene scene = new AssimpContext().ImportFile(objFile, PostProcessSteps.GenerateSmoothNormals
                | PostProcessSteps.CalculateTangentSpace
                | PostProcessSteps.Triangulate);

            if (!scene.HasMeshes)
            {
                throw new MissingFieldException("No meshes found!");
            }
            // this whole class is only made for single mesh files
            if (scene.MeshCount > 1)
            {
                throw new MissingFieldException("Found multiple meshes!");
            }

            var model = InitFromScene(scene);
            model.DetermineBorders(scene.Meshes[0].Vertices);

            return model;
        }

        private static ObjModel InitFromScene(Scene scene)
        {
            ObjModel model = new ObjModel
            {
                VerticesFloat = new float[scene.Meshes[0].VertexCount * 3],
                TextureCoordinatesFloat = new float[scene.Meshes[0].VertexCount * 2],
                NormalsFloat = new float[scene.Meshes[0].VertexCount * 3],
                Tangents = new float[scene.Meshes[0].Tangents.Count * 3],
                BiTangents = new float[scene.Meshes[0].BiTangents.Count * 3]
            };

            // fills in the arrays
            for (int i = 0; i < scene.Meshes[0].VertexCount; ++i)
            {
                model.VerticesFloat[3 * i] = scene.Meshes[0].Vertices[i].X;
                model.VerticesFloat[3 * i + 1] = scene.Meshes[0].Vertices[i].Y;
                model.VerticesFloat[3 * i + 2] = scene.Meshes[0].Vertices[i].Z;

                model.NormalsFloat[3 * i] = scene.Meshes[0].Normals[i].X;
                model.NormalsFloat[3 * i + 1] = scene.Meshes[0].Normals[i].Y;
                model.NormalsFloat[3 * i + 2] = scene.Meshes[0].Normals[i].Z;

                model.TextureCoordinatesFloat[2 * i] = scene.Meshes[0].TextureCoordinateChannels[0][i].X;
                model.TextureCoordinatesFloat[2 * i + 1] = scene.Meshes[0].TextureCoordinateChannels[0][i].Y;

                model.Tangents[3 * i] = scene.Meshes[0].Tangents[i].X;
                model.Tangents[3 * i + 1] = scene.Meshes[0].Tangents[i].Y;
                model.Tangents[3 * i + 2] = scene.Meshes[0].Tangents[i].Z;

                model.BiTangents[3 * i] = scene.Meshes[0].BiTangents[i].X;
                model.BiTangents[3 * i + 1] = scene.Meshes[0].BiTangents[i].Y;
                model.BiTangents[3 * i + 2] = scene.Meshes[0].BiTangents[i].Z;
            }

            return model;
        }

        private void DetermineBorders(List<Vector3D> vertices)
        {
            foreach (var vec in vertices)
            {
                if (vec.X > MaxX)
                {
                    MaxX = vec.X;
                }
                if (vec.Y > MaxY)
                {
                    MaxY = vec.Y;
                }
                if (vec.Z > MaxZ)
                {
                    MaxZ = vec.Z;
                }
                if (vec.X < MinX)
                {
                    MinX = vec.X;
                }
                if (vec.Y < MinY)
                {
                    MinY = vec.Y;
                }
                if (vec.Z < MinZ)
                {
                    MinZ = vec.Z;
                }
            }
        }
    }
}
