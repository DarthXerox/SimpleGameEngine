﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
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

		public virtual void Draw(LightsProgram lightsProgram, Player player, float maxDistance = 100)
		{
			lightsProgram.AttachMaterial(RawMesh.Material);
			foreach (var transformation in ModelTransformations)
			{
				// we can misuse the fact that everything is rendered in xz plane
				//if (checkDistance)
				//{
					if (Vector2.Distance(transformation.Position.Xz, player.Position.Xz) < maxDistance)
					{
						lightsProgram.AttachModelMatrix(transformation.GetModelMatrix());
						RawMesh.Draw(lightsProgram);
					}
				/*}
				//if (!checkDistance || Vector2.Distance(transformation.Position.Xz, player.Position.Xz) < 100)
				else
				{
					lightsProgram.AttachModelMatrix(transformation.GetModelMatrix());
					RawMesh.Draw(lightsProgram);
				}*/
			}
		}
	}
}
