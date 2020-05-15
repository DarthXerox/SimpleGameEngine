using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenGL_in_CSharp.Utils;
using OpenTK.Graphics.OpenGL4;

namespace OpenGL_in_CSharp
{
	public class GameObject : IDisposable
	{
		public Texture2D Texture { get; }

		public int ShaderAttribVertices { get; }
		public int ShaderAttribTexCoords { get; }
		public int ShaderAttribNormals { get; }
		public int ShaderTextureSampler { get; }

		private int vaoMesh;
		private int vboVertices;
		private int vboTextureCoords;
		private int vboNormals;
		private int eboIndices;

		public ObjModel Mesh { get; }
		
		public GameObject(string objFileName, string textureFileName, int shaderAttribVertices, 
			int shaderAttribTexCoords, int shaderAttribNormals, int shaderTextureSampler)
		{
			ShaderAttribVertices = shaderAttribVertices;
			ShaderAttribTexCoords = shaderAttribTexCoords;
			ShaderAttribNormals = shaderAttribNormals;
			ShaderTextureSampler = shaderTextureSampler;
			Texture = new Texture2D(textureFileName);

			Mesh = ObjParser.ParseObjFile(objFileName);
			InitVaoMesh();
		}

		public GameObject(string objFileName, string textureFileName)
			: this(objFileName, textureFileName, 0, 1, 2, 0) { }

		private void InitVaoMesh()
		{
			GL.GenBuffers(1, out vboVertices);
			GL.NamedBufferStorage(vboVertices, Mesh.VerticesFloat.Length * sizeof(float), Mesh.VerticesFloat, 0);

			GL.GenBuffers(1, out vboTextureCoords);
			GL.NamedBufferStorage(vboTextureCoords, Mesh.TextureCoordinatesFloat.Length * sizeof(float), Mesh.TextureCoordinatesFloat, 0);

			GL.GenBuffers(1, out vboNormals);
			GL.NamedBufferStorage(vboNormals, Mesh.NormalsFloat.Length * sizeof(float), Mesh.NormalsFloat, 0);

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

			GL.GenBuffers(1, out eboIndices);
			//GL.BindBuffer(BufferTarget.ElementArrayBuffer, eboIndices);

			GL.NamedBufferStorage(eboIndices, Mesh.Indices.Count * sizeof(int), Mesh.Indices.ToArray(), 0);

			GL.VertexArrayElementBuffer(vaoMesh, eboIndices);
		}


		public void Draw()
		{
			GL.BindVertexArray(vaoMesh);
			Texture.Use(ShaderTextureSampler);
			GL.DrawElements(PrimitiveType.Triangles, Mesh.Indices.Count, DrawElementsType.UnsignedInt, 0);
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

}