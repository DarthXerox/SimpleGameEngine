using System;
using System.Collections.Generic;
using OpenGL_in_CSharp;
using OpenGL_in_CSharp.Utils;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using OpenGL_in_CSharp.TextRendering;
using System.IO;
using OpenGL_in_CSharp.Mesh_and_SceneObjects;

namespace GameNamespace
{
    public enum GameStates
    {
        PlayingGame,
        MainMenu,
        QuitMenu,
        QuitGame,
        YouWin,
        None // nothing changes
    }
    
    public class MainWindow : GameWindow
    {

        public GameStates GameState = GameStates.MainMenu; 
        public LightsProgram NormalMappingProg { private set; get; } // ID of the program
        public LightsProgram FakeNormalMappingProg { private set; get; }

        public ShaderProgram TextProgram { set; get; }

        public LightsProgram PostprocessProgram { set; get; }
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

        private Matrix4 matView;
        private Matrix4 matProjection;
        
        public Camera Camera { private set; get; }
        private Player player;
        private Map map;
        public Fog WorldFog;
        public Terrain Terrain { set; get; }

        private Light light;

        private Light lightForCoins { set; get; } 
        public SceneObject Tree { set; get; }
        public SceneObject TreeLeaves { set; get; }

        public CollisionManager CollisionManager { set; get; }

        public bool IsPlayerMoving { private set; get; } = true;
        private float counter = 0.0f;
        private int framecounter = 0;
        private int step = 2;

        public int framebuffer = 0;
        public int framebuffer_color = 0;
        public int framebuffer_depth = 0;

        public GUI QuitMenuGUI { set; get; }
        public GUI MainMenuGUI { set; get; }
        public GUI YouWinGUI { set; get; }

        public float[] clear_color = { 0.0f, 0.0f, 0.0f, 1.0f };
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

            WorldFog = new Fog(0.03f, new Vector3(0.2f, 0.2f, 0.2f));

            GL.ClearColor(WorldFog.Color.X, WorldFog.Color.Y, WorldFog.Color.Z, 1);
            clear_color = new float[] { WorldFog.Color.X, WorldFog.Color.Y, WorldFog.Color.Z, 1.0f };
            //GL.ClearColor(Color4.Aqua);
            NormalMappingProg = new LightsProgram(FilePaths.NormalMappingVert, FilePaths.NormalMappingFrag);
            FakeNormalMappingProg = new LightsProgram(FilePaths.FakeNormalMappingVert, FilePaths.NormalMappingFrag);
            //NormalMappingProgram = new ShaderProgram(FilePaths.VertexShaderPath, FilePaths.NormalMappingPath);
            light = new Light(new Vector3(10.0f, 10.0f, 5.0f)); //new Light(new Vector4(-0.5f, 0.75f, 0.5f, 1.0f));
            light.Color = new Vector3(1f, 1f, 1f);

            TextProgram = new ShaderProgram(FilePaths.TextVertex, FilePaths.TextFrag);
            PostprocessProgram = new LightsProgram(FilePaths.PostprocessVert, FilePaths.PostprocessFrag);
            Font = new FreeTypeFont(48, FilePaths.MonoFont);


            //objectToDraw2 = new SceneObject(FilePaths.ObjDragon, FilePaths.TexturePathRed);
            //objectToDraw2.ScalingFactor = 0.5f;

            map = new Map(5, 5, FilePaths.HeightMapPath);
            Console.WriteLine($"Height map size: X: {map.HeightMap.Width} Y: {map.HeightMap.Height}");
            //map = new Map(1, 1, FilePaths.TextureBrickWall, FilePaths.HeightMapPath);
            player = new Player(new Vector3(1, 5, 1), map);

           
            Camera = new Camera(
                new Vector3(0.0f, 20.0f, 20.0f),
                WorldOrigin,
                new Vector3(0.0f, 1.0f, 0.0f)
                );
            /*
            Camera = Camera.GenerateOmnipotentCamera(Vector3.Zero);*/
            CursorVisible = false;

