using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
//using AssimpNet;
using Assimp;
using System.ComponentModel;
using ObjLoader.Loader.Data.VertexData;

namespace OpenGL_in_CSharp.Utils
{
    public class Mesh : IDisposable
    {
        public ObjModel Model { get; set; }
        public Texture2D Texture { get; set; }

        public int ShaderAttribVertices { get; }
        public int ShaderAttribTexCoords { get; }
        public int ShaderAttribNormals { get; }
        public int ShaderTextureSampler { get; }

        private int vaoMesh;
        private int vboVertices;
        private int vboTextureCoords;
        private int vboNormals;
        private int eboIndices;


		public Mesh(ObjModel model, Texture2D texture, int shaderAttribVertices,
			int shaderAttribTexCoords, int shaderAttribNormals, int shaderTextureSampler)
		{
			ShaderAttribVertices = shaderAttribVertices;
			ShaderAttribTexCoords = shaderAttribTexCoords;
			ShaderAttribNormals = shaderAttribNormals;
			ShaderTextureSampler = shaderTextureSampler;
			Texture = texture;

			Model = model;
			InitVaoMesh();
		}

		public Mesh(string objFileName, Texture2D texture, int shaderAttribVertices,
			int shaderAttribTexCoords, int shaderAttribNormals, int shaderTextureSampler)
		{
			ShaderAttribVertices = shaderAttribVertices;
			ShaderAttribTexCoords = shaderAttribTexCoords;
			ShaderAttribNormals = shaderAttribNormals;
			ShaderTextureSampler = shaderTextureSampler;
			Texture = texture;

			Model = ObjParser.ParseObjFile(objFileName);
			InitVaoMesh();
		}

		public Mesh(string objFileName, string textureFileName, int shaderAttribVertices,
		   int shaderAttribTexCoords, int shaderAttribNormals, int shaderTextureSampler)
			: this(objFileName, new Texture2D(textureFileName), shaderAttribVertices, shaderAttribTexCoords,
				  shaderAttribNormals, shaderTextureSampler)
		{ 
		}

		public Mesh(string objFileName, string textureFileName)
			: this(objFileName, textureFileName, 0, 1, 2, 0) { }


		public Mesh(string objFileName, Texture2D texture)
			: this(objFileName, texture, 0, 1, 2, 0) { }

		public Mesh(ObjModel model, Texture2D texture)
			: this(model, texture, 0, 1, 2, 0) { }

		private void InitVaoMesh()
		{
			GL.GenBuffers(1, out vboVertices);
			GL.NamedBufferStorage(vboVertices, Model.VerticesFloat.Length * sizeof(float), Model.VerticesFloat, 0);

			GL.GenBuffers(1, out vboTextureCoords);
			GL.NamedBufferStorage(vboTextureCoords, Model.TextureCoordinatesFloat.Length * sizeof(float), Model.TextureCoordinatesFloat, 0);

			GL.GenBuffers(1, out vboNormals);
			GL.NamedBufferStorage(vboNormals, Model.NormalsFloat.Length * sizeof(float), Model.NormalsFloat, 0);

			GL.GenVertexArrays(1, out vaoMesh);
			GL.EnableVertexArrayAttrib(vaoMesh, ShaderAttribVertices);
			GL.VertexArrayVertexBuffer(vaoMesh, ShaderAttribVertices, vboVertices, IntPtr.Zero, 3 * sizeof(float));
			GL.VertexArrayAttribFormat(vaoMesh, ShaderAttribVertices, 3, VertexAttribType.Float, true, 0);
			GL.VertexArrayAttribBinding(vaoMesh, ShaderAttribVertices, ShaderAttribVertices);

			GL.EnableVertexArrayAttrib(vaoMesh, ShaderAttribTexCoords);
			GL.VertexArrayVertexBuffer(vaoMesh, ShaderAttribTexCoords, vboTextureCoords, IntPtr.Zero, 2 * sizeof(float));
			GL.VertexArrayAttribFormat(vaoMesh, ShaderAttribTexCoords, 2, VertexAttribType.Float, true, 0);
			GL.VertexArrayAttribBinding(vaoMesh, ShaderAttribTexCoords, ShaderAttribTexCoords);

			GL.EnableVertexArrayAttrib(vaoMesh, ShaderAttribNormals);
			GL.VertexArrayVertexBuffer(vaoMesh, ShaderAttribNormals, vboNormals, IntPtr.Zero, 3 * sizeof(float));
			GL.VertexArrayAttribFormat(vaoMesh, ShaderAttribNormals, 3, VertexAttribType.Float, true, 0);
			GL.VertexArrayAttribBinding(vaoMesh, ShaderAttribNormals, ShaderAttribNormals);

			GL.GenBuffers(1, out eboIndices);
			//GL.BindBuffer(BufferTarget.ElementArrayBuffer, eboIndices);
			GL.NamedBufferStorage(eboIndices, Model.Indices.Count * sizeof(int), Model.Indices.ToArray(), 0);
			GL.VertexArrayElementBuffer(vaoMesh, eboIndices);
		}

