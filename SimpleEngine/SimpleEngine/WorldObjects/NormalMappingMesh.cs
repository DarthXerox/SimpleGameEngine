using System;
using OpenTK.Graphics.OpenGL4;
using Assimp;
using System.Threading.Tasks;
using System.Drawing;
using SimpleEngine.Utils;
using SimpleEngine.Data;

namespace SimpleEngine.WorldObjects
{
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

        /*
		public NormalMappingMesh(string objFile, string colorTextureFile, string materialFile, string normalTextureFile,
			int shaderAttribVertices = 0, int shaderAttribTexCoords = 1, int shaderAttribNormals = 2, int shaderTextureSampler = 0,
			int shaderAttribTangents = 13, int shaderAttribBiTangents = 14, int textureSampler2 = 1)
			: this(objFile, colorTextureFile, materialFile, normalTextureFile, shaderAttribVertices,
				  shaderAttribTexCoords, shaderAttribNormals, shaderTextureSampler, 
				  shaderAttribTangents, shaderAttribBiTangents, textureSampler2) { }
		*/

		public NormalMappingMesh(ObjModel model, Bitmap colTexture, Bitmap normalTexture, Data.Material material)
        {
			Model = model;
			TextureColor = new Texture2D(colTexture);
			TextureNormal = new Texture2D(normalTexture);
            Material = material;
			InitBasicVao();
        }

        public NormalMappingMesh(string objFile, string colorTextureFile, string materialFile, string normalTextureFile,
            int shaderAttribVertices = 0, int shaderAttribTexCoords = 1, int shaderAttribNormals = 2, int shaderTextureSampler = 0,
			int shaderAttribTangents = 13, int shaderAttribBiTangents = 14, int textureSampler2 = 1)
		{
			//var createTextureNormalTask = Texture2D.CreateTextureAsync(normalTextureFile);
			//var createTextureColTask = Texture2D.CreateTextureAsync(colorTextureFile);
			var bitmapColTask = Task.Run(() => new Bitmap(colorTextureFile));
			var bitmapNormalTask = Task.Run(() => new Bitmap(normalTextureFile));
			var createObjModelTask = ObjModel.LoadWithTangentsAsync(objFile);
			var parseMtlTask = MtlParser.ParseMtlAsync(materialFile);

			ShaderAttribVertices = shaderAttribVertices;
			ShaderAttribTexCoords = shaderAttribTexCoords;
			ShaderAttribNormals = shaderAttribNormals;
			ShaderTextureSampler = shaderTextureSampler;
			ShaderTextureSampler2 = textureSampler2;
			ShaderAttribTangents = shaderAttribTangents;
			ShaderAttribBiTangents = shaderAttribBiTangents;
			//TextureNormal = new Texture2D(normalTextureFile);
			//TextureColor = new Texture2D(colorTextureFile);
			//Material = MtlParser.ParseMtl(materialFile)[0];
			/*
			var watches = System.Diagnostics.Stopwatch.StartNew();
			Scene = new AssimpContext().ImportFile(objFile, PostProcessSteps.GenerateSmoothNormals
				| PostProcessSteps.CalculateTangentSpace
				| PostProcessSteps.Triangulate
				);
			Console.WriteLine(watches.ElapsedMilliseconds);
			if (!Scene.HasMeshes)
			{
				throw new MissingFieldException("No meshes found!");
			}
			// this whole class is only made for single mesh files
			if (Scene.MeshCount > 1)
			{
				throw new MissingFieldException("Found multiple meshes!");
			}

			InitObjModel();
			FindModelBorders();
			*/
			

			//TextureNormal = createTextureNormalTask.Result;
			//TextureColor = createTextureColTask.Result;

			TextureColor = new Texture2D(bitmapColTask.Result);
			TextureNormal = new Texture2D(bitmapNormalTask.Result);
			Model = createObjModelTask.Result;
			InitBasicVao();
            Material = parseMtlTask.Result[0];
		}


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
			GL.DrawArrays(OpenTK.Graphics.OpenGL4.PrimitiveType.Triangles, 0, Model.VerticesFloat.Length / 3);//Scene.Meshes[0].VertexCount);
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