            lightForCoins = new ConeLight(new Vector3(10 + 2 * 5, map.GetHeight(10 + 2 * 5, 5) + 8, 5), -Vector3.UnitY,
                1f, 0.07f, 0.017f, 12.5f, 17.5f);

            CollisionManager = new CollisionManager(player);
            map.SignUpForCollisionChecking(CollisionManager);
            //CollisionManager.CollisionChecking += map.Tree.OnCollisionCheck;
            //CollisionManager.CollisionChecking += map.TallGrass.OnCollisionCheck;

            //new AssimpMesh(FilePaths.ObjCube);
            //TreeLeaves = new NormalMappingMesh(FilePaths.ObjTreeLeaves, FilePaths.TextureTreeLeaves3, FilePaths.BumpTexTreeLeaves);
            /*
            Tree = new SceneObject(new NormalMappingMesh(FilePaths.ObjTreeTrunk,
                FilePaths.TextureTreeTrunk, FilePaths.MtlGold, FilePaths.BumpTexTrunk), new Vector3(10, 0, 10));

            TreeLeaves = new SceneObject(new NormalMappingMesh(FilePaths.ObjTreeLeaves, FilePaths.TextureTreeLeaves3,
                FilePaths.MtlGold, FilePaths.BumpTexTreeLeaves), new Vector3(10, 0, 10));

            Terrain = new Terrain(FilePaths.HeightMapPath, FilePaths.TextureGrass4, 
                 FilePaths.BumpTexGrass4, FilePaths.MtlGold, Vector3.Zero);
                 */
            GL.Enable(EnableCap.Multisample);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);

            InitGUI();

