using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace OpenGL_in_CSharp.Utils
{
    public static class Vector3Extensions
    {
        public static bool IsWithinDistanceInPlane(this Vector3 vec, Vector3 other, float distance)
        {
            return (distance * distance) >= ((vec.X - other.X) * (vec.X - other.X) +
                (vec.Z - other.Z) * (vec.Z - other.Z));
        }

        public static void MoveFromInPlane(ref this Vector3 vec, Vector3 other, float distance)
        {
            float acutalDistance = (float) Math.Sqrt((double) (vec.X - other.X) * (vec.X - other.X) +
                (vec.Z - other.Z) * (vec.Z - other.Z));
            float difference = distance - acutalDistance;
            var unitMovementVector = vec - other;
            unitMovementVector.Y = vec.Y;
            unitMovementVector.Normalize();

            vec += unitMovementVector * difference;
        }
    }

    public static class Vector2Extensions 
    { 
        public static void Resize(ref this Vector2 vec, float newLength)
        {
            vec *= (newLength / vec.Length);
        }
    }

}
