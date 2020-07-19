using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace SimpleEngine.Data
{
    /// <summary>
    /// Takes care of preloading all necessary data from disk
    /// Contains subset of all filepaths to resources (only the necessary ones for the current version of the game)
    /// </summary>
    public static class DataLoader
    {
        private static readonly List<string> ObjFilesLst = new List<string>()
        {
            FilePaths.ObjTreeTrunk,
            FilePaths.ObjTreeLeaves,
            FilePaths.ObjMossyRock1
        };

        private static readonly List<string> ImagesFilesLst = new List<string>()
        {
            FilePaths.TextureHeightMap,
            FilePaths.TextureTreeTrunk,
            FilePaths.BumpTexTrunk,
            FilePaths.TextureTreeLeaves3,
            FilePaths.BumpTexTreeLeaves,
            FilePaths.TextureMossyRock,
            FilePaths.BumpTexMossyRock,
            FilePaths.TextureBrickWall,
            FilePaths.BumpTexBrickWall,
            FilePaths.TextureGrass4,
            FilePaths.BumpTexGrass4,
            FilePaths.TextureGrass1,
            FilePaths.BumpTexGrass1,
            FilePaths.TextureGrass2,
            FilePaths.BumpTexGrass2
        };

        private static readonly List<string> MtlFilesLst = new List<string>()
        {
            FilePaths.MtlBronze,
            FilePaths.MtlEmerald,
            FilePaths.MtlChrome,
            FilePaths.MtlTreeTrunk,
            FilePaths.MtlMossyRock1
        };

        /// <summary>
        /// One of the longest processes at the game start is loading a texture (bitmap)
        /// A 1024 x 1024 pix texture has over a million bytes and each has to be read into the bitmap
        /// We want to put the loading process in the background
        /// </summary>
        public static async Task<ConcurrentDictionary<string, Bitmap>> LoadAllBitmapsAsync()
        {
            return await LoadDataAsync(ImagesFilesLst, str => new Bitmap(str));
        }

        public static async Task<ConcurrentDictionary<string, ObjModel>> LoadAllObjModelsWithTangentsAsync()
        {
            return await LoadDataAsync(ObjFilesLst, ObjModel.LoadWithTangents);
        }
        
        public static async Task<ConcurrentDictionary<string, Material>> LoadAllMaterialsAsync()
        {
            return await LoadDataAsync(MtlFilesLst, str => MtlParser.ParseMtl(str)[0]);
        }

        private static async Task<ConcurrentDictionary<string, T>> LoadDataAsync<T>(List<string> filePaths, Func<string, T> dataCreator) 
        {
            var dict = new ConcurrentDictionary<string, T>();

            await Task.Run(() => Parallel.ForEach(filePaths, str =>
            {
                dict.TryAdd(str, dataCreator(str));
            }));

            return dict;
        }
    }
}
