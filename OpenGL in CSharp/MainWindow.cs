using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using OpenGL_in_CSharp;
using OpenGL_in_CSharp.Utils;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using OpenTK_example_5;
using RangeTree;

namespace GameNamespace
{
    // More interesting tutorials
    // http://neokabuto.blogspot.com/p/tutorials.html
    public class MainWindow : GameWindow
    {
        public SimpleProgram Program { private set; get; } // ID of the program
        public SimpleProgram NormalMappingProgram { private set; get; }

        public AbstractShaderProgram TextProgram { set; get; }

        public SimpleProgram PostprocessProgram { set; get; }
        public Texture2D Background { set; get; }
        public FreeTypeFont Font { set; get; }
        
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
        private Player player;

        private SceneObject objectToDraw;
        private SceneObject objectToDraw2;
        private NormalMappingMesh AssMesh;
        private NormalMappingMesh TreeLeaves;
        private Map map;
        public Texture2D BricksNormal { set;  get; }
        public Fog WorldFog;

        private Material objectMaterial;

        private SceneObject[] animation = new SceneObject[25];


        private Light light;
        //private readonly ConeLight coneLight = new ConeLight(new Vector4(5f, 5f, 5f, 1), new Vector3(1, 1, 1), new Vector3(0, -1, 0), 10);

        public CollisionManager CollisionManager { set; get; }

        public bool IsPlayerMoving { private set; get; } = true;
        public bool DrawOnlyText { get; } = true;

        private float counter = 0.0f;
        private int framecounter = 0;
        private int step = 2;

        public int framebuffer = 0;
        public int framebuffer_color = 0;
        public int framebuffer_depth = 0;

        public bool IsPause { private set; get; } = false;
        public bool FirstPauseFrame { private set; get; } = true;


        public readonly float[] clear_color = { 0.0f, 0.0f, 0.0f, 1.0f };
        public readonly float[] clear_depth = { 1.0f };

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

            WorldFog = new Fog(0.03f, new Vector3(0.5f, 0.5f, 0.5f));

            //GL.ClearColor(WorldFog.Color.X, WorldFog.Color.Y, WorldFog.Color.Z, 1);
            GL.ClearColor(Color4.Aqua);
            Program = new SimpleProgram(FilePaths.VertexShaderPath, FilePaths.FragmentShaderPath);
            //NormalMappingProgram = new ShaderProgram(FilePaths.VertexShaderPath, FilePaths.NormalMappingPath);
            light = new Light(new Vector3(10.0f, 10.0f, 5.0f)); //new Light(new Vector4(-0.5f, 0.75f, 0.5f, 1.0f));
            light.Color = new Vector3(1f, 1f, 1f);

            TextProgram = new AbstractShaderProgram(FilePaths.TextVertex, FilePaths.TextFrag);
            PostprocessProgram = new SimpleProgram(FilePaths.PostprocessVert, FilePaths.PostprocessFrag);
            Background = new Texture2D(FilePaths.TextureBrickWall);
            Font = new FreeTypeFont(48);
            /*
            objectToDraw = new SceneObject(FilePaths.ObjDragon, FilePaths.TexturePathRed, 
                shaderAttribPosition, shaderAttribTexCoors, shaderAttribNormals, shaderUniformTextureSampler);

            objectMaterial = MtlParser.ParseMtl(FilePaths.MtlGold)[1];
            */
            objectToDraw = new Terrain(FilePaths.TexturePath);
            objectMaterial = MtlParser.ParseMtl(FilePaths.MtlGold)[0];
            objectToDraw.RotX = 30.0f;
            objectToDraw.RotY = 20.0f;
            objectToDraw.Position = new Vector3(0.0f, -3.0f, 0.0f);

            objectToDraw2 = new SceneObject(FilePaths.ObjDragon, FilePaths.TexturePathRed);
            objectToDraw2.ScalingFactor = 0.5f;

            map = new Map(2, 2, FilePaths.TexturePathGrass2, FilePaths.HeightMapPath);
            //map = new Map(1, 1, FilePaths.TextureBrickWall, FilePaths.HeightMapPath);
            BricksNormal = new Texture2D(FilePaths.BumpTexBrickWall);
            player = new Player(new Vector3(1, 0, 1), map);

           
            Camera = new Camera(
                new Vector3(0.0f, 20.0f, 20.0f),
                WorldOrigin,
                new Vector3(0.0f, 1.0f, 0.0f)
                );
            /*
            Camera = Camera.GenerateOmnipotentCamera(Vector3.Zero);*/
            CursorVisible = false;


