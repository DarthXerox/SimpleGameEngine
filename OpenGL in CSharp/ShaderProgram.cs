﻿using System;
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
    public class ShaderProgram : IDisposable
    {
        public static List<ShaderType> AvailableShaders { get; } = new List<ShaderType>()
        {
            ShaderType.VertexShader,
            ShaderType.FragmentShader,
            ShaderType.GeometryShader,
            ShaderType.ComputeShader
        };
        public int ID { get; }

        public int LightsAmnt { private set; get; } = 0;

        /// <summary>
        /// Each shader's type corresponds to type in <param>AvailableShaders</param> on the same index
        /// Not every program needs to have exactly 4 shaders, it usually is 2
        /// </summary>
        public List<int> ShaderIds { private set; get; } = new List<int>();


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

            Console.WriteLine(GL.GetProgramInfoLog(ID));
        }

        private int CompileShader(string path, ShaderType shaderType)
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

        public void Use()
        {
            GL.UseProgram(ID);
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

        private void AttachUnifromMatrix4(Matrix4 matrix, string shaderAttribName)
        {
            GL.ProgramUniformMatrix4(ID, GL.GetUniformLocation(ID, shaderAttribName), false, ref matrix);
        }

        private void AttachUniformFloat(float val, string shaderAttribName)
        {
            GL.ProgramUniform1(ID, GL.GetUniformLocation(ID, shaderAttribName), val);
        }

        private void AttachUniformVector3(Vector3 vec, string shaderAttribName)
        {
            GL.ProgramUniform3(ID, GL.GetUniformLocation(ID, shaderAttribName), vec);
        }

        private void AttachUniformVector4(Vector4 vec, string shaderAttribName)
        {
            GL.ProgramUniform4(ID, GL.GetUniformLocation(ID, shaderAttribName), vec);
        }


        /*
        public void AttachDirectionalLight(Light light, int index = 0)
        {
            AttachUniformVector4(light.Position, $"lights[{index}].position");
            AttachUniformVector3(light.Color, $"lights[{index}].ambient");
            AttachUniformVector3(light.Color, $"lights[{index}].diffuse");
            AttachUniformVector3(light.Color, $"lights[{index}].specular");
           
        }

        public void AttachPointLight(PointLight light, int index = 0)
        {
            AttachDirectionalLight(light, index);
            AttachUniformVector3(light.Direction, $"lights[{index}].direction");
            AttachUniformFloat(light.ConstantAtt, $"lights[{index}].constant");
            AttachUniformFloat(light.LinearAtt, $"lights[{index}].linear");
            AttachUniformFloat(light.QuadraticAtt, $"lights[{index}].quadratic");

            // pointlight == conelight with 180 degree cutoff
            AttachUniformFloat(-1, $"lights[{index}].cutOff");
            AttachUniformFloat(-1, $"lights[{index}].outerCutOff");
        }

        public void AttachConeLight(ConeLight light, int index = 0)
        {
            //AttachPointLight(light, index);
            
            AttachUniformVector4(light.Position, $"lights[{index}].position");
            AttachUniformVector3(light.Color, $"lights[{index}].ambient");
            AttachUniformVector3(light.Color, $"lights[{index}].diffuse");
            AttachUniformVector3(light.Color, $"lights[{index}].specular");

            AttachUniformVector3(light.Direction, $"lights[{index}].direction");
            AttachUniformFloat(light.ConstantAtt, $"lights[{index}].constant");
            AttachUniformFloat(light.LinearAtt, $"lights[{index}].linear");
            AttachUniformFloat(light.QuadraticAtt, $"lights[{index}].quadratic");
            
            // two more uniform assignments dont matter too much, it is a very cheap operation
            AttachUniformFloat(light.CutOff, $"lights[{index}].cutOff");
            AttachUniformFloat(light.OuterCutOff, $"lights[{index}].outerCutOff");
        }
        */
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


        public void Dispose()
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
