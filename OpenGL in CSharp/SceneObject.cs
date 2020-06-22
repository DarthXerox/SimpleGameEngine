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
	public class SceneObject : ModelTransformations
	{
		/// <summary>
		/// The idea is that many objects might share the same mesh but only have different position
		/// Rotations are in degrees
		/// </summary>
		/// 
		/*
		public float RotX { set; get; } = 0.0f;
		public float RotY { set; get; } = 0.0f;
		public float RotZ { set; get; } = 0.0f;
		public float ScalingFactor { set; get; } = 1.0f;
		public Vector3 Position { set; get; } = new Vector3(1.0f);
		*/
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

		/*
		public Matrix4 GetModelMatrix()
		{
			//https://www.youtube.com/watch?v=oc8Yl4ZruCA&list=PLRIWtICgwaX0u7Rf9zkZhLoLuZVfUksDP&index=7
			Matrix4 model = Matrix4.Identity;
			model *= Matrix4.CreateRotationX(MathHelper.DegreesToRadians(RotX));
			model *= Matrix4.CreateRotationY(MathHelper.DegreesToRadians(RotY));
			model *= Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(RotZ));
			model *= Matrix4.CreateScale(ScalingFactor);
			model *= Matrix4.CreateTranslation(Position);

			return model;
		}
		*/

		public void Draw()
		{
			RawMesh?.Draw();
		}
	}

}