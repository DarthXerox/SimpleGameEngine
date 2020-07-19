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
    public class WorldObject : IDisposable
    {
		public List<Transformations> ModelTransformations { protected set; get; } 
			
		public Mesh RawMesh { protected set; get; }

		public WorldObject(Mesh mesh)
        {
            ModelTransformations = new List<Transformations>();
			RawMesh = mesh ?? throw new NullReferenceException($"Null mesh reference in {GetType()}.ctor");
		}

        public WorldObject(Mesh mesh, List<Transformations> modelTransformations)
        {
            ModelTransformations = modelTransformations;
			RawMesh = mesh ?? throw new NullReferenceException($"Null mesh reference in {GetType()}.ctor");
		}

		protected WorldObject(Transformations transformations)
        {
            ModelTransformations = new List<Transformations>
            {
				transformations
            };
		}

		public void AddTransformation(Transformations transformations)
		{
			ModelTransformations.Add(transformations);
		}

		public virtual void Draw(LightsProgram lightsProgram, Player player, float maxDistance = 100)
		{
			lightsProgram.AttachMaterial(RawMesh.Material);
			foreach (var transformation in ModelTransformations)
			{
				// we can use the fact that everything is rendered in xz plane
				if (Vector2.Distance(transformation.Position.Xz, player.Position.Xz) < maxDistance)
				{
					lightsProgram.AttachModelMatrix(transformation.GetModelMatrix());
					RawMesh.Draw(lightsProgram);
				}
			}
		}

        public virtual void Dispose()
        {
			RawMesh.Dispose();
        }
    }
}
