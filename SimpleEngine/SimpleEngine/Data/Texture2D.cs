using System;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL4;

namespace SimpleEngine.Data
{
    /// <summary>
    /// From a given picture file (png, jpg...) creates and loads a texture to GPUs
    /// </summary>
    public class Texture2D : IDisposable
    {
        public int ID { get; }

        /// <summary>
        /// Normal synchronous way to load texture
        /// </summary>
        public Texture2D(string imageFile) : this(new Bitmap(imageFile)) { }

        /// <summary>
        /// Useful for async bitmap loading.
        /// Sending texture data to GPU takes time too, but as OpenGL is very thread sensitive, 
        /// all GL calls can only be made within an opengl context (gamewindow) and is bound to one thread only
        /// (FYI: contexts can share texture data, but creating a context is time consuming)
        /// </summary>
        /// <param name="bitmap">Preloaded Bitmap object</param>
        public Texture2D(Bitmap bitmap)
        {
            GL.CreateTextures(TextureTarget.Texture2D, 1, out int temp);
            ID = temp;

            bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);

            GL.BindTexture(TextureTarget.Texture2D, ID);

            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TextureStorage2D(ID, (int)Math.Log(bitmap.Width, 2), SizedInternalFormat.Rgba8, bitmap.Width, bitmap.Height);
            GL.TextureSubImage2D(ID, 0, 0, 0, bitmap.Width, bitmap.Height, OpenTK.Graphics.OpenGL4.PixelFormat.Bgra,
                PixelType.UnsignedByte, bitmapData.Scan0);
            GL.GenerateTextureMipmap(ID);

            bitmap.UnlockBits(bitmapData);

            GL.TextureParameter(ID, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TextureParameter(ID, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            GL.TextureParameter(ID, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TextureParameter(ID, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);

            bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
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
        }
    }
}
