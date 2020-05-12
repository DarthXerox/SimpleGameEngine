using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using OpenTK.Graphics.OpenGL4;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
namespace OpenGL_in_CSharp.Utils
{
    class Texture2D
    {
        public int ID { get; }
        public Bitmap Data { private set; get; }

        public int Width { get { return Data.Width; } }
        public int Height { get { return Data.Height; } }


        public Texture2D(int id, Bitmap data)
        {
            ID = id;
            Data = data;
        }

        public Texture2D(string fileName)
        {
            ID = GL.GenTexture();

            Data = new Bitmap(fileName);
            Data.RotateFlip(RotateFlipType.RotateNoneFlipY);

            GL.BindTexture(TextureTarget.Texture2D, ID);

            BitmapData bitmapData = Data.LockBits(new Rectangle(0, 0, Data.Width, Data.Height), ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TextureStorage2D(ID, (int) Math.Log(Data.Width, 2), SizedInternalFormat.Rgba8, Data.Width, Data.Height);
            GL.TextureSubImage2D(ID, 0, 0, 0, Data.Width, Data.Height, OpenTK.Graphics.OpenGL4.PixelFormat.Bgra,
                PixelType.UnsignedByte, bitmapData.Scan0);
            GL.GenerateTextureMipmap(ID);

            Data.UnlockBits(bitmapData);

            GL.TextureParameter(ID, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TextureParameter(ID, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            GL.TextureParameter(ID, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TextureParameter(ID, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        }


        public void Use()
        {
            GL.BindTexture(TextureTarget.Texture2D, ID);
        }

        public void Use(int shaderLocation)
        {
            GL.BindTextureUnit(shaderLocation, ID);
        }
    }
}
