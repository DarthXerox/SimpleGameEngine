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
        //public List<ICollidable> Collidables { private set; get; }
        public ICollidable PointOfCollision { private set; get; }

        public delegate void CollisionCheckingEventHandler(object sauce, EventArgs args);

        public event CollisionCheckingEventHandler CollisionChecking;

        public CollisionManager(ICollidable point)
        {
            //Collidables = lst;
            PointOfCollision = point;

            //Collidables.ForEach(col => CollisionDetected += col.OnCollisionDetected);
        }

        
        protected virtual void OnCollisionChecking()
        {
            CollisionChecking?.Invoke(this, EventArgs.Empty);
        }

    }
}