            GL.CreateFramebuffers(1, out framebuffer);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer);
            GL.CreateTextures(TextureTarget.Texture2D, 1, out framebuffer_color);
            GL.TextureStorage2D(framebuffer_color, 1, SizedInternalFormat.Rgba32f, Width, Height);

            GL.CreateTextures(TextureTarget.Texture2D, 1, out framebuffer_depth);
            GL.BindTexture(TextureTarget.Texture2D, framebuffer_depth);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent32f, Width, Height, 0,
                PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            // Set output 0 to GL_COLOR_ATTACHMENT0 (glNamedFramebufferDrawBuffers)
            DrawBuffersEnum[] draw_buffers = { DrawBuffersEnum.ColorAttachment0 };
            GL.NamedFramebufferDrawBuffers(framebuffer, 1, draw_buffers);

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

            NormalMappingProg.Use();

            Vector3 camPosition = IsPlayerMoving ? player.Position : Camera.Position;


            int lightIndex = 0;
            //NormalMappingProg.AttachLight(light, lightIndex);
            //FakeNormalMappingProg.AttachLight(light, lightIndex);
            //lightIndex++;
            NormalMappingProg.AttachLight(player.Flashlight, lightIndex);
            FakeNormalMappingProg.AttachLight(player.Flashlight, lightIndex);
            lightIndex++;
            NormalMappingProg.AttachLight(lightForCoins, lightIndex);
            FakeNormalMappingProg.AttachLight(lightForCoins, lightIndex);

            GL.ProgramUniform3(NormalMappingProg.ID, 5, camPosition);
            NormalMappingProg.AttachFog(WorldFog);
            NormalMappingProg.AttachViewMatrix(matView);
            NormalMappingProg.AttachProjectionMatrix(matProjection);

            GL.ProgramUniform3(FakeNormalMappingProg.ID, 5, camPosition);
            FakeNormalMappingProg.AttachFog(WorldFog);
            FakeNormalMappingProg.AttachViewMatrix(matView);
            FakeNormalMappingProg.AttachProjectionMatrix(matProjection);
            /*
            GL.ProgramUniform3(Program.ID, GL.GetUniformLocation(Program.ID, "materialAmbientColor"), objectMaterial.Ambient);
            GL.ProgramUniform3(Program.ID, 9, objectMaterial.Diffuse);
            GL.ProgramUniform3(Program.ID, 10, objectMaterial.Specular);
            GL.ProgramUniform1(Program.ID, 11, objectMaterial.Shininess);
           
            */
            //Program.AttachMaterial(Tree.RawMesh.Material);

            /*
            framecounter = (framecounter + 1) % 360;
            int val = 0;
            if (framecounter < 180)
            {
                val = 1;
            }
            */
            //GL.ProgramUniform1(Program.ID, 12, val);
            //Program.AttachDirectionalLight(light);
            //Program.AttachLight(player.Flashlight, lightIndex);
            /*
            NormalMappingProg.AttachLight(light, lightIndex);

            NormalMappingProg.AttachFog(WorldFog);
            NormalMappingProg.AttachViewMatrix(matView);
            NormalMappingProg.AttachProjectionMatrix(matProjection);
            Tree.Draw(NormalMappingProg);
            TreeLeaves.Draw(NormalMappingProg);


            FakeNormalMappingProg.Use();
            GL.ProgramUniform3(FakeNormalMappingProg.ID, 5, camPosition);
            FakeNormalMappingProg.AttachLight(light, lightIndex);
            FakeNormalMappingProg.AttachFog(WorldFog);
            FakeNormalMappingProg.AttachViewMatrix(matView);
            FakeNormalMappingProg.AttachProjectionMatrix(matProjection);
            */
            //Terrain.Draw(FakeNormalMappingProg);
            //map.DrawMap(Program, NormalMappingProgram);

            //Program.AttachModelMatrix(Matrix4.CreateTranslation(20, 0, 20));
            //AssMesh.Draw();


            //Program.AttachModelMatrix(Matrix4.CreateTranslation(20, 0, 20));

            map.DrawMap(NormalMappingProg, FakeNormalMappingProg, player);
        }


        protected override void OnRenderFrame(FrameEventArgs e)
        { 
            base.OnRenderFrame(e);
            Title = $"(Vsync: {VSync}) FPS: {1f / e.Time:0}";
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


            //RenderGame();

            
            if (GameState != GameStates.PlayingGame)
            {
                GL.BindFramebuffer(FramebufferTarget.FramebufferExt, framebuffer);
                //Console.WriteLine(GL.GetError());

                // Clear attachments
                GL.ClearNamedFramebuffer(framebuffer, ClearBuffer.Color, 0, clear_color);
                GL.ClearNamedFramebuffer(framebuffer, ClearBuffer.Depth, 0, clear_depth);

                RenderGame();

                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
                GL.ClearNamedFramebuffer(0, ClearBuffer.Color, 0, clear_color);

                GL.Disable(EnableCap.DepthTest);
                GL.Disable(EnableCap.Blend);
                GL.Disable(EnableCap.CullFace);

                PostprocessProgram.Use();
                GL.BindTexture(TextureTarget.Texture2D, framebuffer_color);
                GL.DrawArrays(PrimitiveType.Triangles, 0, 6);



                Matrix4 projectionM = Matrix4.CreateScale(new Vector3(1f / this.Width, 1f / this.Height, 1.0f));
                //projectionM = Matrix4.CreateOrthographicOffCenter(0.0f, Width, 0.0f, Height,  -1.0f, 1.0f);
                projectionM = Matrix4.CreateOrthographic(Width, Height, 1, -1);

                //GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
                //GL.Clear(ClearBufferMask.ColorBufferBit);

                GL.Disable(EnableCap.CullFace);
                GL.Enable(EnableCap.Blend);
                //GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
                GL.BlendFunc(0, BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

                TextProgram.Use();
                GL.UniformMatrix4(1, false, ref projectionM);

                if (GameState == GameStates.MainMenu)
                {
                    MainMenuGUI.Draw(GetMousePositionRelativeToWindowMiddle());
                } 
                else if (GameState == GameStates.YouWin)
                {
                    YouWinGUI.Draw(GetMousePositionRelativeToWindowMiddle());
                } 
                else if (GameState == GameStates.QuitMenu)
                {
                    QuitMenuGUI.Draw(GetMousePositionRelativeToWindowMiddle());
                }

                //GL.Uniform3(2, new Vector3(0.5f, 0.8f, 0.2f));
                //Font.RenderText("This is a sample text", 0f, 0f, 1f, new Vector3(0,0,0));

                //GL.Uniform3(2, new Vector3(0.3f, 0.7f, 0.9f));
                //Font.RenderText("(C) LearnOpenGL.com", 50.0f, 200.0f, 0.9f, new Vector3(1,1,0));

                //Console.Write("Mouse: " + GetMousePositionRelativeToWindowMiddle() + " ");
                //TextBox.Draw(GetMousePositionRelativeToWindowMiddle());
                //YesText.Draw(GetMousePositionRelativeToWindowMiddle());
                //NoText.Draw(GetMousePositionRelativeToWindowMiddle());
                //Console.Write($"Window origin X = {this.X}, and Y = {this.Y}  ");

                //Mouse.GetCursorState(). = 5;
                // Console.WriteLine("Mouse: " + GetMousePositionRelativeToWindoWMiddle());
                //Console.WriteLine("Relative to window middle: " + GetMousePositionRelativeToWindoWMiddle());

            }
            else if (GameState == GameStates.PlayingGame)
            {
                RenderGame();
            }
            
            
            
            
            

            
            
            
            



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

        /// <summary>
        /// Pauses the game when the window is put in the background
        /// </summary>
        protected override void OnFocusedChanged(EventArgs e)
        {
            if (GameState == GameStates.PlayingGame)
            {
                GameState = GameStates.QuitMenu;
            }
            base.OnFocusedChanged(e);
        }


        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (player.CoinsCollected >= Coins.AllCoinsCount)
            {
                GameState = GameStates.YouWin;
            }
            if (Mouse.GetState().IsAnyButtonDown) 
            {
                Vector2 mousePosMiddle = GetMousePositionRelativeToWindowMiddle();
                GameStates newState = GameStates.None;
                switch (GameState)
                {
                    case GameStates.MainMenu:
                        newState = MainMenuGUI.OnMouseClick(mousePosMiddle);
                        break;
                    case GameStates.QuitMenu:
                        newState = QuitMenuGUI.OnMouseClick(mousePosMiddle);
                        break;
                    case GameStates.YouWin:
                        newState = YouWinGUI.OnMouseClick(mousePosMiddle);
                        break;
                }
                if (newState != GameStates.None)
                {
                    GameState = newState;
                }
            }
            //CollisionManager.CheckCollisions();

            HandleKeyboard();
            if (GameState == GameStates.PlayingGame)
            {
                CursorVisible = false;
                if (IsPlayerMoving)
                {
                    player.Move(Mouse.GetState());
                    CollisionManager.CheckCollisions();
                }
                else
                {
                    Camera.Move(Mouse.GetState());
                }
            } 
            else if (GameState == GameStates.QuitGame)
            {
                Exit();
            } 
            else
            {
                CursorVisible = true;
            }
            base.OnRenderFrame(e);
        }

        private void HandleKeyboard()
        {
            var keyState = Keyboard.GetState();
            /*if (keyState.IsKeyDown(Key.Escape))
            {
                Exit();
            } */
            if (keyState.IsKeyDown(Key.F))
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
            else if (keyState.IsKeyDown(Key.Escape))
            {
                GameState = GameStates.QuitMenu;
            }
            /*
            else if (keyState.IsKeyDown(Key.N))
            {
                GameState = WindowStates.PlayingGame;
            }*/
            /*else if (keyState.IsKeyDown(Key.X))
            {
                IsPlayerMoving = !IsPlayerMoving;
            }*/
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            if (Focused && GameState == GameStates.PlayingGame) // check to see if the window is focused  
            {
                Mouse.SetPosition(X + Width / 2f, Y + Height / 2f);
            }

            base.OnMouseMove(e);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Width, Height);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer);
            GL.CreateTextures(TextureTarget.Texture2D, 1, out framebuffer_color);
            GL.TextureStorage2D(framebuffer_color, 1, SizedInternalFormat.Rgba32f, Width, Height);

            GL.CreateTextures(TextureTarget.Texture2D, 1, out framebuffer_depth);
            GL.BindTexture(TextureTarget.Texture2D, framebuffer_depth);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent32f, Width, Height, 0,
                PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            // Set output 0 to GL_COLOR_ATTACHMENT0 (glNamedFramebufferDrawBuffers)
            DrawBuffersEnum[] draw_buffers = { DrawBuffersEnum.ColorAttachment0 };
            GL.NamedFramebufferDrawBuffers(framebuffer, 1, draw_buffers);

            GL.NamedFramebufferTexture(framebuffer, FramebufferAttachment.ColorAttachment0, framebuffer_color, 0);
            GL.NamedFramebufferTexture(framebuffer, FramebufferAttachment.DepthAttachment, framebuffer_depth, 0);

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
            {
                Console.WriteLine("ERROR::FRAMEBUFFER:: Framebuffer is not complete!");
                Console.WriteLine(GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer));
            }
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public Vector2 GetMousePositionRelativeToWindowMiddle()
        {
            var mouse = Mouse.GetCursorState();
            return new Vector2()
            {
                X = mouse.X - (this.X + Width / 2),
                Y =  - (mouse.Y - (this.Y + Height / 2))
            };
        }
        public Vector2 GetMousePositionRelativeToWindowOrigin()
        {
            var mouse = Mouse.GetCursorState();
            return new Vector2()
            {
                X = mouse.X - this.X,
                Y = mouse.Y - this.Y
            };
        }

        public void InitGUI()
        {
            MainMenuGUI = new GUI(new Dictionary<TextBox, GameStates>()
            {
                // Rendering a slightly bigger text behind a smaller creates a nice little "shadowy effect"
                { new TextBox(0, 200, "FOREST", 2.2f, new Vector3(0f, 00f, 0f), Font, false), GameStates.None },
                { new TextBox(0, 200, "FOREST", 2.0f, new Vector3(0f, 0.5f, 0f), Font, false), GameStates.None },

                { new TextBox(0, 80, "PLAY", 0.5f, new Vector3(1f), Font), GameStates.PlayingGame },
                { new TextBox(0, 0, "QUIT", 0.5f, new Vector3(1f), Font), GameStates.QuitMenu }
            });

            QuitMenuGUI = new GUI(new Dictionary<TextBox, GameStates>()
            {
                { new TextBox(0, 120, "Do you want to quit the game?", 0.5f, new Vector3(1f), Font, false), GameStates.None },
                { new TextBox(-60, -60, "YES", 0.5f, new Vector3(1f), Font), GameStates.QuitGame },
                { new TextBox(60, -60, "NO", 0.5f, new Vector3(1f), Font), GameStates.PlayingGame }
            });

            YouWinGUI = new GUI(new Dictionary<TextBox, GameStates>()
            {
                { new TextBox(0, 200, "YOU WIN!", 1.7f, new Vector3(0f), Font, false), GameStates.None },
                { new TextBox(0, 200, "YOU WIN!", 1.5f, new Vector3(0f, 0.5f, 0f), Font, false), GameStates.None },
                { new TextBox(0, 0, "QUIT", 0.5f, new Vector3(1f), Font), GameStates.QuitGame }
            });
        }

    }
}
