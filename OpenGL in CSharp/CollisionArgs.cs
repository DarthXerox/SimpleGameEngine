using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGL_in_CSharp
{
    public class CollisionArgs : EventArgs
    {
        public ICollidable PointOfCollision { set; get; }

        public CollisionArgs(ICollidable pointOfCollision)
        {
            PointOfCollision = pointOfCollision;
        }
    }
}
