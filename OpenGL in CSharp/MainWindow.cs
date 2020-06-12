using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OpenGL_in_CSharp;
using OpenGL_in_CSharp.Utils;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;

namespace GameNamespace
{
    // More interesting tutorials
    // http://neokabuto.blogspot.com/p/tutorials.html
    public class MainWindow : GameWindow
    {
        public ShaderProgram Program { private set; get; } // ID of the program
        
        // These must correspond to the given loacations in shaders
        private int shaderAttribPosition = 0;
        private int shaderAttribTexCoors = 1;
        private int shaderAttribNormals = 2;

        private int shaderUniformMatModel = 0;
        private int shaderUniformMatView = 1;
        private int shaderUniformMatProjection = 2;
        private int shaderUnifromLigthPos = 3;
        private int shaderUniformLightCol = 4;

        private int shaderUniformTextureSampler = 0;
        private int shaderUniformTextureSampler2 = 1;

        public Vector3 WorldOrigin { private set; get; } = new Vector3(0.0f);


        //private Matrix4 matModel = Matrix4.Identity; 
        private Matrix4 matView;
        private Matrix4 matProjection;
        
        public Camera Camera { private set; get; }


        private SceneObject objectToDraw;
        private Material objectMaterial;

        private SceneObject[] animation = new SceneObject[25];

        private Light light;

        private float counter = 0.0f;
        private int framecounter = 0;
        private int step = 2;

        public MainWindow() //1280:720
           : base(720, // initial width
            720, // initial height
            GraphicsMode.Default,
            "Slenderman??",  // initial title
            GameWindowFlags.Default,
            DisplayDevice.Default,
            4, // OpenGL major version
            0, // OpenGL minor version
            GraphicsContextFlags.ForwardCompatible)
        {
            Title += ": OpenGL Version: " + GL.GetString(StringName.Version);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            GL.Viewport(0, 0, Width, Height);
            GL.ClearColor(Color4.CornflowerBlue);
            Program = new ShaderProgram(FilePaths.VertexShaderPath, FilePaths.FragmentShaderPath);
            light = new Light(new Vector3(10.0f, 10.0f, 5.0f), true); //new Light(new Vector4(-0.5f, 0.75f, 0.5f, 1.0f));


            /*objectToDraw = new SceneObject(FilePaths.ObjDragon, FilePaths.TexturePathRed, 
                shaderAttribPosition, shaderAttribTexCoors, shaderAttribNormals, shaderUniformTextureSampler);

            objectMaterial = MtlParser.ParseMtl(FilePaths.MtlGold)[1];
            */
            objectToDraw = new Terrain(100, 100, FilePaths.TexturePathGrass);
            objectMaterial = MtlParser.ParseMtl(FilePaths.MtlGold)[0];

            for (int i = 0; i < 25; i++)
            {
                animation[i] = new SceneObject(FilePaths.Prefix + $"Animation{Path.DirectorySeparatorChar}anim ({i + 1}).obj"
                    , FilePaths.TexturePathSampleMan,
                shaderAttribPosition, shaderAttribTexCoors, shaderAttribNormals, shaderUniformTextureSampler);
            }
            animation[0].RotX = 30.0f;
            animation[0].RotY = 20.0f;
            animation[0].Translation = new Vector3(0.0f, -3.0f, 0.0f);
            
            Camera = new Camera(
                new Vector3(0.0f, 2.0f, 20.0f),
                WorldOrigin,
                new Vector3(0.0f, 1.0f, 0.0f)
                );

            //GL.Enable(EnableCap.DepthTest);
            //GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
        }


        protected override void OnRenderFrame(FrameEventArgs e)
        {
            Camera.Move();
            base.OnRenderFrame(e);
            Title = $"(Vsync: {VSync}) FPS: {1f / e.Time:0}";
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.DepthTest);

            /*counter += 0.025f;
            float radius = 20.0f;
            float camX = (float)Math.Sin(counter) * radius;
            float camZ = (float)Math.Cos(counter) * radius;
            Camera = new Camera(
                new Vector3(camX, 2.0f, camZ),
                WorldOrigin,
                new Vector3(0.0f, 1.0f, 0.0f)
                );
                */



            matProjection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(55.0f), (float)Width / Height, 0.1f, 100.0f);
            /*Camera = new Camera(
                new Vector3(0.0f, 2.0f, 20.0f),
                WorldOrigin,
                new Vector3(0.0f, 1.0f, 0.0f)
                );*/
            matView = Camera.GetViewMatrix();

            
            Program.Use();
            GL.ProgramUniform3(Program.ID, 5, Camera.Position);
            GL.ProgramUniform3(Program.ID, 8, objectMaterial.Ambient);
            GL.ProgramUniform3(Program.ID, 9, objectMaterial.Diffuse);
            GL.ProgramUniform3(Program.ID, 10, objectMaterial.Specular);
            GL.ProgramUniform1(Program.ID, 11, objectMaterial.Shininess);

            GL.ProgramUniform4(Program.ID, shaderUnifromLigthPos, light.Position);
            GL.ProgramUniform3(Program.ID, shaderUniformLightCol, light.Color);

            //Program.AttachModelMatrix(animation[0].GetModelMatrix());
            Program.AttachModelMatrix(Matrix4.Identity);
            Program.AttachViewMatrix(matView);
            Program.AttachProjectionMatrix(matProjection);

            objectToDraw.Draw();
            //Console.WriteLine(framecounter / step);
            /*animation[framecounter / step].Draw();
            framecounter++;
            if (framecounter >= step * 21)
            {
                framecounter = 0;
            }*/

            SwapBuffers();
        }




        






        /// <summary>
        /// NOT IMPORTANT
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            //GL.DeleteProgram(Program);
            Exit();
        }


        protected override void OnFocusedChanged(EventArgs e)
        {
            //pause the game
        }


        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            HandleKeyboard();
        }
        private void HandleKeyboard()
        {
            
            var keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Key.Escape))
            {
                Exit();
            } 
            else if (keyState.IsKeyDown(Key.F))
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            }
            else if (keyState.IsKeyDown(Key.L))
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            }
            else if (keyState.IsKeyDown(Key.P))
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Point);
                GL.PointSize(10);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
        }

        /*
        private int CreateProgram(string vertexShaderPath, string fragShaderPath)
        {
            int programId = GL.CreateProgram();
            int vertexShaderId = CompileShader(vertexShaderPath, ShaderType.VertexShader);
            int fragmentShaderId = CompileShader(fragShaderPath, ShaderType.FragmentShader);

            GL.AttachShader(programId, vertexShaderId);
            GL.AttachShader(programId, fragmentShaderId);
            GL.LinkProgram(programId);

            GL.DeleteShader(vertexShaderId);
            GL.DeleteShader(fragmentShaderId);

            GL.DetachShader(programId, vertexShaderId);
            GL.DetachShader(programId, fragmentShaderId);
            Console.WriteLine(GL.GetProgramInfoLog(programId));

            return programId;
        }

        private int CompileShader(string path, ShaderType shaderType)
        {
            var shaderId = GL.CreateShader(shaderType);
            GL.ShaderSource(shaderId, File.ReadAllText(path));
            GL.CompileShader(shaderId);
            Console.WriteLine(GL.GetShaderInfoLog(shaderId));

            return shaderId;
        }
        */
    }
}
