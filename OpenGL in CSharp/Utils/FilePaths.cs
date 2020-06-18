using System.IO;

namespace OpenGL_in_CSharp.Utils
{
    public static class FilePaths
    {
        public static readonly string Prefix = $"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}";
        public static readonly string TextureFolder = $"{Prefix}Textures{Path.DirectorySeparatorChar}";
        public static readonly string ShadersFolder = $"{Prefix}Shaders{Path.DirectorySeparatorChar}";
        public static readonly string ObjectsFolder = $"{Prefix}Objects{Path.DirectorySeparatorChar}";
        public static readonly string MaterialsFolder = $"{Prefix}Materials{Path.DirectorySeparatorChar}";

        public static readonly string VertexShaderPath = ShadersFolder + "VertexShader.vert";
        public static readonly string FragmentShaderPath = ShadersFolder + "FragmentShader.frag";

        public static readonly string TexturePath = TextureFolder + "Lelouch.jpg";
        public static readonly string TexturePath2 = TextureFolder + "img.jpg";
        public static readonly string TexturePathWood = TextureFolder + "wood.jpg";
        public static readonly string TexturePathRocks = TextureFolder + "rocks.jpg";
        public static readonly string TexturePathRed = TextureFolder + "red_square.jpg";
        public static readonly string TexturePathSampleMan = TextureFolder + "diffuse.png";
        public static readonly string TexturePathGrass = TextureFolder + "grass_texture.jpg";
        public static readonly string TexturePathGrass2 = TextureFolder + "grass2.png";


        public static readonly string HeightMapPath = TextureFolder + "heightmap.png";

        public static readonly string ObjCube = ObjectsFolder + "test.obj";
        public static readonly string ObjCubeBlender = ObjectsFolder + "cubeBlender.obj";
        public static readonly string ObjDragon = ObjectsFolder + "dragon.obj";
        public static readonly string ObjSampleMan = ObjectsFolder + "custom.obj";

        public static readonly string MtlBasic = MaterialsFolder + "custom.mtl";
        public static readonly string MtlTest = MaterialsFolder + "test.mtl";
        public static readonly string MtlGold = MaterialsFolder + "gold.mtl";

    }
}
