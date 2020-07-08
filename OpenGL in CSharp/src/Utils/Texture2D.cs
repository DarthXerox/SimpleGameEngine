using System;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL4;

namespace OpenGL_in_CSharp.Utils
{
    /// <summary>
    /// From a given picture file (png, jpg...) creates and loads a texture
    /// </summary>
    public class Texture2D : IDisposable
    {
        public int ID { get; }
        public Bitmap Data { private set; get; }
        public Texture2D(string fileName)
        {
            //ID = GL.GenTexture();
            int temp;
            GL.CreateTextures(TextureTarget.Texture2D, 1, out temp);
            ID = temp;

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
            // Data.Dispose();

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

        public void Dispose()
        {
            GL.DeleteTexture(ID);
            Data.Dispose();
        }
    }
}
