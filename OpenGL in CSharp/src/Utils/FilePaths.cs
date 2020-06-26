﻿using System.IO;

namespace OpenGL_in_CSharp.Utils
{
    public static class FilePaths
    {
        public static readonly string Prefix = $"..{Path.DirectorySeparatorChar}.." +
            $"{Path.DirectorySeparatorChar}res{Path.DirectorySeparatorChar}";
        public static readonly string TextureFolder = $"{Prefix}Textures{Path.DirectorySeparatorChar}";
        public static readonly string ShadersFolder = $"{Prefix}Shaders{Path.DirectorySeparatorChar}";
        public static readonly string FontsFolder = $"{Prefix}Fonts{Path.DirectorySeparatorChar}";
        public static readonly string ObjectsFolder = $"{Prefix}Objects{Path.DirectorySeparatorChar}";
        public static readonly string MaterialsFolder = $"{Prefix}Materials{Path.DirectorySeparatorChar}";
        public static readonly string SoundsFolder = $"{Prefix}Sounds{Path.DirectorySeparatorChar}";

        //public static readonly string VertexShaderPath = ShadersFolder + "VertexShader.vert";
       // public static readonly string FragmentShaderPath = ShadersFolder + "FragmentShader.frag";
        public static readonly string NormalMappingFrag = ShadersFolder + "NormalMapping.frag";
        public static readonly string NormalMappingVert = ShadersFolder + "NormalMapping.vert";
        public static readonly string FakeNormalMappingVert = ShadersFolder + "VertFakeNormalMapping.glsl";

        public static readonly string TextVertex = ShadersFolder + "Text.vert";
        public static readonly string TextFrag = ShadersFolder + "Text.frag";
        public static readonly string PostprocessVert = ShadersFolder + "postprocess.vert";
        public static readonly string PostprocessFrag = ShadersFolder + "postprocess.frag";


        public static readonly string HeightMapPath = TextureFolder + "heightmap.png";
        //public static readonly string TexturePath = TextureFolder + "Lelouch.jpg";
        //public static readonly string TexturePath2 = TextureFolder + "img.jpg";
        //public static readonly string TexturePathWood = TextureFolder + "wood.jpg";
        //public static readonly string TexturePathRocks = TextureFolder + "rocks.jpg";
        //public static readonly string TexturePathRed = TextureFolder + "red_square.jpg";
        //public static readonly string TexturePathSampleMan = TextureFolder + "diffuse.png";
        //public static readonly string TexturePathGrass = TextureFolder + "grass_texture.jpg";
        //public static readonly string TexturePathGrass2 = TextureFolder + "grass2.png";
        public static readonly string TextureTreeTrunk = TextureFolder + "tree_trunk.jpg";
        //public static readonly string TextureTreeLeaves = TextureFolder + "tree_leaves.jpg";
        //public static readonly string TextureTreeLeaves2 = TextureFolder + "tree_leaves2.png";
        public static readonly string TextureTreeLeaves3 = TextureFolder + "tree_leaves3.png";
        //public static readonly string TextureTallGrass = TextureFolder + "tall_grass.png";
        public static readonly string TextureMossyRock = TextureFolder + "Colormap.jpg";
        public static readonly string TextureBrickWall = TextureFolder + "brickwall.jpg";
       // public static readonly string TextureGrass3 = TextureFolder + "grass3 color.jpg";
        public static readonly string TextureGrass4 = TextureFolder + "grass4 color.jpg";
        //public static readonly string TextureKey = TextureFolder + "key.png";
        //public static readonly string TextureCoin = TextureFolder + "coin color.png";


       // public static readonly string SansFont = FontsFolder + "FreeSans.ttf";
        //public static readonly string HaloFont = FontsFolder + "Halo3.ttf";
        public static readonly string MonoFont = FontsFolder + "joystix monospace.ttf";

       // public static readonly string BumpTexGrass3 = TextureFolder + "grass3 normal.jpg";
        public static readonly string BumpTexGrass4 = TextureFolder + "grass4 normal.jpg";
        public static readonly string BumpTexBrickWall = TextureFolder + "brickwall_normal.jpg";
        public static readonly string BumpTexMossyRock = TextureFolder + "Normalmap.jpg";
        public static readonly string BumpTexTrunk = TextureFolder + "tree_trunk_normal.png";
        public static readonly string BumpTexTreeLeaves = TextureFolder + "tree_leaves_normal.png";
        //public static readonly string BumpTextKey = TextureFolder + "key normal.png";
        //public static readonly string BumpTexCoin = TextureFolder + "coin normal.png";

       // public static readonly string ObjCube = ObjectsFolder + "test.obj";
       // public static readonly string ObjCubeBlender = ObjectsFolder + "cubeBlender.obj";
      //  public static readonly string ObjDragon = ObjectsFolder + "dragon.obj";
  //      public static readonly string ObjSampleMan = ObjectsFolder + "custom.obj";
        public static readonly string ObjTreeTrunk = ObjectsFolder + "tree_trunk.obj";
        public static readonly string ObjTreeLeaves = ObjectsFolder + "tree_leaves.obj";
    //    public static readonly string ObjTallGrass = ObjectsFolder + "tall_grass.obj";
        public static readonly string ObjMossyRock1 = ObjectsFolder + "Mossy Rock 1.obj";
      //  public static readonly string ObjMossyRock2 = ObjectsFolder + "Mossy Rock 2.obj";
        //public static readonly string ObjMossyRock3 = ObjectsFolder + "Mossy Rock 3.obj";
   //     public static readonly string ObjKey = ObjectsFolder + "key.obj";
     //   public static readonly string ObjKey2 = ObjectsFolder + "key2.obj";
       // public static readonly string ObjCoin = ObjectsFolder + "coin.obj";

      //  public static readonly string MtlGold = MaterialsFolder + "gold.mtl";
        public static readonly string MtlBronze = MaterialsFolder + "bronze.mtl";
        public static readonly string MtlChrome = MaterialsFolder + "chrome.mtl";
        public static readonly string MtlEmerald = MaterialsFolder + "emerald.txt";


        public static readonly string MtlMossyRock1 = MaterialsFolder + "Mossy Rock 1.mtl";

        public static readonly string SoundStonePickUp = SoundsFolder + "stone pickup.wav";
        public static readonly string SoundMenuBtnHover = SoundsFolder + "menu btn hover.wav";
       // public static readonly string SoundGrassWalking = SoundsFolder + "grass walking.wav";
      //  public static readonly string SoundGameWon = SoundsFolder + ".wav";

    }
}