using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace OpenGL_in_CSharp
{
    /// <summary>
    /// Provides connection between shaders and C# classes
    /// </summary>
    /// 

    public class AbstractShaderProgram : IDisposable
    {
        public static List<ShaderType> AvailableShaders { get; } = new List<ShaderType>()
        {
            ShaderType.VertexShader,
            ShaderType.FragmentShader,
            ShaderType.GeometryShader,
            ShaderType.ComputeShader
        };
        public int ID { protected set; get; }

        /// <summary>
        /// Each shader's type corresponds to type in <param>AvailableShaders</param> on the same index
        /// Not every program needs to have one of all of the shader types, it usually has 2
        /// </summary>
        public List<int> ShaderIds { private set; get; } = new List<int>();

        public AbstractShaderProgram(params string[] shaderFilePaths)
        {
            ID = GL.CreateProgram();
            for (int i = 0; i < shaderFilePaths.Length; i++)
            {
                if (i < AvailableShaders.Count)
                {
                    ShaderIds.Add(CompileShader(shaderFilePaths[i], AvailableShaders[i]));
                    GL.AttachShader(ID, ShaderIds[i]);
                }
            }
            GL.LinkProgram(ID);
            Console.WriteLine(GL.GetProgramInfoLog(ID));
        }

        protected int CompileShader(string path, ShaderType shaderType)
        {
            var shaderId = GL.CreateShader(shaderType);
            try
            {
                GL.ShaderSource(shaderId, File.ReadAllText(path));
            }
            catch (FileNotFoundException)
            {
                Console.Error.WriteLine("Filename: " + path);
                Console.Error.WriteLine("Shader file not found!!!");
            }
            catch (ArgumentException)
            {
                Console.Error.WriteLine("Filename: " + path);
                Console.Error.WriteLine("Incorrect format of shader filename!!!");
            }
            GL.CompileShader(shaderId);
            Console.WriteLine(GL.GetShaderInfoLog(shaderId));

            return shaderId;
        }

        public void AttachUnifromMatrix4(Matrix4 matrix, string shaderAttribName)
        {
            GL.ProgramUniformMatrix4(ID, GL.GetUniformLocation(ID, shaderAttribName), false, ref matrix);
        }

        public void AttachUniformFloat(float val, string shaderAttribName)
        {
            GL.ProgramUniform1(ID, GL.GetUniformLocation(ID, shaderAttribName), val);
        }

        public void AttachUniformVector3(Vector3 vec, string shaderAttribName)
        {
            GL.ProgramUniform3(ID, GL.GetUniformLocation(ID, shaderAttribName), vec);
        }

        public void AttachUniformVector4(Vector4 vec, string shaderAttribName)
        {
            GL.ProgramUniform4(ID, GL.GetUniformLocation(ID, shaderAttribName), vec);
        }

        public virtual void Use()
        {
            GL.UseProgram(ID);
        }

        public virtual void Dispose()
        {
            foreach (var id in ShaderIds)
            {
                GL.DeleteShader(id);
                GL.DetachShader(ID, id);
            }
            GL.DeleteProgram(ID);
        }

        //protected abstract void InitAttribLocations();
    }

    public class SimpleProgram : AbstractShaderProgram
    {
        public int PositionAttrib { protected set; get; }
        public int TexCoordsAttrib { protected set; get; }
        public int NormalsAttrib { protected set; get; }
        public int ModelUniform { protected set; get; }
        public int ViewUniform { protected set; get; }
        public int ProjectionUniform { protected set; get; }
        public int TextureSamplerUniform { protected set; get; }

        public SimpleProgram(params string[] shaderFilePaths) : base(shaderFilePaths)
        {
            /*
            ID = GL.CreateProgram();
            for (int i = 0; i < shaderFilePaths.Length; i++)
            {
                if (i < AvailableShaders.Count)
                {
                    ShaderIds.Add(CompileShader(shaderFilePaths[i], AvailableShaders[i]));
                    GL.AttachShader(ID, ShaderIds[i]);
                }
            }
            GL.LinkProgram(ID);
            InitAttribLocations();

            Console.WriteLine(GL.GetProgramInfoLog(ID));
            */
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

        /*
        protected override void InitAttribLocations()
        {
            PositionAttrib = GL.GetAttribLocation(ID, "position");
            TexCoordsAttrib = GL.GetAttribLocation(ID, "texCoors");
            NormalsAttrib = GL.GetAttribLocation(ID, "normals");

            ModelUniform = GL.GetUniformLocation(ID, "model");
            ViewUniform = GL.GetUniformLocation(ID, "view");
            ProjectionUniform = GL.GetUniformLocation(ID, "projection");

            TextureSamplerUniform = GL.GetUniformLocation(ID, "texture0");
        }
        */
    }


    public class TextProgram : AbstractShaderProgram
    {
        public int VertexAttrib { private set; get; }
        public int TextColorUniform { private set; get; }
        public int ProjectionUniform { private set; get; }

        public int TextSampler { private set; get; }

        public TextProgram(params string[] shaderFilePaths)
        {
            /*
            ID = GL.CreateProgram();
            for (int i = 0; i < shaderFilePaths.Length; i++)
            {
                if (i < AvailableShaders.Count)
                {
                    ShaderIds.Add(CompileShader(shaderFilePaths[i], AvailableShaders[i]));
                    GL.AttachShader(ID, ShaderIds[i]);
                }
            }
            GL.LinkProgram(ID);
            //InitAttribLocations();

            Console.WriteLine(GL.GetProgramInfoLog(ID));
            */
        }

        /*
        protected override void InitAttribLocations()
        {
            VertexAttrib = GL.GetAttribLocation(ID, "vertex");
            TextColorUniform = GL.GetUniformLocation(ID, "textColor");
            ProjectionUniform = GL.GetUniformLocation(ID, "projection");
            TextSampler = GL.GetUniformLocation(ID, "textColor");
        }
        */
    }

}