		public void Draw()
		{
			GL.BindVertexArray(vaoMesh);
			Texture.Use(ShaderTextureSampler);
			GL.DrawElements(OpenTK.Graphics.OpenGL4.PrimitiveType.Triangles, Model.Indices.Count, DrawElementsType.UnsignedInt, 0);
		}

		public void Dispose()
		{
			GL.DeleteVertexArray(vaoMesh);

			GL.DeleteBuffer(vboVertices);
			GL.DeleteBuffer(vboTextureCoords);
			GL.DeleteBuffer(vboNormals);
			GL.DeleteBuffer(eboIndices);

			Texture.Dispose();
		}
	}

	public class BumpMesh : Mesh
	{
		public Texture2D NormalMap { private set; get; }

		public BumpMesh(string obj, string texCol, string texBump) : base(obj, texCol)
		{
			NormalMap = new Texture2D(texBump);
		}


	}

	public class AssimpMesh
	{

		ObjModel Model = new ObjModel();

		Texture2D TextureColor { set; get; }
		Texture2D TextureNormal { set; get; }


		private int vaoMesh;
		private int vboVertices;
		private int vboTextureCoords;
		private int vboNormals;
		private int vboTangents;
		private int vboBiTangents;

		public int ShaderAttribVertices { get; } = 0;
		public int ShaderAttribTexCoords { get; } = 1;
		public int ShaderAttribNormals { get; } = 2;
		public int ShaderTextureSampler { get; } = 0;
		public int ShaderTextureSampler2 { get; } = 1;


		public int ShaderAttribTangents { get; } = 13;
		public int ShaderAttribBiTangents { get; } = 14;

		public Scene Scene { get; }

