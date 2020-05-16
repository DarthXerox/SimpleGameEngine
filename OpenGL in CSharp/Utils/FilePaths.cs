using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace OpenGL_in_CSharp.Utils
{
    public static class FilePaths
    {
        public static readonly string Prefix = $"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}";
        public static readonly string VertexShaderPath = Prefix + "VertexShader.vert";
        public static readonly string FragmentShaderPath = Prefix + "FragmentShader.frag";

        public static readonly string TexturePath = Prefix + "Lelouch.jpg";
        public static readonly string TexturePath2 = Prefix + "img.jpg";
        public static readonly string TexturePathWood = Prefix + "wood.jpg";
        public static readonly string TexturePathRocks = Prefix + "rocks.jpg";
        public static readonly string TexturePathRed = Prefix + "red_square.jpg";


        public static readonly string ObjCube = Prefix + "test.obj";
        public static readonly string ObjCubeBlender = Prefix + "cubeBlender.obj";
        public static readonly string ObjDragon = Prefix + "dragon.obj";


    }
}
