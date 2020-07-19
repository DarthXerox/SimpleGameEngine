using OpenTK;
using SimpleEngine.Data;
using SimpleEngine.Utils;

namespace SimpleEngine.Text
{
    public class TextBox
    {
        private readonly float pixelLength;
        private readonly float pixelHeight;
        public float PixelLength => pixelLength;
        public float PixelHeight => pixelHeight;
        public float MinX { get; }
        public float MaxX { get; }
        public float MinY { get; }
        public float MaxY { get; }
        public float MidX { get; }
        public float MidY { get; }
        public bool HasHitbox { get; }
        public bool IsHighlighted { private set; get; } = false;
        public Vector3 Color { set; get; }
        public Vector3 HighLightColor { set; get; } = new Vector3(1, 1, 0);
        public string Text { get; }
        public float Scale { get; }

        /// <summary>
        /// Font is passed by a reference (not created within TextBox)
        /// so it is not a good idea to dispose of it later
        /// </summary>
        public FreeTypeFont Font { get; } 

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
            // Moves the borders so that midX and midY are exactly in the middle of the text "rectangle"
            // => creates alignment to middle
            Font.GetTextPixelSize(Text, scale, ref pixelLength, ref pixelHeight);
            MinX = midX - PixelLength / 2f;
            MinY = midY - PixelHeight / 2f;
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
            if (HasHitbox && pos.X <= MaxX && pos.X >= MinX && pos.Y <= MaxY && pos.Y >= MinY)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Drawing uses alignment to middle
        /// </summary>
        /// <param name="mousePos">mouse cursor position RELATIVE to window MIDDLE </param>
        public void Draw(Vector2 mousePos, int modelUniform = 0, int colorUniform = 2, int textureBinding = 0)
        {
            if (HasHitbox && IsColliding(mousePos))
            {
                if (!IsHighlighted) // makes sure the sound is played only once while the mouse is hovering over the text 
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
}
