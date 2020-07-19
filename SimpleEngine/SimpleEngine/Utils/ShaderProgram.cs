using System;
using System.Collections.Generic;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace SimpleEngine.Utils
{
    /// <summary>
    /// Provides connection between shaders (Resources/Shaders) and C# classes
    /// </summary>
    public class ShaderProgram : IDisposable
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
        public List<int> ShaderIds { get; } = new List<int>();

        public ShaderProgram(params string[] shaderFilePaths)
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
            if (GL.GetProgramInfoLog(ID).Length != 0)
            {
                Console.Error.WriteLine(GL.GetProgramInfoLog(ID));
                throw new Exception("Couldn't link shader program!");
            }
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
            if (GL.GetShaderInfoLog(shaderId).Length != 0)
            {
                Console.Error.WriteLine(GL.GetShaderInfoLog(shaderId));
                throw new Exception($"Couldn't compile shader {path}!");
            }

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

    }
}
