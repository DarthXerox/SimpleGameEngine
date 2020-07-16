using System.IO;

namespace SimpleEngine.Data
{
    public static class FilePaths
    {
        public static readonly string Prefix = $"..{Path.DirectorySeparatorChar}.." +
            $"{Path.DirectorySeparatorChar}Resources{Path.DirectorySeparatorChar}";
        public static readonly string TextureFolder = $"{Prefix}Textures{Path.DirectorySeparatorChar}";
        public static readonly string ShadersFolder = $"{Prefix}Shaders{Path.DirectorySeparatorChar}";
        public static readonly string FontsFolder = $"{Prefix}Fonts{Path.DirectorySeparatorChar}";
        public static readonly string ObjectsFolder = $"{Prefix}Objects{Path.DirectorySeparatorChar}";
        public static readonly string MaterialsFolder = $"{Prefix}Materials{Path.DirectorySeparatorChar}";
        public static readonly string SoundsFolder = $"{Prefix}Sounds{Path.DirectorySeparatorChar}";

        public static readonly string NormalMappingFrag = ShadersFolder + "NormalMapping.frag";
        public static readonly string NormalMappingVert = ShadersFolder + "NormalMapping.vert";
        public static readonly string FakeNormalMappingVert = ShadersFolder + "FakeNormalMapping.vert";
        public static readonly string TextVertex = ShadersFolder + "Text.vert";
        public static readonly string TextFrag = ShadersFolder + "Text.frag";
        public static readonly string PostprocessVert = ShadersFolder + "postprocess.vert";
        public static readonly string PostprocessFrag = ShadersFolder + "postprocess.frag";

        public static readonly string TextureHeightMap = TextureFolder + "heightmap.png";
        public static readonly string TextureTreeTrunk = TextureFolder + "tree_trunk.jpg";
        public static readonly string TextureTreeLeaves3 = TextureFolder + "tree_leaves3.png";
        public static readonly string TextureMossyRock = TextureFolder + "Colormap.jpg";
        public static readonly string TextureBrickWall = TextureFolder + "brickwall.jpg";
        public static readonly string TextureGrass4 = TextureFolder + "grass4 color.jpg";
        public static readonly string TextureGrass1 = TextureFolder + "grass1.png";
        public static readonly string TextureGrass2 = TextureFolder + "grass2.png";



        public static readonly string FontSans = FontsFolder + "FreeSans.ttf";
        public static readonly string FontMono = FontsFolder + "joystix monospace.ttf";

        public static readonly string BumpTexGrass4 = TextureFolder + "grass4 normal.jpg";
        public static readonly string BumpTexGrass1 = TextureFolder + "grass1 normal.png";
        public static readonly string BumpTexGrass2 = TextureFolder + "grass2 normal.png";

        public static readonly string BumpTexBrickWall = TextureFolder + "brickwall_normal.jpg";
        public static readonly string BumpTexMossyRock = TextureFolder + "Normalmap.jpg";
        public static readonly string BumpTexTrunk = TextureFolder + "tree_trunk_normal.png";
        public static readonly string BumpTexTreeLeaves = TextureFolder + "tree_leaves_normal.png";


        public static readonly string ObjTreeTrunk = ObjectsFolder + "tree_trunk.obj";
        public static readonly string ObjTreeLeaves = ObjectsFolder + "tree_leaves.obj";
        public static readonly string ObjMossyRock1 = ObjectsFolder + "Mossy Rock 1.obj";
      
        public static readonly string MtlGold = MaterialsFolder + "gold.mtl";
        public static readonly string MtlBronze = MaterialsFolder + "bronze.mtl";
        public static readonly string MtlChrome = MaterialsFolder + "chrome.mtl";
        public static readonly string MtlEmerald = MaterialsFolder + "emerald.mtl";
        public static readonly string MtlMossyRock1 = MaterialsFolder + "Mossy Rock 1.mtl";
        public static readonly string MtlTreeTrunk = MaterialsFolder + "trunk.mtl";


        public static readonly string SoundStonePickUp = SoundsFolder + "stone pickup.wav";
        public static readonly string SoundMenuBtnHover = SoundsFolder + "menu btn hover.wav";
    }
}
