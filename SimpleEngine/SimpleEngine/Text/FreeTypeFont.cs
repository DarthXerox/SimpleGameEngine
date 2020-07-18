using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using SharpFont;
using SimpleEngine.Utils;

namespace SimpleEngine.Text
{
    /// <summary>
    /// Class used for loading .ttf  font files and drawing them
    /// </summary>
    public class FreeTypeFont : IDisposable
    {
        public Dictionary<uint, Character> Characters { get; } = new Dictionary<uint, Character>();
        private int vaoID;
        private int vboID;

        public FreeTypeFont(uint pixelHeight, string fontFile, int positionAttrib = 0, int texCoordsAttrib = 1)
        {
            using (Face face = new Face(new Library(), fontFile))
            {
                // Setting the width to 0 lets the face dynamically calculate the width based on the given height
                face.SetPixelSizes(0, pixelHeight); 

                // set 1 byte pixel alignment 
                GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

                GL.ActiveTexture(TextureUnit.Texture0);

                for (uint asciiVal = 0; asciiVal < 128; asciiVal++)
                {
                    face.LoadChar(asciiVal, LoadFlags.Render, LoadTarget.Normal);
                    GlyphSlot glyph = face.Glyph;
                    FTBitmap bitmap = glyph.Bitmap;

                    // Creates a simple texture from the glyphs bitmap (unfortunately FTBitmap is different from Bitmap)
                    GL.CreateTextures(TextureTarget.Texture2D, 1, out int texID);
                    GL.BindTexture(TextureTarget.Texture2D, texID);
                    GL.TexImage2D(TextureTarget.Texture2D, 0,
                                    PixelInternalFormat.R8, bitmap.Width, bitmap.Rows, 0,
                                    PixelFormat.Red, PixelType.UnsignedByte, bitmap.Buffer);

                    GL.TextureParameter(texID, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                    GL.TextureParameter(texID, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                    GL.TextureParameter(texID, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                    GL.TextureParameter(texID, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                    
                    Character chr = new Character
                    {
                        TextureID = texID,
                        Size = new Vector2(bitmap.Width, bitmap.Rows),
                        Bearing = new Vector2(glyph.BitmapLeft, glyph.BitmapTop),
                        Advance = glyph.Advance.X.Value
                    };
                    Characters.Add(asciiVal, chr);
                }
            }
            GL.BindTexture(TextureTarget.Texture2D, 0);
            // set default (4 byte) pixel alignment 
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 4);

            // Whenever an instance of this class is created only 4 * 6 floats are sent to GPU
            // and for each rendered character only a model matrix is different
            float[] quad =
            {
                // x      y       u     v    
                0.0f, -1.0f,   0.0f, 1.0f,
                0.0f,  0.0f,   0.0f, 0.0f,
                1.0f,  0.0f,   1.0f, 0.0f,
                0.0f, -1.0f,   0.0f, 1.0f,
                1.0f,  0.0f,   1.0f, 0.0f,
                1.0f, -1.0f,   1.0f, 1.0f
            };

            GL.CreateBuffers(1, out vboID);
            GL.NamedBufferStorage(vboID, 4 * 6 * sizeof(float), quad, 0);

            GL.CreateVertexArrays(1, out vaoID);
            GL.EnableVertexArrayAttrib(vaoID, positionAttrib);
            GL.VertexArrayVertexBuffer(vaoID, positionAttrib, vboID, IntPtr.Zero, 4 * sizeof(float));
            GL.VertexArrayAttribFormat(vaoID, positionAttrib, 2, VertexAttribType.Float, false, 0);
            GL.VertexArrayAttribBinding(vaoID, positionAttrib, positionAttrib);

            GL.EnableVertexArrayAttrib(vaoID, texCoordsAttrib);
            GL.VertexArrayVertexBuffer(vaoID, texCoordsAttrib, vboID, (IntPtr)(2 * sizeof(float)), 4 * sizeof(float));
            GL.VertexArrayAttribFormat(vaoID, texCoordsAttrib, 2, VertexAttribType.Float, false, 0);
            GL.VertexArrayAttribBinding(vaoID, texCoordsAttrib, texCoordsAttrib);
        }


        public void RenderText(string text, float x, float y, float scale, Vector3 color,
            int modelUniform = 0, int colorUniform = 2, int textureBinding = 0)
        {
            var transformations = new Transformations();

            GL.BindVertexArray(vaoID);
            float pixelAdvancementX = 0.0f;
            foreach (var c in text)
            {
                if (!Characters.ContainsKey(c))
                {
                    throw new InvalidOperationException("Invalid ASCII character! Valid are only first 128 ASCII characters");
                }

                Character chr = Characters[c];
                transformations.Position = new Vector3(
                    x + pixelAdvancementX + chr.Bearing.X * scale,
                    y + chr.Bearing.Y * scale,
                    0f);
                transformations.Scaling = new Vector3(chr.Size * scale);


                // Now advance cursors for next glyph (note that advance is number of 1/64 pixels)
                pixelAdvancementX += (chr.Advance >> 6) * scale; // Bitshift by 6 to get value in pixels (2^6 = 64 (divide amount of 1/64th pixels by 64 to get amount of pixels))
                Matrix4 modelMat = transformations.GetModelMatrix();
                GL.UniformMatrix4(modelUniform, false, ref modelMat);

                GL.BindTextureUnit(textureBinding, chr.TextureID);
                GL.Uniform3(colorUniform, color);
                GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            }

            GL.BindVertexArray(0);
        }

        public void GetTextPixelSize(string text, float scale, ref float length, ref float height)
        {
            length = 0.0f;
            height = 0.0f;
            foreach (var c in text)
            {
                if (!Characters.ContainsKey(c))
                {
                    throw new InvalidOperationException("Invalid ASCII character! Valid are only first 128 ASCII characters");
                }
                Character chr = Characters[c];
                length += (chr.Advance >> 6) * scale; 
                if (chr.Size.Y > height)
                {
                    height = chr.Size.Y;
                }
            }
        }

        public void Dispose()
        {
            // making a struct an IDisposable doesn't make sense
            Characters.Values
                .ToList()
                .ForEach(chr => GL.DeleteTexture(chr.TextureID));
            Characters.Clear();
            GL.DeleteBuffer(vboID);
            GL.DeleteVertexArray(vaoID);
        }
    }
}
