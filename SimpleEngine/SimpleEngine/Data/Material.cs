using OpenTK;

namespace SimpleEngine.Data
{
    /// <summary>
    /// Contains data specific to a material (colors, shininess...)
    /// </summary>
    public class Material
    {
        public string Name { set; get; }
        public Vector3 Ambient { set; get; }
        public Vector3 Diffuse { set; get; }
        public Vector3 Specular { set; get; }
        public float Shininess { set; get; }
        public float? Transparency { set; get; }
    }
}
