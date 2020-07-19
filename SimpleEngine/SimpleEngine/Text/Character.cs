using OpenTK;

namespace SimpleEngine.Text
{
    /// <summary>
    /// Represents a single character of a font
    /// </summary>
    public struct Character
    {
        public int TextureID { get; set; }
        public Vector2 Size { get; set; }
        public Vector2 Bearing { get; set; }
        public int Advance { get; set; }
    }
}
