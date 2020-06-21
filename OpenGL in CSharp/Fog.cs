using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace OpenGL_in_CSharp
{
    public class Fog
    {
        public Vector3 Color { get; }
        public float Denstity { get; }

        public Fog(float density)
        {
            Denstity = density;
            Color = new Vector3(0.5f, 0.5f, 0.5f);
        }
    }
}
