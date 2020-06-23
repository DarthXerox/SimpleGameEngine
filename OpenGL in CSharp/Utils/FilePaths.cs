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
        public static readonly string NormalMappingFrag = ShadersFolder + "NormalMapping.frag";
        public static readonly string NormalMappingVert = ShadersFolder + "NormalMapping.vert";

        public static readonly string HeightMapPath = TextureFolder + "heightmap.png";
        public static readonly string TexturePath = TextureFolder + "Lelouch.jpg";
        public static readonly string TexturePath2 = TextureFolder + "img.jpg";
        public static readonly string TexturePathWood = TextureFolder + "wood.jpg";
        public static readonly string TexturePathRocks = TextureFolder + "rocks.jpg";
        public static readonly string TexturePathRed = TextureFolder + "red_square.jpg";
        public static readonly string TexturePathSampleMan = TextureFolder + "diffuse.png";
        public static readonly string TexturePathGrass = TextureFolder + "grass_texture.jpg";
        public static readonly string TexturePathGrass2 = TextureFolder + "grass2.png";
        public static readonly string TextureTreeTrunk = TextureFolder + "tree_trunk.jpg";
        public static readonly string TextureTreeLeaves = TextureFolder + "tree_leaves.jpg";
        public static readonly string TextureTreeLeaves2 = TextureFolder + "tree_leaves2.png";
        public static readonly string TextureTreeLeaves3 = TextureFolder + "tree_leaves3.png";
        public static readonly string TextureTallGrass = TextureFolder + "tall_grass.png";
        public static readonly string TextureMossyRock = TextureFolder + "Colormap.jpg";
        public static readonly string TextureBrickWall = TextureFolder + "brickwall.jpg";

        public static readonly string BumpTexBrickWall = TextureFolder + "brickwall_normal.jpg";
        public static readonly string BumpTexMossyRock = TextureFolder + "Normalmap.jpg";
        public static readonly string BumpTexTrunk = TextureFolder + "tree_trunk_normal.png";
        public static readonly string BumpTexTreeLeaves = TextureFolder + "tree_leaves_normal.png";

        public static readonly string ObjCube = ObjectsFolder + "test.obj";
        public static readonly string ObjCubeBlender = ObjectsFolder + "cubeBlender.obj";
        public static readonly string ObjDragon = ObjectsFolder + "dragon.obj";
        public static readonly string ObjSampleMan = ObjectsFolder + "custom.obj";
        public static readonly string ObjTreeTrunk = ObjectsFolder + "tree_trunk.obj";
        public static readonly string ObjTreeLeaves = ObjectsFolder + "tree_leaves.obj";
        public static readonly string ObjTallGrass = ObjectsFolder + "tall_grass.obj";
        public static readonly string ObjMossyRock1 = ObjectsFolder + "Mossy Rock 1.obj";
        public static readonly string ObjMossyRock2 = ObjectsFolder + "Mossy Rock 2.obj";
        public static readonly string ObjMossyRock3 = ObjectsFolder + "Mossy Rock 3.obj";

        public static readonly string MtlBasic = MaterialsFolder + "custom.mtl";
        public static readonly string MtlTest = MaterialsFolder + "test.mtl";
        public static readonly string MtlGold = MaterialsFolder + "gold.mtl";
        public static readonly string MtlTreeTrunk = MaterialsFolder + "tree_trunk.mtl";
        public static readonly string MtlTreeLeaves = MaterialsFolder + "tree_leaves.mtl";
        public static readonly string MtlMossyRock1 = MaterialsFolder + "Mossy Rock 1.mtl";
        public static readonly string MtlMossyRock2 = MaterialsFolder + "Mossy Rock 2.mtl";
        public static readonly string MtlMossyRock3 = MaterialsFolder + "Mossy Rock 3.mtl";
    }
}
