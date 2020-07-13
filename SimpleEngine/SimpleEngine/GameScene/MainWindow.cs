using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using SimpleEngine.WorldObjects;
using SimpleEngine.Collisions;
using SimpleEngine.Text;
using SimpleEngine.Utils;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using SimpleEngine.Data;

namespace SimpleEngine.GameScene
{

    ///
    /// <summary>
    /// Check out later:
    /// http://luiscubal.blogspot.com/2013/04/asynchronous-opengl-texture-loading.html
    /// </summary>
    /// 


    public class MainWindow : GameWindow
    {
        private int framebuffer = 0;
        private int framebufferColor = 0;
        private int framebufferDepth = 0;
        private bool isFirstFrame = true;
        private DateTime loadTime;

        public GameStates GameState = GameStates.MainMenu;
        public LightsProgram NormalMappingProg { private set; get; } // ID of the program
        public LightsProgram FakeNormalMappingProg { private set; get; }
        public ShaderProgram TextProgram { set; get; }
        public LightsProgram PostprocessProgram { set; get; }
        public FreeTypeFont Font { set; get; }


        public Matrix4 ViewMatrix { private set; get; }
        public Matrix4 ProjectionMatrix { private set; get; }

        public Camera Camera { private set; get; }
        public Player Player { private set; get; }
        public World Map { private set; get; }
        public Fog WorldFog { private set; get; }

        public Light Sun { private set; get; }

        public CollisionManager CollisionManager { set; get; }

        //enables to switch between camera and player
        public bool IsPlayerMoving { private set; get; } = true;



        public GUI QuitMenuGUI { set; get; }
        public GUI MainMenuGUI { set; get; }
        public GUI YouWinGUI { set; get; }
        public GUI HelpGUI { set; get; }

        public float[] clear_color = { 0.0f, 0.0f, 0.0f, 1.0f };
        public readonly float[] clear_depth = { 1.0f };

        // make it possible to delete
        Task<ConcurrentDictionary<string, Bitmap>> Textures = DataLoader.LoadAllBitmapsAsync();
        Task<ConcurrentDictionary<string, ObjModel>> Models = DataLoader.LoadAllObjModelsWithTangentsAsync();

        //ConcurrentDictionary<string, Bitmap> TexSave;
        //Task<Dictionary<string, Bitmap>> Textures = DataLoader.LoadAllBitmapsAsync();

        public MainWindow()
           : base(900, // 800, 720
            720,
            GraphicsMode.Default,
            "Forest",
            GameWindowFlags.Default,
            DisplayDevice.Default,
            4,
            0,
            GraphicsContextFlags.ForwardCompatible)
        {
            loadTime = DateTime.Now;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Console.WriteLine((DateTime.Now - loadTime).TotalSeconds + "s");
            GL.Viewport(0, 0, Width, Height);

            WorldFog = new Fog(0.03f, new Vector3(0.2f, 0.2f, 0.2f));
            GL.ClearColor(WorldFog.Color.X, WorldFog.Color.Y, WorldFog.Color.Z, 1);
            clear_color = new float[] { WorldFog.Color.X, WorldFog.Color.Y, WorldFog.Color.Z, 1.0f };

            //var watches = System.Diagnostics.Stopwatch.StartNew();
            NormalMappingProg = new LightsProgram(FilePaths.NormalMappingVert, FilePaths.NormalMappingFrag);
            FakeNormalMappingProg = new LightsProgram(FilePaths.FakeNormalMappingVert, FilePaths.NormalMappingFrag);
            TextProgram = new ShaderProgram(FilePaths.TextVertex, FilePaths.TextFrag);
            PostprocessProgram = new LightsProgram(FilePaths.PostprocessVert, FilePaths.PostprocessFrag);
            //Console.WriteLine($"Total programs load time: {watches.ElapsedMilliseconds}ms");

            Font = new FreeTypeFont(48, FilePaths.FontMono);

            Sun = new Light(new Vector3(10.0f, 10.0f, 5.0f))
            {
                Color = new Vector3(0.3f, 0.3f, 0.3f)
            };

            //Console.WriteLine("Before map init time: " + (DateTime.Now - loadTime).TotalSeconds + "s");




            // used for moving camera, this feature can only be turned on in the code see IsPlayerMoving
            Camera = new Camera(
                new Vector3(0.0f, 20.0f, 20.0f),
                Vector3.Zero,
                new Vector3(0.0f, 1.0f, 0.0f)
                );

            CursorVisible = false;



            InitGUI();

            //just the postprocess stuff from 7th exercise
            GL.CreateFramebuffers(1, out framebuffer);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer);
            GL.CreateTextures(TextureTarget.Texture2D, 1, out framebufferColor);
            GL.TextureStorage2D(framebufferColor, 1, SizedInternalFormat.Rgba32f, Width, Height);

