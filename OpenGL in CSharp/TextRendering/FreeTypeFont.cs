using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

using SharpFont;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenGL_in_CSharp.Utils;

namespace OpenGL_in_CSharp.TextRendering
{
    /// <summary>
    ///
    /// This class is based on: https://github.com/Rabbid76/c_sharp_opengl/blob/master/OpenTK_example_5/FreeTypeFont.cs
    /// The only thing I kept the same is the texture loading part in constructor
    /// I reworked the rest (and I had no choice it was a complete mess...)
    ///
    /// </summary>

    public struct Character
    {
        public int TextureID { get; set; }
        public Vector2 Size { get; set; }
        public Vector2 Bearing { get; set; }
        public int Advance { get; set; }
    }

    public class FreeTypeFont
    {
        public Dictionary<uint, Character> Characters { get; } = new Dictionary<uint, Character>();
        public int VaoID { private set; get; }
        public int VboID { private set; get; }

        public FreeTypeFont(uint pixelheight, int positionAttrib = 0, int texCoordsAttrib = 1)
        {
            using (Face face = new Face(new Library(), FilePaths.SansFont))
            {
                face.SetPixelSizes(0, pixelheight);

                // set 1 byte pixel alignment 
                GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

                // set texture unit
                GL.ActiveTexture(TextureUnit.Texture0);

                // Load first 128 characters of ASCII set
                for (uint asciiVal = 0; asciiVal < 128; asciiVal++)
                {
                    face.LoadChar(asciiVal, LoadFlags.Render, LoadTarget.Normal);
                    GlyphSlot glyph = face.Glyph;
                    FTBitmap bitmap = glyph.Bitmap;

                    // create glyph texture
                    int texObj = GL.GenTexture();
                    GL.BindTexture(TextureTarget.Texture2D, texObj);
                    GL.TexImage2D(TextureTarget.Texture2D, 0,
                                    PixelInternalFormat.R8, bitmap.Width, bitmap.Rows, 0,
                                    PixelFormat.Red, PixelType.UnsignedByte, bitmap.Buffer);

                    // set texture parameters
                    GL.TextureParameter(texObj, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                    GL.TextureParameter(texObj, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                    GL.TextureParameter(texObj, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                    GL.TextureParameter(texObj, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

                    // add character
                    Character chr = new Character
                    {
                        TextureID = texObj,
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

            // Whenever an instance of this class is created only 4 * 6 are sent to GPU
            // and for each rendered character only a model matrix is different
            float[] vquad =
            {
                // x      y      u     v    
                0.0f, -1.0f,   0.0f, 1.0f,
                0.0f,  0.0f,   0.0f, 0.0f,
                1.0f,  0.0f,   1.0f, 0.0f,
                0.0f, -1.0f,   0.0f, 1.0f,
                1.0f,  0.0f,   1.0f, 0.0f,
                1.0f, -1.0f,   1.0f, 1.0f
            };

            VboID = GL.GenBuffer();
            GL.NamedBufferStorage(VboID, 4 * 6 * sizeof(float), vquad, 0);

            VaoID = GL.GenVertexArray();
            GL.EnableVertexArrayAttrib(VaoID, positionAttrib);
            GL.VertexArrayVertexBuffer(VaoID, positionAttrib, VboID, IntPtr.Zero, 4 * sizeof(float));
            GL.VertexArrayAttribFormat(VaoID, positionAttrib, 2, VertexAttribType.Float, false, 0);
            GL.VertexArrayAttribBinding(VaoID, positionAttrib, positionAttrib);

            GL.EnableVertexArrayAttrib(VaoID, texCoordsAttrib);
            GL.VertexArrayVertexBuffer(VaoID, texCoordsAttrib, VboID, (IntPtr) (2 * sizeof(float)), 4 * sizeof(float));
            GL.VertexArrayAttribFormat(VaoID, texCoordsAttrib, 2, VertexAttribType.Float, false, 0);
            GL.VertexArrayAttribBinding(VaoID, texCoordsAttrib, texCoordsAttrib);
        }

        public void RenderText(string text, float x, float y, float scale, Vector3 color, 
            int modelUniform = 0, int colorUniform = 2, int textureBinding = 0)
        {
            ModelTransformations transformations = new ModelTransformations();

            GL.BindVertexArray(VaoID);
            float pixelAdvancementX = 0.0f;
            foreach (var c in text)
            {
                if (!Characters.ContainsKey(c))
                {
                    throw new InvalidOperationException("Invalid ASCII character! Use only first 128");
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
        /*
        public void RenderText(string text, float x, float y, float scaling, int modelUniform = 0)
        {
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindVertexArray(_vao);

            Matrix4 rotateM = Matrix4.Identity;//CreateRotationZ(angle_rad);
            Matrix4 transOriginM = Matrix4.CreateTranslation(new Vector3(x, y, 0f));

            // Iterate through all characters
            float char_x = 0.0f;
            foreach (var c in text)
            {
                if (Characters.ContainsKey(c) == false)
                    continue;
                Character ch = Characters[c];

                float w = ch.Size.X * scaling;
                float h = ch.Size.Y * scaling;
                float xrel = char_x + ch.Bearing.X * scaling;
                float yrel = ch.Bearing.Y * scaling;

                // Now advance cursors for next glyph (note that advance is number of 1/64 pixels)
                char_x += (ch.Advance >> 6) * scaling; // Bitshift by 6 to get value in pixels (2^6 = 64 (divide amount of 1/64th pixels by 64 to get amount of pixels))

                Matrix4 scaleM = Matrix4.CreateScale(new Vector3(w, h, 1.0f));
                Matrix4 transRelM = Matrix4.CreateTranslation(new Vector3(xrel, yrel, 0.0f));

                Matrix4 modelM = scaleM * rotateM * transOriginM * transRelM; // OpenTK `*`-operator is reversed
                GL.UniformMatrix4(modelUniform, false, ref modelM);

                // Render glyph texture over quad
                GL.BindTexture(TextureTarget.Texture2D, ch.TextureID);

                // Render quad
                GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            }

            GL.BindVertexArray(0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }
        */
        /*
        public void RenderText(string text, float x, float y, float scaling, int modelUniform = 0)
        {
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindVertexArray(_vao);


            // Iterate through all characters
            float char_x = 0.0f;
            foreach (var c in text)
            {
                if (Characters.ContainsKey(c) == false)
                    continue;
                Character ch = Characters[c];   
                
                float w = ch.Size.X * scaling;
                float h = ch.Size.Y * scaling;
                Matrix4 scaleM = Matrix4.CreateScale(new Vector3(w, h, 1.0f));

                float xrel = char_x + ch.Bearing.X * scalin;
                float yrel = ch.Bearing.Y * transformations.ScalingFactor;
                transformations.Position = new Vector3(transformations.Position.X + xrel,
                    transformations.Position.Y - yrel, transformations.Position.Z);
                
                // Now advance cursors for next glyph (note that advance is number of 1/64 pixels)
                char_x += (ch.Advance >> 6) * transformations.ScalingFactor; // Bitshift by 6 to get value in pixels (2^6 = 64 (divide amount of 1/64th pixels by 64 to get amount of pixels))

                Matrix4 scaleM = Matrix4.CreateScale(new Vector3(w, h, 1.0f));
                Matrix4 transRelM = Matrix4.CreateTranslation(new Vector3(xrel, yrel, 0.0f));

                Matrix4 modelM = scaleM * transRelM * rotateM * transOriginM; // OpenTK `*`-operator is reversed
                
                Matrix4 trans = scaleM* transformations.GetModelMatrix();
                GL.UniformMatrix4(modelUniform, false, ref trans);

                // Render glyph texture over quad
                GL.BindTexture(TextureTarget.Texture2D, ch.TextureID);

                // Render quad
                GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            }

            GL.BindVertexArray(0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }
*/
    }


    public class TextRenderer
    {
        public Dictionary<uint, Character> Characters { get; } = new Dictionary<uint, Character>();
        public int VaoID { private set; get; }
        public int VboID { private set; get; }

        public TextRenderer(uint pixelheight, string fontPath, int positionAttrib = 0, int texCoordsAttrib = 1)
        {
            Face face = new Face(new Library(), fontPath);
            face.SetPixelSizes(0, pixelheight);

            // set 1 byte pixel alignment 
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

            // set texture unit
            GL.ActiveTexture(TextureUnit.Texture0);

            // Load first 128 characters of ASCII set
            for (uint c = 0; c < 128; c++)
            {
                // load glyph
                //face.LoadGlyph(c, LoadFlags.Render, LoadTarget.Normal);
                face.LoadChar(c, LoadFlags.Render, LoadTarget.Normal);
                GlyphSlot glyph = face.Glyph;
                FTBitmap bitmap = glyph.Bitmap;


                // create glyph texture
                int texObj = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, texObj);
                GL.TexImage2D(TextureTarget.Texture2D, 0,
                                PixelInternalFormat.R8, bitmap.Width, bitmap.Rows, 0,
                                PixelFormat.Red, PixelType.UnsignedByte, bitmap.Buffer);

                // set texture parameters
                GL.TextureParameter(texObj, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TextureParameter(texObj, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                GL.TextureParameter(texObj, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TextureParameter(texObj, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

                // add character
                Character ch = new Character
                {
                    TextureID = texObj,
                    Size = new Vector2(bitmap.Width, bitmap.Rows),
                    Bearing = new Vector2(glyph.BitmapLeft, glyph.BitmapTop),
                    Advance = (int)glyph.Advance.X.Value
                };
                Characters.Add(c, ch);
            }

            // bind default texture
            GL.BindTexture(TextureTarget.Texture2D, 0);

            // set default (4 byte) pixel alignment 
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 4);

            float[] vquad =
            {
                // x      y      u     v    
                0.0f, -1.0f,   0.0f, 1.0f,
                0.0f,  0.0f,   0.0f, 0.0f,
                1.0f,  0.0f,   1.0f, 0.0f,
                0.0f, -1.0f,   0.0f, 1.0f,
                1.0f,  0.0f,   1.0f, 0.0f,
                1.0f, -1.0f,   1.0f, 1.0f
            };

            VboID = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VboID);
            GL.BufferData(BufferTarget.ArrayBuffer, 4 * 6 * 4, vquad, BufferUsageHint.StaticDraw);

            /*
            GL.GenVertexArrays(1, out _vao);
            GL.EnableVertexArrayAttrib(_vao, positionAttrib);
            GL.VertexArrayVertexBuffer(_vao, positionAttrib, _vbo, IntPtr.Zero, 2 * sizeof(float));
            GL
            */

            VaoID = GL.GenVertexArray();
            GL.BindVertexArray(VaoID);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * 4, 0);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * 4, 2 * 4);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }

        public void RenderText(string text, float x, float y, float scale, Vector2 dir, int modelUniform = 0)
        {
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindVertexArray(VaoID);

            float angle_rad = (float)Math.Atan2(dir.Y, dir.X);
            Matrix4 rotateM = Matrix4.Identity;//CreateRotationZ(angle_rad);
            Matrix4 transOriginM = Matrix4.CreateTranslation(new Vector3(x, y, 0f));

            // Iterate through all characters
            float char_x = 0.0f;
            foreach (var c in text)
            {
                if (Characters.ContainsKey(c) == false)
                    continue;
                Character ch = Characters[c];

                float w = ch.Size.X * scale;
                float h = ch.Size.Y * scale;
                float xrel = char_x + ch.Bearing.X * scale;
                float yrel = ch.Bearing.Y * scale;

                // Now advance cursors for next glyph (note that advance is number of 1/64 pixels)
                char_x += (ch.Advance >> 6) * scale; // Bitshift by 6 to get value in pixels (2^6 = 64 (divide amount of 1/64th pixels by 64 to get amount of pixels))

                Matrix4 scaleM = Matrix4.CreateScale(new Vector3(w, h, 1.0f));
                Matrix4 transRelM = Matrix4.CreateTranslation(new Vector3(xrel, yrel, 0.0f));

                Matrix4 modelM = scaleM * rotateM * transOriginM * transRelM; // OpenTK `*`-operator is reversed
                GL.UniformMatrix4(modelUniform, false, ref modelM);

                // Render glyph texture over quad
                GL.BindTexture(TextureTarget.Texture2D, ch.TextureID);

                // Render quad
                GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            }

            GL.BindVertexArray(0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }
    }
}
