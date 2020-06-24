using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Input;
using SharpFont;

namespace OpenGL_in_CSharp.TextRendering
{
    public class TextBox
    {
        public float MinX {  get; }
        public float MaxX {  get; }
        public float MinY {  get; }
        public float MaxY { get; }

        public Vector3 Color { set; get; }
        public Vector3 HighLightColor = new Vector3(1, 1, 0);
        public string Text { get; }
        public float Scale { get; }

        FreeTypeFont Font { get; }

        public TextBox(int minX, int minY, string text, float scale, Vector3 baseColor, FreeTypeFont font)
        {
            MinX = minX;
            MinY = minY;
            Text = text;
            Scale = scale;
            Font = font;
            Color = baseColor;
            MaxX = Font.Characters[System.Text.Encoding.ASCII.GetBytes(text)[0]].Size.X * Scale * Text.Length;
            MaxY = Font.Characters[System.Text.Encoding.ASCII.GetBytes(text)[0]].Size.Y * Scale;
        }

        public bool IsColliding(float x, float y)
        {
            if (x <= MaxX && x >= MinX && y <= MaxY && y >= MinY)
            {
                return true;
            }
            return false;
        }

        public void Draw(BaseProgram postprocessProgram, MouseState mouse)
        {
            if (IsColliding(mouse.X, mouse.Y))
            {
                //draw with special color AND double scale
                //Font.RenderText(text, )
            } 
        }
    }

    public abstract class GUI
    {
        public Dictionary<TextBox, WindowState> TextBoxes = new Dictionary<TextBox, WindowState>();
    }


}
