using System;
using System.Collections.Generic;
using OpenGL_in_CSharp.Utils;
using OpenTK;

namespace OpenGL_in_CSharp
{
	/// <summary>
	/// Defines a package or connection between a mesh/model 
	/// and multiple positions in world (or transformation)
	/// </summary>
	public class SceneObject 
	{
		public List<ModelTransformations> ModelTransformations { protected set; get; } 
			= new List<ModelTransformations>();

		public Mesh RawMesh { protected set; get; }

		public SceneObject(Mesh mesh, ModelTransformations initialTransformations)
		{
			RawMesh = mesh ?? throw new NullReferenceException($"Null mesh reference in {GetType()}.ctor");
			ModelTransformations.Add(initialTransformations);
		}

		public SceneObject(Mesh mesh, Vector3 initialPosition) 
			: this (mesh, new ModelTransformations() { Position = initialPosition }) { }

		public SceneObject(Mesh mesh) 
		{
			RawMesh = mesh;
		}

		protected SceneObject(ModelTransformations transformations) 
		{
			ModelTransformations.Add(transformations);
		}

		public void AddPosition(Vector3 pos)
		{
			ModelTransformations.Add(new ModelTransformations()
			{
				Position = pos
			});
		}

		public void AddTransformation(ModelTransformations transformations)
		{
			ModelTransformations.Add(transformations);
		}

		public virtual void Draw(LightsProgram lightsProgram)
		{
			lightsProgram.AttachMaterial(RawMesh.Material);
			foreach (var transformation in ModelTransformations)
			{
				lightsProgram.AttachModelMatrix(transformation.GetModelMatrix());
				RawMesh.Draw(lightsProgram);
			}
		}
	}
}