		public AssimpMesh(string objFile, string texCol, string texNormal)
		{
			AssimpContext Importer = new AssimpContext();
			Scene = Importer.ImportFile(objFile, PostProcessSteps.CalculateTangentSpace | PostProcessSteps.GenerateSmoothNormals
				//| PostProcessSteps.FlipUVs
				);

			if (!Scene.HasMeshes)
			{
				throw new InvalidEnumArgumentException("No meshes found!");
			}

			if (Scene.MeshCount > 1)
			{
				throw new InvalidEnumArgumentException("Found multiple meshes!");
			}

			/*
			HelpPrinter.PrintList(Scene.Meshes[0].Vertices);
			HelpPrinter.PrintList(Scene.Meshes[0].TextureCoordinateChannels[0]);

			HelpPrinter.PrintList(Scene.Meshes[0].Normals);

			HelpPrinter.PrintArray(Scene.Meshes[0].GetIndices());
			HelpPrinter.PrintList(Scene.Meshes[0].Tangents);

			Console.WriteLine("FAce count");
			Console.WriteLine(Scene.Meshes[0].Faces.Count);
			HelpPrinter.PrintList(Scene.Meshes[0].Faces);
			*/

			//HelpPrinter.PrintList(Scene.Meshes[0].Tangents);

			TextureColor = new Texture2D(texCol);
			TextureNormal = new Texture2D(texNormal);

			Model.VerticesFloat = new float[Scene.Meshes[0].VertexCount * 3];
			Model.TextureCoordinatesFloat = new float[Scene.Meshes[0].VertexCount * 2];
			Model.NormalsFloat = new float[Scene.Meshes[0].VertexCount * 3];
			Model.Tangents = new float[Scene.Meshes[0].Tangents.Count * 3];
			Model.BiTangents = new float[Scene.Meshes[0].BiTangents.Count * 3];


			for (int i = 0; i < Scene.Meshes[0].VertexCount; ++i)
			{
				Model.VerticesFloat[3 * i] = Scene.Meshes[0].Vertices[i].X;
				Model.VerticesFloat[3 * i + 1] = Scene.Meshes[0].Vertices[i].Y;
				Model.VerticesFloat[3 * i + 2] = Scene.Meshes[0].Vertices[i].Z;

				Model.NormalsFloat[3 * i] = Scene.Meshes[0].Normals[i].X;
				Model.NormalsFloat[3 * i + 1] = Scene.Meshes[0].Normals[i].Y;
				Model.NormalsFloat[3 * i + 2] = Scene.Meshes[0].Normals[i].Z;

				Model.TextureCoordinatesFloat[2 * i] = Scene.Meshes[0].TextureCoordinateChannels[0][i].X;
				Model.TextureCoordinatesFloat[2 * i + 1] = Scene.Meshes[0].TextureCoordinateChannels[0][i].Y;

				Model.Tangents[3 * i] = Scene.Meshes[0].Tangents[i].X;
				Model.Tangents[3 * i + 1] = Scene.Meshes[0].Tangents[i].Y;
				Model.Tangents[3 * i + 2] = Scene.Meshes[0].Tangents[i].Z;

				Model.BiTangents[3 * i] = Scene.Meshes[0].BiTangents[i].X;
				Model.BiTangents[3 * i + 1] = Scene.Meshes[0].BiTangents[i].Y;
				Model.BiTangents[3 * i + 2] = Scene.Meshes[0].BiTangents[i].Z;
			}
			//HelpPrinter.PrintArray(Model.Tangents);

			InitVaoMesh();
		}
		private void InitVaoMesh()
		{
			GL.GenBuffers(1, out vboVertices);
			GL.NamedBufferStorage(vboVertices, Model.VerticesFloat.Length * sizeof(float), Model.VerticesFloat, 0);

			GL.GenBuffers(1, out vboTextureCoords);
			GL.NamedBufferStorage(vboTextureCoords, Model.TextureCoordinatesFloat.Length * sizeof(float), Model.TextureCoordinatesFloat, 0);

			GL.GenBuffers(1, out vboNormals);
			GL.NamedBufferStorage(vboNormals, Model.NormalsFloat.Length * sizeof(float), Model.NormalsFloat, 0);

			GL.GenBuffers(1, out vboTangents);
			GL.NamedBufferStorage(vboTangents, Model.Tangents.Length * sizeof(float), Model.Tangents, 0);

			GL.GenBuffers(1, out vboBiTangents);
			GL.NamedBufferStorage(vboBiTangents, Model.BiTangents.Length * sizeof(float), Model.BiTangents, 0);

			GL.GenVertexArrays(1, out vaoMesh);
			GL.EnableVertexArrayAttrib(vaoMesh, ShaderAttribVertices);
			GL.VertexArrayVertexBuffer(vaoMesh, ShaderAttribVertices, vboVertices, IntPtr.Zero, 3 * sizeof(float));
			GL.VertexArrayAttribFormat(vaoMesh, ShaderAttribVertices, 3, VertexAttribType.Float, false, 0);
			GL.VertexArrayAttribBinding(vaoMesh, ShaderAttribVertices, ShaderAttribVertices);

			GL.EnableVertexArrayAttrib(vaoMesh, ShaderAttribTexCoords);
			GL.VertexArrayVertexBuffer(vaoMesh, ShaderAttribTexCoords, vboTextureCoords, IntPtr.Zero, 2 * sizeof(float));
			GL.VertexArrayAttribFormat(vaoMesh, ShaderAttribTexCoords, 2, VertexAttribType.Float, false, 0);
			GL.VertexArrayAttribBinding(vaoMesh, ShaderAttribTexCoords, ShaderAttribTexCoords);

			GL.EnableVertexArrayAttrib(vaoMesh, ShaderAttribNormals);
			GL.VertexArrayVertexBuffer(vaoMesh, ShaderAttribNormals, vboNormals, IntPtr.Zero, 3 * sizeof(float));
			GL.VertexArrayAttribFormat(vaoMesh, ShaderAttribNormals, 3, VertexAttribType.Float, false, 0);
			GL.VertexArrayAttribBinding(vaoMesh, ShaderAttribNormals, ShaderAttribNormals);

			GL.EnableVertexArrayAttrib(vaoMesh, ShaderAttribTangents);
			GL.VertexArrayVertexBuffer(vaoMesh, ShaderAttribTangents, vboTangents, IntPtr.Zero, 3 * sizeof(float));
			GL.VertexArrayAttribFormat(vaoMesh, ShaderAttribTangents, 3, VertexAttribType.Float, false, 0);
			GL.VertexArrayAttribBinding(vaoMesh, ShaderAttribTangents, ShaderAttribTangents);

			GL.EnableVertexArrayAttrib(vaoMesh, ShaderAttribBiTangents);
			GL.VertexArrayVertexBuffer(vaoMesh, ShaderAttribBiTangents, vboBiTangents, IntPtr.Zero, 3 * sizeof(float));
			GL.VertexArrayAttribFormat(vaoMesh, ShaderAttribBiTangents, 3, VertexAttribType.Float, false, 0);
			GL.VertexArrayAttribBinding(vaoMesh, ShaderAttribBiTangents, ShaderAttribBiTangents);
		}


		public void Draw()
		{
			GL.BindVertexArray(vaoMesh);
			TextureColor.Use(ShaderTextureSampler);
			TextureNormal.Use(ShaderTextureSampler2);
			GL.DrawArrays(OpenTK.Graphics.OpenGL4.PrimitiveType.Triangles, 0, Scene.Meshes[0].VertexCount);
		}	
		
	}
}
