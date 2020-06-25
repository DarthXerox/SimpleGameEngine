using System;
using OpenTK.Graphics.OpenGL4;
using Assimp;
using Assimp.Unmanaged;

namespace OpenGL_in_CSharp.Utils
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

		public Mesh(string objFile, string textureFile, string materialFile, int shaderAttribVertices = 0,
		   int shaderAttribTexCoords = 1, int shaderAttribNormals = 2, int shaderTextureSampler = 0)
			: this(objFile, textureFile, MtlParser.ParseMtl(materialFile)[0], shaderAttribVertices, 
				  shaderAttribTexCoords, shaderAttribNormals, shaderTextureSampler)
		{
		}

		public Mesh(string objFile, string textureFile, Material material, int shaderAttribVertices = 0,
		   int shaderAttribTexCoords = 1, int shaderAttribNormals = 2, int shaderTextureSampler = 0)
		{
			ShaderAttribVertices = shaderAttribVertices;
			ShaderAttribTexCoords = shaderAttribTexCoords;
			ShaderAttribNormals = shaderAttribNormals;
			ShaderTextureSampler = shaderTextureSampler;

			TextureColor = new Texture2D(textureFile);
			Model = ObjParser.ParseObjFile(objFile);
			Material = material;
			InitBasicVao();
			InitIndices();
		}

		public Mesh(ObjModel model, string textureFile, string materialFile)
		{
			Model = model;
			TextureColor = new Texture2D(textureFile);
			Material = MtlParser.ParseMtl(materialFile)[0];
			InitBasicVao();
			InitIndices();
		}

		protected Mesh() { }
		/*
		public Mesh(string objFile, string textureFile, int shaderAttribVertices = 0,
		   int shaderAttribTexCoords = 1, int shaderAttribNormals = 2, int shaderTextureSampler = 0)
			: this(objFile, new Texture2D(textureFile), shaderAttribVertices, shaderAttribTexCoords,
				  shaderAttribNormals, shaderTextureSampler)
		{
		}

		

	
		public Mesh(string objFile, string textureFile)
			: this(objFile, textureFile, 0, 1, 2, 0) { }


		public Mesh(ObjModel model, string textureFile)
			: this(model, new Texture2D(textureFile), 0, 1, 2, 0) { }

		protected Mesh(ObjModel model, Texture2D texture, int shaderAttribVertices,
			int shaderAttribTexCoords, int shaderAttribNormals, int shaderTextureSampler)
		{
			ShaderAttribVertices = shaderAttribVertices;
			ShaderAttribTexCoords = shaderAttribTexCoords;
			ShaderAttribNormals = shaderAttribNormals;
			ShaderTextureSampler = shaderTextureSampler;
			TextureColor = texture;

			Model = model;
			InitBasicVao();
			InitIndices();
		}

		protected Mesh(string objFile, Texture2D texture, int shaderAttribVertices,
			int shaderAttribTexCoords, int shaderAttribNormals, int shaderTextureSampler)
		{
			ShaderAttribVertices = shaderAttribVertices;
			ShaderAttribTexCoords = shaderAttribTexCoords;
			ShaderAttribNormals = shaderAttribNormals;
			ShaderTextureSampler = shaderTextureSampler;
			TextureColor = texture;

			Model = ObjParser.ParseObjFile(objFile);
			InitBasicVao();
			InitIndices();
		}

		protected Mesh(string texCol) 
		{
			TextureColor = new Texture2D(texCol);
		}
		*/
		protected virtual void InitBasicVao()
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
		}

		private void InitIndices()
		{
			GL.GenBuffers(1, out eboIndices);
			//GL.BindBuffer(BufferTarget.ElementArrayBuffer, eboIndices);
			GL.NamedBufferStorage(eboIndices, Model.Indices.Count * sizeof(int), Model.Indices.ToArray(), 0);
			GL.VertexArrayElementBuffer(vaoMesh, eboIndices);
		}

		public virtual void Draw(LightsProgram lightsProgram)
		{
			GL.BindVertexArray(vaoMesh);
			TextureColor.Use(ShaderTextureSampler);
			lightsProgram.AttachMaterial(Material);
			GL.DrawElements(OpenTK.Graphics.OpenGL4.PrimitiveType.Triangles, Model.Indices.Count, DrawElementsType.UnsignedInt, 0);
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

	/// <summary>
	/// Normal mapping technique loads normals from a special texture (normal/bump texture)
	/// but these textures are in so called tangent space (space relative to the texture) 
	/// that's why they need to be transferred to world space, for this we need to retrieve 
	/// tangents and bitangents for each vertex of the mesh, for this we need a new file loader - Assimp
	/// </summary>
	public class NormalMappingMesh : Mesh
	{
		Texture2D TextureNormal { get; }

		private int vboTangents;
		private int vboBiTangents;
		public int ShaderTextureSampler2 { get; } = 1;
		public int ShaderAttribTangents { get; } = 13;
		public int ShaderAttribBiTangents { get; } = 14;

		public Scene Scene { get; }


		public NormalMappingMesh(string objFile, string colorTextureFile, string materialFile, string normalTextureFile,
			int shaderAttribVertices = 0, int shaderAttribTexCoords = 1, int shaderAttribNormals = 2, int shaderTextureSampler = 0,
			int shaderAttribTangents = 13, int shaderAttribBiTangents = 14, int textureSampler2 = 1)
			: this(objFile, colorTextureFile, MtlParser.ParseMtl(materialFile)[0], normalTextureFile, shaderAttribVertices,
				  shaderAttribTexCoords, shaderAttribNormals, shaderTextureSampler, 
				  shaderAttribTangents, shaderAttribBiTangents, textureSampler2) { }

		public NormalMappingMesh(string objFile, string colorTextureFile, Material material, string normalTextureFile, 
			int shaderAttribVertices = 0, int shaderAttribTexCoords = 1, int shaderAttribNormals = 2, int shaderTextureSampler = 0,
			int shaderAttribTangents = 13, int shaderAttribBiTangents = 14, int textureSampler2 = 1)
			//: base(objFile, colorTextureFile, material, shaderAttribVertices, 
			//	  shaderAttribTexCoords, shaderAttribNormals, shaderTextureSampler)
		{
			ShaderAttribVertices = shaderAttribVertices;
			ShaderAttribTexCoords = shaderAttribTexCoords;
			ShaderAttribNormals = shaderAttribNormals;
			ShaderTextureSampler = shaderTextureSampler;
			ShaderTextureSampler2 = textureSampler2;
			ShaderAttribTangents = shaderAttribTangents;
			ShaderAttribBiTangents = shaderAttribBiTangents;
			TextureNormal = new Texture2D(normalTextureFile);
			TextureColor = new Texture2D(colorTextureFile);
			Material = material;

			Scene = new AssimpContext().ImportFile(objFile, PostProcessSteps.GenerateSmoothNormals
				| PostProcessSteps.CalculateTangentSpace
				//| PostProcessSteps.FlipUVs
				);

			if (!Scene.HasMeshes)
			{
				throw new MissingFieldException("No meshes found!");
			}
			// this whole class is only made for single mesh files
			if (Scene.MeshCount > 1)
			{
				throw new MissingFieldException("Found multiple meshes!");
			}

			/*
			var y = Scene.ToUnmanagedScene(Scene);
			var afterPostProcess = AssimpLibrary.Instance.ApplyPostProcessing(y, PostProcessSteps.CalculateTangentSpace);
			Scene = Scene.FromUnmanagedScene(afterPostProcess);
			*/

			InitObjModel();
			InitBasicVao();
			FindModelBorders();
		}

		/*
		public NormalMappingMesh(string objFile, string texCol, string texNormal) 
			: base(texCol)
		{
			Scene = new AssimpContext().ImportFile(objFile, PostProcessSteps.CalculateTangentSpace 
				| PostProcessSteps.GenerateSmoothNormals
				//| PostProcessSteps.FlipUVs
				);

			if (!Scene.HasMeshes)
			{
				throw new MissingFieldException("No meshes found!");
			}
			// this whole class is only made for single mesh files
			if (Scene.MeshCount > 1)
			{
				throw new MissingFieldException("Found multiple meshes!");
			}
			TextureNormal = new Texture2D(texNormal);
			InitObjModel();
			InitBasicVao();
			FindModelBorders();
		}
		*/

		private void InitObjModel()
		{
			Model = new ObjModel
			{
				VerticesFloat = new float[Scene.Meshes[0].VertexCount * 3],
				TextureCoordinatesFloat = new float[Scene.Meshes[0].VertexCount * 2],
				NormalsFloat = new float[Scene.Meshes[0].VertexCount * 3],
				Tangents = new float[Scene.Meshes[0].Tangents.Count * 3],
				BiTangents = new float[Scene.Meshes[0].BiTangents.Count * 3]
			};

			// fills in the arrays
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
		}

		protected override void InitBasicVao()
		{
			base.InitBasicVao();
			GL.GenBuffers(1, out vboTangents);
			GL.NamedBufferStorage(vboTangents, Model.Tangents.Length * sizeof(float), Model.Tangents, 0);

			GL.GenBuffers(1, out vboBiTangents);
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
			GL.DrawArrays(OpenTK.Graphics.OpenGL4.PrimitiveType.Triangles, 0, Scene.Meshes[0].VertexCount);
		}

		public override void Dispose()
		{
			base.Dispose();
			GL.DeleteBuffer(vboTangents);
			GL.DeleteBuffer(vboBiTangents);
			TextureNormal.Dispose();
		}

		private void FindModelBorders()
		{
			foreach (var vec in Scene.Meshes[0].Vertices)
			{
				if (vec.X > Model.MaxX)
				{
					Model.MaxX = vec.X;
				}
				if (vec.Y > Model.MaxY)
				{
					Model.MaxY = vec.Y;
				}
				if (vec.Z > Model.MaxZ)
				{
					Model.MaxZ = vec.Z;
				}
				///
				if (vec.X < Model.MinX)
				{
					Model.MinX = vec.X;
				}
				if (vec.Y < Model.MinY)
				{
					Model.MinY = vec.Y;
				}
				if (vec.Z < Model.MinZ)
				{
					Model.MinZ = vec.Z;
				}
			}
		}
	}
}
