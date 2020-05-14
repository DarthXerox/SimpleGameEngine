using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using ObjLoader;
using ObjLoader.Loader.Data.Elements;
using ObjLoader.Loader.Loaders;
using OpenGL_in_CSharp.Utils;
using OpenTK.Graphics.OpenGL4;

namespace OpenGL_in_CSharp
{


	public class GameObject : IDisposable
	{
		public LoadResult Data { get; }
		public Texture2D Texture { get; }

		public int ShaderAttribVertices { get; }
		public int ShaderAttribTexCoords { get; }
		public int ShaderAttribNormals { get; }

		private int vaoMesh;
		private int vboVertices;
		private int vboTextureCoords;
		private int vboNormals;
		private int eboFaces;

		private List<float> vertices = new List<float>();
		private List<float> texCoords = new List<float>();
		private List<float> normals = new List<float>();

		


		public GameObject(string objFileName, string textureFileName, int shaderAttribVertices, 
			int shaderAttribTexCoords, int shaderAttribNormals)
		{
			ShaderAttribVertices = shaderAttribVertices;
			ShaderAttribTexCoords = shaderAttribTexCoords;
			ShaderAttribNormals = shaderAttribNormals;
			Texture = new Texture2D(textureFileName);

			var objLoader = new ObjLoaderFactory().Create();
			var fileStream = new FileStream(objFileName, FileMode.Open);

			Data = objLoader.Load(fileStream);
			fileStream.Close();
			PrintCounts();
			CreateMesh();
		}

		public GameObject(string objFileName, string textureFileName) 
			: this(objFileName, textureFileName, 0, 1, 2)
		{
		}

		private void CreateMesh()
		{
			ProcessIndices();
			GL.GenVertexArrays(1, out vaoMesh);

			GL.GenBuffers(1, out vboVertices);
			GL.NamedBufferStorage(vboVertices, vertices.Count, vertices.ToArray(), 0);

			GL.GenBuffers(1, out vboTextureCoords);
			GL.NamedBufferStorage(vboTextureCoords, texCoords.Count, texCoords.ToArray(), 0);

			GL.GenBuffers(1, out vboNormals);
			GL.NamedBufferStorage(vboNormals, normals.Count, normals.ToArray(), 0);

			GL.EnableVertexArrayAttrib(vaoMesh, ShaderAttribVertices);
			GL.VertexArrayVertexBuffer(vaoMesh, ShaderAttribVertices, vboVertices, IntPtr.Zero, 3 * sizeof(float));
			GL.VertexArrayAttribFormat(vaoMesh, ShaderAttribVertices, 3, VertexAttribType.Float, true, 0);
			GL.VertexArrayAttribBinding(vaoMesh, ShaderAttribVertices, ShaderAttribVertices);

			GL.EnableVertexArrayAttrib(vaoMesh, ShaderAttribTexCoords);
			GL.VertexArrayVertexBuffer(vaoMesh, ShaderAttribTexCoords, vboTextureCoords, IntPtr.Zero, 2 * sizeof(float));
			GL.VertexArrayAttribFormat(vaoMesh, ShaderAttribTexCoords, 2, VertexAttribType.Float, false, 0);
			GL.VertexArrayAttribBinding(vaoMesh, ShaderAttribTexCoords, ShaderAttribTexCoords);

			GL.EnableVertexArrayAttrib(vaoMesh, ShaderAttribNormals);
			GL.VertexArrayVertexBuffer(vaoMesh, ShaderAttribNormals, vboNormals, IntPtr.Zero, 3 * sizeof(float));
			GL.VertexArrayAttribFormat(vaoMesh, ShaderAttribNormals, 3, VertexAttribType.Float, false, 0);
			GL.VertexArrayAttribBinding(vaoMesh, ShaderAttribNormals, ShaderAttribNormals);
		}


		private void ProcessIndices()
		{
			foreach (var group in Data.Groups) 
			{
				foreach (var face in group.Faces)
				{
					for (int i = 0; i < 3; i++)
					{
						vertices.Add(Data.Vertices[face[i].VertexIndex - 1].X);
						vertices.Add(Data.Vertices[face[i].VertexIndex - 1].Y);
						vertices.Add(Data.Vertices[face[i].VertexIndex - 1].Z);
					}

					for (int i = 0; i < 2; i++)
					{
						texCoords.Add(Data.Textures[face[i].TextureIndex - 1].X);
						texCoords.Add(1.0f - Data.Textures[face[i].TextureIndex - 1].Y);
					}

					for (int i = 0; i < 3; i++)
					{
						normals.Add(Data.Normals[face[i].NormalIndex - 1].X);
						normals.Add(Data.Normals[face[i].NormalIndex - 1].Y);
						normals.Add(Data.Normals[face[i].NormalIndex - 1].Z);
					}
				}
			}
		}



		public void Draw()
		{
			GL.BindVertexArray(vaoMesh);
			//Texture.Use();
			GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 6 * 2 * 3);
		}


		public void PrintCounts()
		{
			Console.WriteLine("Vertices Count: " + Data.Vertices.Count);
			Console.WriteLine("Textures  Count: " + Data.Textures.Count);
			Console.WriteLine("Normals  Count: " + Data.Normals.Count);
			Console.WriteLine("Faces  Count: " + Data.Groups.First().Faces.Count);
			Console.WriteLine("Materials   Count: " + Data.Materials.Count);
			Console.WriteLine("Groups name: ", Data.Groups.First().Name);

			foreach (var group in Data.Groups)
			{
				foreach (var face in group.Faces)
				{
					for (int i = 0; i < 3; i++)
					{
						Console.Write(Data.Vertices[face[i].VertexIndex - 1].X);
						Console.Write(" " + Data.Vertices[face[i].VertexIndex - 1].Y);
						Console.Write(" " + Data.Vertices[face[i].VertexIndex - 1].Z);
						Console.WriteLine();

					}
				}
			}

		}
		public void Dispose()
		{

		}
	}

}