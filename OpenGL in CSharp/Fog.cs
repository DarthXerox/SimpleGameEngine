using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace OpenGL_in_CSharp
{
    public class Fog
    {
        public Vector3 Color { get; }
        public float Density { get; }

        public Fog(float density)
        {
            Density = density;
            Color = new Vector3(0.5f, 0.5f, 0.5f);
        }

        public Fog(float density, Vector3 color)
        {
            Density = density;
            Color = color;
        }
    }
}
