﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

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
			GL.DrawElements(PrimitiveType.Triangles, Model.Indices.Count, DrawElementsType.UnsignedInt, 0);
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
