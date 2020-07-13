using SimpleEngine.Data;
using SimpleEngine.GameScene;
using OpenTK;

namespace SimpleEngine.Utils
{
    /// <summary>
    /// Represents a program that uses light, materials and fog for rendering
    /// </summary>
    public class LightsProgram : ShaderProgram
    {
        public int PositionAttrib { protected set; get; }
        public int TexCoordsAttrib { protected set; get; }
        public int NormalsAttrib { protected set; get; }
        public int ModelUniform { protected set; get; }
        public int ViewUniform { protected set; get; }
        public int ProjectionUniform { protected set; get; }
        public int TextureSamplerUniform { protected set; get; }

        public LightsProgram(params string[] shaderFilePaths) : base(shaderFilePaths)
        {
        }

        public void AttachModelMatrix(Matrix4 matrix)
        {
            AttachUnifromMatrix4(matrix, "model");
        }

        public void AttachViewMatrix(Matrix4 matrix)
        {
            AttachUnifromMatrix4(matrix, "view");
        }

        public void AttachProjectionMatrix(Matrix4 matrix)
        {
            AttachUnifromMatrix4(matrix, "projection");
        }

        public void AttachLight(Light light, int index = 0)
        {
            AttachUniformVector4(light.Position, $"lights[{index}].position");
            AttachUniformVector3(light.Color, $"lights[{index}].ambient");
            AttachUniformVector3(light.Color, $"lights[{index}].diffuse");
            AttachUniformVector3(light.Color, $"lights[{index}].specular");

            AttachUniformVector3(light.Direction, $"lights[{index}].direction");
            AttachUniformFloat(light.ConstantAtt, $"lights[{index}].constant");
            AttachUniformFloat(light.LinearAtt, $"lights[{index}].linear");
            AttachUniformFloat(light.QuadraticAtt, $"lights[{index}].quadratic");

            AttachUniformFloat(light.CutOff, $"lights[{index}].cutOff");
            AttachUniformFloat(light.OuterCutOff, $"lights[{index}].outerCutOff");
        }

        public void AttachFog(Fog fog)
        {
            AttachUniformFloat(fog.Density, "fog.density");
            AttachUniformVector3(fog.Color, "fog.color");
        }

        public void AttachMaterial(Material material)
        {
            AttachUniformVector3(material.Ambient, "material.ambient");
            AttachUniformVector3(material.Diffuse, "material.diffuse");
            AttachUniformVector3(material.Specular, "material.specular");
            AttachUniformFloat(material.Shininess, "material.shininess");
        }
    }
}
