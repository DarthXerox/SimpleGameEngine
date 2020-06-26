using System.Collections.Generic;
using System.Linq;
using GameNamespace;
using OpenGL_in_CSharp.Utils;
using OpenTK;

namespace OpenGL_in_CSharp.TextRendering
{
    public class TextBox
    {
        public float MinX { get; }
        public float MaxX { get; }
        public float MinY { get; }
        public float MaxY { get; }

        public float MidX { get; }
        public float MidY { get; }

        public bool HasHitbox { get; }
        public bool IsHighlighted { private set; get; } = false;
        public Vector3 Color { set; get; }
        public Vector3 HighLightColor = new Vector3(1, 1, 0);
        private readonly float pixelLength;
        private readonly float pixelHeight;

        public string Text { get; }
        public float Scale { get; }

        public float PixelLength => pixelLength; 
        public float PixelHeight => pixelHeight; 
        FreeTypeFont Font { get; }

        public TextBox(float midX, float midY, string text, float scale, 
            Vector3 baseColor, FreeTypeFont font, bool hasHitbox = true)
        {
            Text = text;
            Scale = scale;
            Font = font;
            Color = baseColor;
            MidX = midX;
            MidY = midY;
            HasHitbox = hasHitbox;
            Font.GetPixelLength(Text, scale, ref pixelLength, ref pixelHeight);
            MinX = midX - PixelLength / 2;
            MinY = midY - PixelHeight / 2;
            MaxX = MinX + PixelLength;
            MaxY = MinY + PixelHeight;
        }

        public bool IsColliding(float x, float y)
        {
            if (HasHitbox && x <= MaxX && x >= MinX && y <= MaxY && y >= MinY)
            {
                return true;
            }
            return false;
        }

        public bool IsColliding(Vector2 pos)
        {
            return IsColliding(pos.X, pos.Y);
        }

        /// <summary>
        /// Drawing uses alignment to middle
        /// </summary>
        /// <param name="mousePos">mouse cursor position RELATIVE to window MIDDLE </param>
        public void Draw(Vector2 mousePos,
            int modelUniform = 0, int colorUniform = 2, int textureBinding = 0)
        {
            //Console.WriteLine(PixelLength);
            if (HasHitbox && IsColliding(mousePos.X, mousePos.Y))
            {
                if (!IsHighlighted)
                {
                    Sounder.PlaySound(FilePaths.SoundMenuBtnHover);
                }
                float scaleMultiplier = 1.2f;
                Font.RenderText(Text, MidX - scaleMultiplier * (PixelLength / 2), 
                    MidY + scaleMultiplier * (PixelHeight / 2),
                    scaleMultiplier * Scale, 
                    HighLightColor, modelUniform, colorUniform, textureBinding);
                IsHighlighted = true;
            } 
            else
            {
                IsHighlighted = false;
                //each letter's origin is in its topleft corner, that's why we have to render them at MaxY
                Font.RenderText(Text, MinX, MaxY, Scale, Color, modelUniform, colorUniform, textureBinding);
            }
        }
    }

    public class GUI
    {
        public Dictionary<TextBox, GameStates> TextBoxes { set; get; } 
            = new Dictionary<TextBox, GameStates>();

        public GUI(Dictionary<TextBox, GameStates> textBoxes)
        {
            TextBoxes = textBoxes;
        }

        /// <param name="mousePos">mouse cursor position RELATIVE to window's MIDDLE </param>
        public GameStates OnMouseClick(Vector2 mousePos)
        {
            foreach (var pair in TextBoxes)
            {
                if (pair.Key.HasHitbox && pair.Key.IsColliding(mousePos))
                {
                    return pair.Value;
                }
            }
            return GameStates.None;
        }

        /// <param name="mousePos">mouse cursor position RELATIVE to window MIDDLE </param>
        public void Draw(Vector2 mousePos, int modelUniform = 0, int colorUniform = 2, int textureBinding = 0)
        {
            TextBoxes.Keys.ToList().ForEach(textBox => textBox.Draw(mousePos, modelUniform, colorUniform, textureBinding));
        }
    }
}
