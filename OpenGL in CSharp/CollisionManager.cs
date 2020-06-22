using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGL_in_CSharp
{
    /// <summary>
    /// Collision detection in my game uses 2 types of hitboxes:
    /// -> axis-aligned bounding box 
    /// -> cylinder perpendicular to plane xz (its upper and lower circle bases are parallel to xz plane)
    /// </summary>
    public class CollisionManager
    {
        public ICollidable PointOfCollision { private set; get; }
        public delegate void CollisionCheckingEventHandler(object sauce, CollisionArgs args);
        public event CollisionCheckingEventHandler CollisionChecking;

        public CollisionManager(ICollidable point)
        {
            PointOfCollision = point;
        }

        public void CheckCollisions()
        {
            OnCollisionChecking();
        }

        protected virtual void OnCollisionChecking()
        {
            CollisionChecking?.Invoke(this, new CollisionArgs(PointOfCollision));
        }
    }
}
