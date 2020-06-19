using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace OpenGL_in_CSharp
{
    /// <summary>
    /// Each object implementing this class
    /// </summary>
    public interface ICollidable
    {
        Vector3 Position { get; }

        Vector3 LowerCentre { get; }

        float Height { get; }

        float Radius { get; }

        bool IsColliding(ICollidable other);

        void OnCollisionCheck(object source, CollisionArgs args);

        void MoveOutOfCollision(ICollidable other);

    }
}
