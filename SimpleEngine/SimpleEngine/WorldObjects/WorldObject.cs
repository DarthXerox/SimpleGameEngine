using System;
using System.Collections.Generic;
using SimpleEngine.GameScene;
using SimpleEngine.Utils;
using OpenTK;

namespace SimpleEngine.WorldObjects
{
    /// <summary>
    /// Defines a package or connection between a mesh/model 
    /// and multiple positions in world (or transformation)
    /// </summary>
    public class WorldObject
    {
		public List<ModelTransformations> ModelTransformations { protected set; get; } 
			= new List<ModelTransformations>();

		public Mesh RawMesh { protected set; get; }

		public WorldObject(Mesh mesh, ModelTransformations initialTransformations)
		{
			RawMesh = mesh ?? throw new NullReferenceException($"Null mesh reference in {GetType()}.ctor");
			ModelTransformations.Add(initialTransformations);
		}

		public WorldObject(Mesh mesh) 
		{
			RawMesh = mesh;
		}

		protected WorldObject(ModelTransformations transformations) 
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
				if (Vector2.Distance(transformation.Position.Xz, player.Position.Xz) < maxDistance)
				{
					lightsProgram.AttachModelMatrix(transformation.GetModelMatrix());
					RawMesh.Draw(lightsProgram);
				}
			}
		}
	}
}