            CollisionManager = new CollisionManager(player);
            map.SignUpForCollisionChecking(CollisionManager);
            //CollisionManager.CollisionChecking += map.Tree.OnCollisionCheck;
            //CollisionManager.CollisionChecking += map.TallGrass.OnCollisionCheck;

            //new AssimpMesh(FilePaths.ObjCube);
            AssMesh = new NormalMappingMesh(FilePaths.ObjTreeTrunk, FilePaths.TextureTreeTrunk, FilePaths.BumpTexTrunk);
            TreeLeaves = new NormalMappingMesh(FilePaths.ObjTreeLeaves, FilePaths.TextureTreeLeaves3, FilePaths.BumpTexTreeLeaves);

            GL.Enable(EnableCap.Multisample);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);



            GL.CreateFramebuffers(1, out framebuffer);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer);

            // Initialize color output texture with GL_RGBA32F format (glCreateTextures, glTextureStorage2D)
            // ...
            GL.CreateTextures(TextureTarget.Texture2D, 1, out framebuffer_color);
            //glBindTexture(GL_TEXTURE_2D, framebuffer_color);GL_RGBA32F

            GL.TextureStorage2D(framebuffer_color, 1, SizedInternalFormat.Rgba32f, Width, Height);

            // Initialize depth output texture with GL_DEPTH_COMPONENT32F format
            // ...
            GL.CreateTextures(TextureTarget.Texture2D, 1, out framebuffer_depth);
            //glBindTexture(GL_TEXTURE_2D, framebuffer_depth);GL_DEPTH_COMPONENT32F
            GL.BindTexture(TextureTarget.Texture2D, framebuffer_depth);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent32f, Width, Height, 0,
                PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            //GL.TextureStorage2D(framebuffer_depth, 1, SizedInternalFormat.R32f, Width, Height);



            // Set output 0 to GL_COLOR_ATTACHMENT0 (glNamedFramebufferDrawBuffers)
            DrawBuffersEnum[] draw_buffers = { DrawBuffersEnum.ColorAttachment0 };
            // ...
            GL.NamedFramebufferDrawBuffers(framebuffer, 1, draw_buffers);

            // Associate color and depth `attachments`(GL_COLOR_ATTACHMENT0,GL_DEPTH_ATTACHMENT)
            // with (framebuffer_)color and (framebuffer_)depth `textures` (glNamedFramebufferTexture)
            GL.NamedFramebufferTexture(framebuffer, FramebufferAttachment.ColorAttachment0, framebuffer_color, 0);
            GL.NamedFramebufferTexture(framebuffer, FramebufferAttachment.DepthAttachment, framebuffer_depth, 0);

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
            {
                Console.WriteLine("ERROR::FRAMEBUFFER:: Framebuffer is not complete!");
                Console.WriteLine(GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer));
            }
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }


        /// <summary>
        /// Takes care of rendering a running game
        /// </summary>
        private void RenderGame()
        {
            GL.Enable(EnableCap.DepthTest);
            matProjection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(55.0f), (float)Width / Height, 0.1f, 1000.0f);
            matView = IsPlayerMoving ? player.GetViewMatrix() : Camera.GetViewMatrix();

            Program.Use();

            Vector3 camPosition = IsPlayerMoving ? player.Position : Camera.Position;
            GL.ProgramUniform3(Program.ID, 5, camPosition);
            GL.ProgramUniform3(Program.ID, GL.GetUniformLocation(Program.ID, "materialAmbientColor"), objectMaterial.Ambient);
            GL.ProgramUniform3(Program.ID, 9, objectMaterial.Diffuse);
            GL.ProgramUniform3(Program.ID, 10, objectMaterial.Specular);
            GL.ProgramUniform1(Program.ID, 11, objectMaterial.Shininess);
            
            /*
            GL.ProgramUniform4(Program.ID, GL.GetUniformLocation(Program.ID, "lightPosition"), light.Position);
            
            GL.ProgramUniform3(Program.ID, GL.GetUniformLocation(Program.ID, "lightColor"), light.Color);
            */

            framecounter = (framecounter + 1) % 360;
            int val = 0;
            if (framecounter < 180)
            {
                val = 1;
            }
            
            //GL.ProgramUniform1(Program.ID, 12, val);
            //Program.AttachDirectionalLight(light);
            int lightIndex = 0;
            Program.AttachLight(player.Flashlight, lightIndex);
            lightIndex++;
            Program.AttachLight(light, lightIndex);
            lightIndex++;



            Program.AttachFog(WorldFog);
            //Program.AttachModelMatrix(animation[0].GetModelMatrix());

            //BricksNormal.Use(1);
            Program.AttachViewMatrix(matView);
            Program.AttachProjectionMatrix(matProjection);
            map.DrawMap(Program, NormalMappingProgram);

            Program.AttachModelMatrix(Matrix4.CreateTranslation(20, 0, 20));
            AssMesh.Draw();
            //Program.AttachModelMatrix(Matrix4.CreateTranslation(20, 0, 20));
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        { 
            //base.OnRenderFrame(e);
            Title = $"(Vsync: {VSync}) FPS: {1f / e.Time:0}";
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            ///GL.Disable(EnableCap.Blend);
            //GL.UseProgram(PostprocessProgram.)
            //PostprocessProgram.Use();
            //GL.BindTexture(TextureTarget.Texture2D0, Background.ID);
            //Background.Use(0);
            //objectToDraw2.Draw();
            //GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
            
            if (IsPause && !FirstPauseFrame)
            {
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
                GL.ClearNamedFramebuffer(0, ClearBuffer.Color, 0, clear_color);

                GL.Disable(EnableCap.DepthTest);
                GL.Disable(EnableCap.Blend);
                GL.Disable(EnableCap.CullFace);

                PostprocessProgram.Use();
                GL.BindTexture(TextureTarget.Texture2D, Background.ID);
                GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            }
            else if (IsPause)
            {
                GL.BindFramebuffer(FramebufferTarget.FramebufferExt, framebuffer);
                //Console.WriteLine(GL.GetError());

                // Clear attachments
                GL.ClearNamedFramebuffer(framebuffer, ClearBuffer.Color, 0, clear_color);
                GL.ClearNamedFramebuffer(framebuffer, ClearBuffer.Depth, 0, clear_depth);

                RenderGame();
                FirstPauseFrame = false;

                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
                GL.ClearNamedFramebuffer(0, ClearBuffer.Color, 0, clear_color);

                GL.Disable(EnableCap.DepthTest);
                GL.Disable(EnableCap.Blend);
                GL.Disable(EnableCap.CullFace);

                PostprocessProgram.Use();
                GL.BindTexture(TextureTarget.Texture2D, framebuffer_color);
                GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            }
            else
            {
                RenderGame();
            }
            
            
            /*
            if (!DrawOnlyText)
            {
                Matrix4 projectionM = Matrix4.CreateScale(new Vector3(1f / this.Width, 1f / this.Height, 1.0f));
                //projectionM = Matrix4.CreateOrthographicOffCenter(0.0f, this.Width, Height, 0.0f,  -1.0f, 1.0f);
                projectionM = Matrix4.CreateOrthographic(Width, Height, 1, -1);

                //GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
                //GL.Clear(ClearBufferMask.ColorBufferBit);

                GL.Disable(EnableCap.CullFace);
                GL.Enable(EnableCap.Blend);
                //GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
                GL.BlendFunc(0, BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

                TextProgram.Use();
                GL.UniformMatrix4(1, false, ref projectionM);

                GL.Uniform3(2, new Vector3(0.5f, 0.8f, 0.2f));
                Font.RenderText("This is a sample text",0f, 0f, 1f, new Vector2(0f, 0f));

                GL.Uniform3(2, new Vector3(0.3f, 0.7f, 0.9f));
                Font.RenderText("(C) LearnOpenGL.com", 50.0f, 200.0f, 0.9f, new Vector2(1.0f, 0));

                Font.RenderText("Loooooooooooool", new ModelTransformations() { 
                    Position = new Vector3(100, 80, 0)
                });

            }
            */
            
            



            /*
            Program.AttachModelMatrix(Matrix4.Identity);
            objectToDraw.Draw();
            */
            /*
            Program.AttachModelMatrix(objectToDraw2.GetModelMatrix());
            objectToDraw2.Draw();
            */
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
            if (IsPlayerMoving)
            {
                player.Move(Mouse.GetState());
                CollisionManager.CheckCollisions();
                //coneLight.Position = new Vector4(player.Position, 1f);
                //coneLight.Direction = player.Front;
            } else
            {
                Camera.Move(Mouse.GetState());
            }

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
            else if (keyState.IsKeyDown(Key.M))
            {
                IsPause = true;
            }
            else if (keyState.IsKeyDown(Key.N))
            {
                IsPause = false;
                FirstPauseFrame = true;
            }
            /*else if (keyState.IsKeyDown(Key.X))
            {
                IsPlayerMoving = !IsPlayerMoving;
            }*/
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            if (Focused) // check to see if the window is focused  
            {
                Mouse.SetPosition(X + Width / 2f, Y + Height / 2f);
            }

            base.OnMouseMove(e);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
        }
    }
}
