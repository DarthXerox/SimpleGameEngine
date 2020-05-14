using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL_in_CSharp;
using OpenGL_in_CSharp.Utils;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;

namespace GameNamespace
{
    class MainWindow : GameWindow
    {
        public static readonly string Prefix = $"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}";
        public static readonly string VertexShaderPath = Prefix + "VertexShader.vert";
        public static readonly string FragmentShaderPath = Prefix + "FragmentShader.frag";
        public static readonly string TexturePath = Prefix + "lelouch.jpg";

        private int frameCount = 0;

        public int ProgramID { private set; get; } // ID of the program
        private int vaoMain;
        private Texture2D texture1;
        private Texture2D texture2;


        private int vboPosition;
        private int vboTexCoors;

        // These must correspond to the given loacations in shaders
        private int shaderAttribPosition = 0;
        private int shaderAttribTexCoors = 1;
        private int shaderAttribNormals = 2;
        private int shaderUniformMatTrans = 1;

        private int shaderUniformMatView = 2;
        private int shaderUniformTextureSampler = 3;
        private int shaderUniformTextureSampler2 = 4;


        private Vector3[] vertData;
        private Vector3[] colData;
        private Matrix4[] matViewData;
        private float[] vertices;

        private GameObject objectToDraw;

        Matrix4 x = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(45.0f));

        //
        /// <summary>
        /// 
        /// </summary>
        // ADD matching vertexshader atribute IDs !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        // http://neokabuto.blogspot.com/2013/03/opentk-tutorial-2-drawing-triangle.html
        //
        //


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
            ProgramID = CreateProgram(VertexShaderPath, FragmentShaderPath);
            /*
            texture1 = new Texture2D(TexturePath);
            texture2 = new Texture2D(Prefix + "img.jpg");

            float[] vertices2 =
            {
                //Position          Texture coordinates
                 0.5f,  0.5f, 0.0f, 1.0f, 1.0f, // top right
                 0.5f, -0.5f, 0.0f, 1.0f, 0.0f, // bottom right
                -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, // bottom left
                -0.5f,  0.5f, 0.0f, 0.0f, 1.0f  // top left
            };

            GL.GenBuffers(1, out vboPosition);
            GL.NamedBufferStorage(vboPosition, vertices2.Length * sizeof(float), vertices2, 0);
            GL.GenVertexArrays(1, out vaoMain);

            GL.EnableVertexArrayAttrib(vaoMain, shaderAttribPosition);
            GL.VertexArrayVertexBuffer(vaoMain, shaderAttribPosition, vboPosition, IntPtr.Zero, sizeof(float) * 5);
            GL.VertexArrayAttribFormat(vaoMain, shaderAttribPosition, 3, VertexAttribType.Float, false, 0);
            GL.VertexArrayAttribBinding(vaoMain, shaderAttribPosition, shaderAttribPosition);

            GL.EnableVertexArrayAttrib(vaoMain, shaderAttribTexCoors);
            GL.VertexArrayVertexBuffer(vaoMain, shaderAttribTexCoors, vboPosition, IntPtr.Zero, sizeof(float) * 5);
            GL.VertexArrayAttribFormat(vaoMain, shaderAttribTexCoors, 2, VertexAttribType.Float, false, (sizeof(float) * 3));
            GL.VertexArrayAttribBinding(vaoMain, shaderAttribTexCoors, shaderAttribTexCoors);
            */

            matViewData = new Matrix4[]{
                //Matrix4.LookAt(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY),
                Matrix4.Identity
            };
             
            objectToDraw = new GameObject(Prefix + "test.obj", TexturePath, 
                shaderAttribPosition, shaderAttribTexCoors, shaderAttribNormals);
            
            GL.Enable(EnableCap.DepthTest);

            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            
        }


        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            Title = $"(Vsync: {VSync}) FPS: {1f / e.Time:0}";
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.DepthTest);

            GL.UseProgram(ProgramID);
            //GL.BindVertexArray(vaoMain);
            GL.ProgramUniformMatrix4(ProgramID, shaderUniformMatView, false, ref matViewData[0]);
            
            GL.ProgramUniformMatrix4(ProgramID, shaderUniformMatTrans, false, ref x);


            objectToDraw.Texture.Use(shaderUniformTextureSampler);
            objectToDraw.Draw();
            /*
            frameCount++;
            if (frameCount < 60 )
            {
                texture1.Use(shaderUniformTextureSampler);
                texture2.Use(shaderUniformTextureSampler2);
            }
            else if (frameCount < 120)
            {
                texture2.Use(shaderUniformTextureSampler);
                texture1.Use(shaderUniformTextureSampler2);
            }
            else 
            {
                frameCount = 0;
            }

            //GL.EnableVertexAttribArray(shaderAttribPosition);
            //GL.EnableVertexAttribArray(shaderAttribColor);
            GL.DrawArrays(PrimitiveType.TriangleFan, 0, 4);
            //GL.DisableVertexAttribArray(shaderAttribPosition);
            //GL.DisableVertexAttribArray(shaderAttribColor);
            */

            SwapBuffers();
        }




        






        /// <summary>
        /// NOT IMPORTANT
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            GL.DeleteBuffer(vboPosition);
            GL.DeleteBuffer(vboTexCoors);

            GL.DeleteVertexArray(vaoMain);
            GL.DeleteProgram(ProgramID);
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
    }
}
