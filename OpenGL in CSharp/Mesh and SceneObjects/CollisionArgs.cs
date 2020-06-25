using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGL_in_CSharp
{
    public class CollisionArgs : EventArgs
    {
        public Player PointOfCollision { set; get; }

        public CollisionArgs(Player pointOfCollision)
        {
            PointOfCollision = pointOfCollision;
        }
    }
}
