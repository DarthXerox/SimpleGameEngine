using System;
using OpenTK.Graphics.OpenGL4;
using SimpleEngine.Utils;
using SimpleEngine.Data;
using System.Drawing;

namespace SimpleEngine.WorldObjects
{
    public class Mesh : IDisposable
	{
		public ObjModel Model { protected set; get; }
		public Texture2D TextureColor { protected set; get; }
		public Material Material { protected set; get; }
		public int ShaderAttribVertices { protected set; get; } = 0;
		public int ShaderAttribTexCoords { protected set; get; } = 1;
		public int ShaderAttribNormals { protected set; get; } = 2;
		public int ShaderTextureSampler { protected set; get; } = 0;

		protected int vaoMesh;
		protected int vboVertices;
		protected int vboTextureCoords;
		protected int vboNormals;
		protected int eboIndices;

		public Mesh(ObjModel model, string textureFile, string materialFile)
		{
			Model = model;
			TextureColor = new Texture2D(textureFile);
            Material = MtlParser.ParseMtl(materialFile)[0];
			InitBasicVao();
			InitIndices();
		}

		public Mesh(ObjModel model, Bitmap colTexture, Material material)
        {
			Model = model;
			TextureColor = new Texture2D(colTexture);
            Material = material;
			InitBasicVao();
			InitIndices();
		}

		protected Mesh() { }
		
		protected virtual void InitBasicVao()
		{
			GL.CreateBuffers(1, out vboVertices);
			GL.NamedBufferStorage(vboVertices, Model.VerticesFloat.Length * sizeof(float), Model.VerticesFloat, 0);

			GL.CreateBuffers(1, out vboTextureCoords);
			GL.NamedBufferStorage(vboTextureCoords, Model.TextureCoordinatesFloat.Length * sizeof(float), Model.TextureCoordinatesFloat, 0);

			GL.CreateBuffers(1, out vboNormals);
			GL.NamedBufferStorage(vboNormals, Model.NormalsFloat.Length * sizeof(float), Model.NormalsFloat, 0);

			GL.CreateVertexArrays(1, out vaoMesh);
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
		}

		private void InitIndices()
		{
			GL.CreateBuffers(1, out eboIndices);
			GL.NamedBufferStorage(eboIndices, Model.Indices.Count * sizeof(uint), Model.Indices.ToArray(), 0);
			GL.VertexArrayElementBuffer(vaoMesh, eboIndices);
		}

		/// <summary>
		/// The program must have beend called to use
		/// </summary>
		/// <param name="lightsProgram"></param>
		public virtual void Draw(LightsProgram lightsProgram)
		{
			lightsProgram.Use();
			GL.BindVertexArray(vaoMesh);
			TextureColor.Use(ShaderTextureSampler);
			lightsProgram.AttachMaterial(Material);
			GL.DrawElements(PrimitiveType.Triangles, Model.Indices.Count, DrawElementsType.UnsignedInt, 0);
		}

		public virtual void Dispose()
		{
			GL.DeleteVertexArray(vaoMesh);
			GL.DeleteBuffer(vboVertices);
			GL.DeleteBuffer(vboTextureCoords);
			GL.DeleteBuffer(vboNormals);
			GL.DeleteBuffer(eboIndices);
			TextureColor.Dispose();
		}
	}
}
