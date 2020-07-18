using System;
using OpenTK.Graphics.OpenGL4;
using Assimp;
using System.Threading.Tasks;
using System.Drawing;
using SimpleEngine.Utils;
using SimpleEngine.Data;

namespace SimpleEngine.WorldObjects
{
    /*
     * Normal mapping technique loads normals from a special texture(normal/bump texture)
     * but these textures are in so called tangent space(space relative to the texture)
     * that's why they need to be transferred to world space, for this we need to retrieve 
     * tangents and bitangents for each vertex of the mesh, for this we need a new file loader - Assimp
	 */

    /// <summary>
    /// To basic mesh adds data necessary to enable normal mapping technique to be applied to a 3D object
    /// </summary>
    public class NormalMappingMesh : Mesh
	{
		private int vboTangents;
		private int vboBiTangents;
        Texture2D TextureNormal { get; }
		public int ShaderTextureSampler2 { get; } = 1;
		public int ShaderAttribTangents { get; } = 13;
		public int ShaderAttribBiTangents { get; } = 14;

		public NormalMappingMesh(ObjModel model, Bitmap colTexture, Bitmap normalTexture, Data.Material material)
        {
			Model = model;
			TextureColor = new Texture2D(colTexture);
			TextureNormal = new Texture2D(normalTexture);
            Material = material;
			InitBasicVao();
        }


		/// <summary>
		/// Apart from basic vao initialization we load tangents and bitangents too
		/// </summary>
		protected override void InitBasicVao()
		{
			base.InitBasicVao();
			GL.CreateBuffers(1, out vboTangents);
			GL.NamedBufferStorage(vboTangents, Model.Tangents.Length * sizeof(float), Model.Tangents, 0);

			GL.CreateBuffers(1, out vboBiTangents);
			GL.NamedBufferStorage(vboBiTangents, Model.BiTangents.Length * sizeof(float), Model.BiTangents, 0);

			GL.EnableVertexArrayAttrib(vaoMesh, ShaderAttribTangents);
			GL.VertexArrayVertexBuffer(vaoMesh, ShaderAttribTangents, vboTangents, IntPtr.Zero, 3 * sizeof(float));
			GL.VertexArrayAttribFormat(vaoMesh, ShaderAttribTangents, 3, VertexAttribType.Float, false, 0);
			GL.VertexArrayAttribBinding(vaoMesh, ShaderAttribTangents, ShaderAttribTangents);

			GL.EnableVertexArrayAttrib(vaoMesh, ShaderAttribBiTangents);
			GL.VertexArrayVertexBuffer(vaoMesh, ShaderAttribBiTangents, vboBiTangents, IntPtr.Zero, 3 * sizeof(float));
			GL.VertexArrayAttribFormat(vaoMesh, ShaderAttribBiTangents, 3, VertexAttribType.Float, false, 0);
			GL.VertexArrayAttribBinding(vaoMesh, ShaderAttribBiTangents, ShaderAttribBiTangents);
		}

		public override void Draw(LightsProgram lightsProgram)
		{
			GL.BindVertexArray(vaoMesh);
			TextureColor.Use(ShaderTextureSampler);
			TextureNormal.Use(ShaderTextureSampler2);
			lightsProgram.AttachMaterial(Material);
			GL.DrawArrays(OpenTK.Graphics.OpenGL4.PrimitiveType.Triangles, 0, Model.VerticesFloat.Length / 3);
		}

		public override void Dispose()
		{
			base.Dispose();
			GL.DeleteBuffer(vboTangents);
			GL.DeleteBuffer(vboBiTangents);
			TextureNormal.Dispose();
		}
	}
}