            GL.CreateTextures(TextureTarget.Texture2D, 1, out framebufferDepth);
            GL.BindTexture(TextureTarget.Texture2D, framebufferDepth);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent32f, Width, Height, 0,
                PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            DrawBuffersEnum[] draw_buffers = { DrawBuffersEnum.ColorAttachment0 };
            GL.NamedFramebufferDrawBuffers(framebuffer, 1, draw_buffers);

            GL.NamedFramebufferTexture(framebuffer, FramebufferAttachment.ColorAttachment0, framebufferColor, 0);
            GL.NamedFramebufferTexture(framebuffer, FramebufferAttachment.DepthAttachment, framebufferDepth, 0);

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
            {
                Console.WriteLine("ERROR::FRAMEBUFFER:: Framebuffer is not complete!");
                Console.WriteLine(GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer));
            }
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);

            var watches = System.Diagnostics.Stopwatch.StartNew();
            //Map = new Map(2, 2, FilePaths.HeightMapPath);
            //Map = new Map(2, 2, new ConcurrentDictionary<string, Bitmap>(Textures.Result));
            Map = new World(2, 2, Textures.Result, Models.Result);

            //Textures.Result.Clear();
            Console.WriteLine($"Total map load time: {watches.ElapsedMilliseconds}ms");

            Player = new Player(new Vector3(1, 5, 1), Map);
            CollisionManager = new CollisionManager(Player);
            Map.SignUpForCollisionChecking(CollisionManager);

            Console.WriteLine("Load end time: " + (DateTime.Now - loadTime).TotalSeconds + "s");
        }


        private void RenderGame()
        {
            GL.Enable(EnableCap.DepthTest);
            ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(55.0f), (float)Width / Height, 0.1f, 1000.0f);
            ViewMatrix = IsPlayerMoving ? Player.GetViewMatrix() : Camera.GetViewMatrix();
            Vector3 camPosition = IsPlayerMoving ? Player.Position : Camera.Position;

            int lightIndex = 0;
            NormalMappingProg.AttachLight(Sun, lightIndex);
            FakeNormalMappingProg.AttachLight(Sun, lightIndex);
            lightIndex++;
            NormalMappingProg.AttachLight(Player.Flashlight, lightIndex);
            FakeNormalMappingProg.AttachLight(Player.Flashlight, lightIndex);
            lightIndex++;

            GL.ProgramUniform3(NormalMappingProg.ID, 5, camPosition);
            NormalMappingProg.AttachFog(WorldFog);
            NormalMappingProg.AttachViewMatrix(ViewMatrix);
            NormalMappingProg.AttachProjectionMatrix(ProjectionMatrix);

            GL.ProgramUniform3(FakeNormalMappingProg.ID, 5, camPosition);
            FakeNormalMappingProg.AttachFog(WorldFog);
            FakeNormalMappingProg.AttachViewMatrix(ViewMatrix);
            FakeNormalMappingProg.AttachProjectionMatrix(ProjectionMatrix);

            Map.DrawMap(NormalMappingProg, FakeNormalMappingProg, Player);
        }


        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            Title = $"(Vsync: {VSync}) FPS: {1f / e.Time:0}";
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if (GameState != GameStates.PlayingGame)
            {
                GL.BindFramebuffer(FramebufferTarget.FramebufferExt, framebuffer);

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
                GL.BindTexture(TextureTarget.Texture2D, framebufferColor);
                GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

                // for rendering text we use orthographic projection
                Matrix4 projectionM = Matrix4.CreateOrthographic(Width, Height, 1, -1);


                GL.Disable(EnableCap.CullFace);
                GL.Enable(EnableCap.Blend);
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
                else if (GameState == GameStates.HelpMenu)
                {
                    HelpGUI.Draw(GetMousePositionRelativeToWindowMiddle());
                }
                else if (GameState == GameStates.QuitMenu)
                {
                    QuitMenuGUI.Draw(GetMousePositionRelativeToWindowMiddle());
                    Player.DrawCollectedStonesGUI(Width, Height, Font);
                }
            }
            else if (GameState == GameStates.PlayingGame)
            {
                RenderGame();

                TextProgram.Use();
                Player.DrawCollectedStonesGUIWithTime(Width, Height, Font);
            }

            SwapBuffers();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
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
            if (isFirstFrame)
            {
                Console.WriteLine("Time in seconds:" + (DateTime.Now - loadTime).TotalSeconds);
                isFirstFrame = false;
            }
            if (Player.StonesCollected >= FloatingStone.AllCoinsCount)
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
                    case GameStates.HelpMenu:
                        newState = HelpGUI.OnMouseClick(mousePosMiddle);
                        break;
                }
                if (newState != GameStates.None)
                {
                    GameState = newState;
                }
            }

            HandleKeyboard();
            if (GameState == GameStates.PlayingGame)
            {
                CursorVisible = false;
                if (IsPlayerMoving)
                {
                    Player.Move(Mouse.GetState());
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

            //on resize also the framebuffer size is changed so we need to change the postprocess framebuffer too
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer);
            GL.CreateTextures(TextureTarget.Texture2D, 1, out framebufferColor);
            GL.TextureStorage2D(framebufferColor, 1, SizedInternalFormat.Rgba32f, Width, Height);

            GL.CreateTextures(TextureTarget.Texture2D, 1, out framebufferDepth);
            GL.BindTexture(TextureTarget.Texture2D, framebufferDepth);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent32f, Width, Height, 0,
                PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            DrawBuffersEnum[] draw_buffers = { DrawBuffersEnum.ColorAttachment0 };
            GL.NamedFramebufferDrawBuffers(framebuffer, 1, draw_buffers);

            GL.NamedFramebufferTexture(framebuffer, FramebufferAttachment.ColorAttachment0, framebufferColor, 0);
            GL.NamedFramebufferTexture(framebuffer, FramebufferAttachment.DepthAttachment, framebufferDepth, 0);

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
            {
                Console.WriteLine("ERROR::FRAMEBUFFER:: Framebuffer is not complete!");
                Console.WriteLine(GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer));
            }
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        /// <summary>
        /// Gets mouse position relative to the very middle of the game window screen
        /// this is useful for text rendering - Matrix4.CreateOrthographicOffCenter
        /// </summary>
        /// <returns></returns>
        public Vector2 GetMousePositionRelativeToWindowMiddle()
        {
            var mouse = Mouse.GetCursorState();
            return new Vector2()
            {
                X = mouse.X - (X + Width / 2),
                Y = -(mouse.Y - (Y + Height / 2))
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
                { new TextBox(0, -20, "HELP", 0.5f, new Vector3(1f), Font), GameStates.HelpMenu },
                { new TextBox(0, -120, "QUIT", 0.5f, new Vector3(1f), Font), GameStates.QuitMenu }

            });

            HelpGUI = new GUI(new Dictionary<TextBox, GameStates>()
            {
                // Rendering a slightly bigger text behind a smaller creates a nice little "shadowy effect"
                { new TextBox(0, 200, "Collect 5 floating rocks to win", 0.5f, new Vector3(1f), Font, false), GameStates.None },
                { new TextBox(0, 100, "mouse - look around", 0.5f, new Vector3(1f), Font, false), GameStates.None },
                { new TextBox(0, 150, "W, A, S, D - walk", 0.5f, new Vector3(1f), Font, false), GameStates.None },
                { new TextBox(0, 50, "E - turn on/off flashlight", 0.5f, new Vector3(1f), Font, false), GameStates.None },

                { new TextBox(0, -80, "Back", 0.5f, new Vector3(1f), Font), GameStates.MainMenu }
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
                { new TextBox(0, -100, "QUIT", 0.5f, new Vector3(1f), Font), GameStates.QuitGame }
            });
        }
    }
}
