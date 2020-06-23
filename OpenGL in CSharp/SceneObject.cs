using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using OpenGL_in_CSharp.Utils;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace OpenGL_in_CSharp
{
	/// <summary>
	/// Defines a package or connection between a mesh/model and a position in world (or transformation)
	/// </summary>
	public class SceneObject : ModelTransformations
	{
		public Mesh RawMesh { protected set; get; }

		public SceneObject(string objFileName, string textureFileName, int shaderAttribVertices,
			int shaderAttribTexCoords, int shaderAttribNormals, int shaderTextureSampler)
		{ 
			RawMesh = new Mesh(objFileName, textureFileName, shaderAttribVertices,
				shaderAttribTexCoords, shaderAttribNormals, shaderTextureSampler);
		}

		public SceneObject(string objFileName, string textureFileName)
		{
			RawMesh = new Mesh(objFileName, textureFileName);
		}

		public SceneObject(string objFileName, string textureFileName, string texNormalFileName)
		{
			RawMesh = new NormalMappingMesh(objFileName, textureFileName, texNormalFileName);
		}

		public SceneObject() { }

		public SceneObject(Mesh mesh)
		{
			RawMesh = mesh;
		}

		public void GenerateMesh(string objFileName, string textureFileName, int shaderAttribVertices,
			int shaderAttribTexCoords, int shaderAttribNormals, int shaderTextureSampler)
		{
			RawMesh = new Mesh(objFileName, textureFileName, shaderAttribVertices,
				shaderAttribTexCoords, shaderAttribNormals, shaderTextureSampler);
		}

		public void Draw()
		{
			RawMesh?.Draw();
		}
	}

}