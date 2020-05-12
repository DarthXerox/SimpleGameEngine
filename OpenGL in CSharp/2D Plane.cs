using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace OpenGL_in_CSharp
{
    public static class Plane
    {
        public static readonly float[,] Vertices = new float[,] { 
                                                            { -0.8f, -0.8f, 0.5f },
                                                            { 0.8f, -0.8f, 0.5f },
                                                            { 0.8f, 0.8f, 0.5f },
                                                            { -0.8f, 0.8f, 0.5f } };

        public static readonly byte[,] Color = new byte[,] {
            { 0, 0, 0 },
            { 0, 0, 0 },
            { 255, 255, 255 },
            { 255, 255, 255 } 
        };
    }
}
