using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using OpenGL_in_CSharp.Utils;

namespace OpenGL_in_CSharp
{
    /// <summary>
    /// Takes care of preloading all necessary data from disk
    /// Contains subset of all filepaths to resources (only the ones necesary for the current version of the game)
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
            FilePaths.HeightMapPath,
            FilePaths.TextureTreeTrunk,
            FilePaths.BumpTexTrunk,
            FilePaths.TextureTreeLeaves3,
            FilePaths.BumpTexTreeLeaves,
            FilePaths.TextureMossyRock,
            FilePaths.BumpTexMossyRock,
            FilePaths.TextureBrickWall,
            FilePaths.BumpTexBrickWall,
            FilePaths.TextureGrass4,
            FilePaths.BumpTexGrass4
        };

        private static readonly List<string> MtlFilesLst = new List<string>()
        {
            FilePaths.MtlBronze,
            FilePaths.MtlEmerald,
            FilePaths.MtlChrome
        };

        /// <summary>
        /// One of the longest processes at the game start is loading a texture
        /// A 1024 x 1024 pix texture has over a million bytes and each has to be read into the bitmap
        /// We want to put the loading process in the background
        /// </summary>
        public static async Task<ConcurrentDictionary<string, Bitmap>> LoadAllBitmapsAsync()
        {
            /*
            var Textures = new ConcurrentDictionary<string, Bitmap>(
                new Dictionary<string, Bitmap>()
                {
                    { FilePaths.HeightMapPath, null},
                    { FilePaths.TextureTreeTrunk, null},
                    { FilePaths.BumpTexTrunk, null},
                    { FilePaths.TextureTreeLeaves3, null},
                    { FilePaths.BumpTexTreeLeaves, null},
                    { FilePaths.TextureMossyRock, null},
                    { FilePaths.BumpTexMossyRock, null},
                    { FilePaths.TextureBrickWall, null},
                    { FilePaths.BumpTexBrickWall, null},
                    { FilePaths.TextureGrass4, null},
                    { FilePaths.BumpTexGrass4, null},
                });


            return await Task.Run(() =>
            {
                Parallel.ForEach(Textures.Keys, key =>
                {
                    Textures.TryUpdate(key, new Bitmap(key), null);
                });
                return Textures;
            });
            */
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
