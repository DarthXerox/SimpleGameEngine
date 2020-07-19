using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using SimpleEngine.Collisions;
using SimpleEngine.Data;
using SimpleEngine.Text;
using SimpleEngine.Utils;
using SimpleEngine.WorldObjects;

namespace SimpleEngine.GameScene
{
    public class MainWindow : GameWindow
    {
        private int framebuffer;
        private int framebufferTexColor;
        private int framebufferTexDepth;
        private bool isFirstFrame = true;
        public readonly Task<ConcurrentDictionary<string, Bitmap>> TexturesTask = DataLoader.LoadAllBitmapsAsync();
        public readonly Task<ConcurrentDictionary<string, ObjModel>> ModelsTask = DataLoader.LoadAllObjModelsWithTangentsAsync();
        public readonly Task<ConcurrentDictionary<string, Material>> MaterialsTask = DataLoader.LoadAllMaterialsAsync();
        public GameStates GameState { private set; get; } = GameStates.MainMenu;
        public Matrix4 ViewMatrix { private set; get; }
        public Matrix4 ProjectionMatrix { private set; get; }
        public LightsProgram NormalMappingProg { private set; get; } // ID of the program
        public LightsProgram FakeNormalMappingProg { private set; get; }
        public ShaderProgram TextProgram { set; get; }
        public LightsProgram PostprocessProgram { set; get; }
        public Camera Camera { private set; get; }
        public Player Player { private set; get; }
        public World World { private set; get; }
        public Fog WorldFog { private set; get; }
        public Light Moon { private set; get; }
        public CollisionManager CollisionManager { set; get; }

        //enables to switch  between omnipotent camera and player
        public bool IsPlayerMoving { get; } = true;
        public FreeTypeFont Font { set; get; }
        public GUI QuitMenuGUI { set; get; }
        public GUI MainMenuGUI { set; get; }
        public GUI YouWinGUI { set; get; }
        public GUI HelpGUI { set; get; }
        public float[] ClearColor { private set; get; } = { 0.0f, 0.0f, 0.0f, 1.0f };
        public float[] ClearDepth { get; } = { 1.0f };

        public MainWindow()
           : base(900, 
            720,
            GraphicsMode.Default,
            "Forest",
            GameWindowFlags.Default,
            DisplayDevice.Default,
            4,
            0,
            GraphicsContextFlags.ForwardCompatible)
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            GL.Viewport(0, 0, Width, Height);

            WorldFog = new Fog(0.03f, new Vector3(0.2f, 0.2f, 0.2f));
            GL.ClearColor(WorldFog.Color.X, WorldFog.Color.Y, WorldFog.Color.Z, 1);
            ClearColor = new float[] { WorldFog.Color.X, WorldFog.Color.Y, WorldFog.Color.Z, 1.0f };

            NormalMappingProg = new LightsProgram(FilePaths.NormalMappingVert, FilePaths.NormalMappingFrag);
            FakeNormalMappingProg = new LightsProgram(FilePaths.FakeNormalMappingVert, FilePaths.NormalMappingFrag);
            TextProgram = new ShaderProgram(FilePaths.TextVertex, FilePaths.TextFrag);
            PostprocessProgram = new LightsProgram(FilePaths.PostprocessVert, FilePaths.PostprocessFrag);

            Font = new FreeTypeFont(48, FilePaths.FontMono);

            Moon = new Light(new Vector3(10.0f, 10.0f, 5.0f))
            {
                Color = new Vector3(0.3f, 0.3f, 0.3f)
            };

            // used for moving camera, this feature can only be turned on in the code, see IsPlayerMoving
            if (!IsPlayerMoving)
            {
                Camera = new Camera(
                    new Vector3(0.0f, 20.0f, 20.0f),
                    new Vector3(0.0f, 1.0f, 0.0f)
                );
            }

            CursorVisible = false; // we want to hide the cursor while the game is being played

            InitGUI();

            // To be able to draw letters (GUI) over the background (game running)
            // we have to render the background not directly to window screen but to a separate framebuffer 
            // Here we create this separate buffer
            GL.CreateFramebuffers(1, out framebuffer);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer);
            GL.CreateTextures(TextureTarget.Texture2D, 1, out framebufferTexColor);
            GL.TextureStorage2D(framebufferTexColor, 1, SizedInternalFormat.Rgba32f, Width, Height);

            GL.CreateTextures(TextureTarget.Texture2D, 1, out framebufferTexDepth);
            GL.BindTexture(TextureTarget.Texture2D, framebufferTexDepth);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent32f, Width, Height, 0,
                PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.NamedFramebufferDrawBuffers(framebuffer, 1, new[] { DrawBuffersEnum.ColorAttachment0 });

            GL.NamedFramebufferTexture(framebuffer, FramebufferAttachment.ColorAttachment0, framebufferTexColor, 0);
            GL.NamedFramebufferTexture(framebuffer, FramebufferAttachment.DepthAttachment, framebufferTexDepth, 0);

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
            {
                Console.WriteLine("ERROR::FRAMEBUFFER:: Framebuffer is not complete!");
                Console.WriteLine(GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer));
            }
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace); 
            GL.CullFace(CullFaceMode.Back); // makes sure only the side (of a polygon) facing the camera is rendered

            var watches = System.Diagnostics.Stopwatch.StartNew();
            World = new World(2, 2, TexturesTask.Result, ModelsTask.Result, MaterialsTask.Result);

            Player = new Player(new Vector3(50, 5, 50), World);
            CollisionManager = new CollisionManager(Player);
            World.SignUpForCollisionChecking(CollisionManager);
        }


        private void RenderGame()
        {
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(55.0f), (float)Width / Height, 0.1f, 1000.0f);
            ViewMatrix = IsPlayerMoving ? Player.GetViewMatrix() : Camera.GetViewMatrix();
            Vector3 camPosition = IsPlayerMoving ? Player.Position : Camera.Position;

            int lightIndex = 0;
            NormalMappingProg.AttachLight(Moon, lightIndex);
            FakeNormalMappingProg.AttachLight(Moon, lightIndex);
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

            World.DrawMap(NormalMappingProg, FakeNormalMappingProg, Player);
        }


        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            Title = $"Forest FPS: {1f / e.Time:0}";
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if (GameState != GameStates.PlayingGame)
            {
                // Now we prepare a separate clear framebuffer and draw the game to it ... 
                GL.BindFramebuffer(FramebufferTarget.FramebufferExt, framebuffer);
                GL.ClearNamedFramebuffer(framebuffer, ClearBuffer.Color, 0, ClearColor);
                GL.ClearNamedFramebuffer(framebuffer, ClearBuffer.Depth, 0, ClearDepth);

                RenderGame();

                // ... now we bind the default ("screen" frame buffer) ...
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
                GL.ClearNamedFramebuffer(0, ClearBuffer.Color, 0, ClearColor);

                GL.Disable(EnableCap.DepthTest);
                GL.Disable(EnableCap.Blend);
                GL.Disable(EnableCap.CullFace);

                PostprocessProgram.Use();

                // ... and draw the separate framebuffer to it as a square (of 2 triangles) ...
                GL.BindTexture(TextureTarget.Texture2D, framebufferTexColor);
                GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

                // ... (orthographic projection is more useful as it helps "pretend" that the scene is just a square
                // and not an actual 3D space) ...
                var projectionM = Matrix4.CreateOrthographic(Width, Height, 1, -1); 

                GL.Enable(EnableCap.Blend); 
                GL.BlendFunc(0, BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

                // ... on top of all this we draw the proper GUI.
                // See, drawing text with OpenGL is ez af xD
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

            // 2 buffer drawing is used
            // while one buffer is drawn to the screen another one is being updated
            SwapBuffers(); 
        }


        /// <summary>
        /// Pauses the game when the window is put in the background
        /// </summary>
        protected override void OnFocusedChanged(EventArgs e)
        {
            base.OnFocusedChanged(e);
            if (GameState == GameStates.PlayingGame)
            {
                GameState = GameStates.QuitMenu;
            }
        }


        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            if (isFirstFrame)
            {
                isFirstFrame = false;
            }
            if (Player.StonesCollected >= FloatingStone.AllStonesCount)
            {
                GameState = GameStates.YouWin;
            }

            // No input is handled when the window is in the background
            if (Focused)
            {
                // GUI handling
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
                // movement handling
                switch (GameState)
                {
                    case GameStates.PlayingGame:
                    {
                        CursorVisible = false;
                        World.Stones.Move();
                        if (IsPlayerMoving)
                        {
                            Player.Move(Mouse.GetState());
                            CollisionManager.CheckCollisions();
                        }
                        else
                        {
                            Camera.Move(Mouse.GetState());
                        }

                        break;
                    }
                    case GameStates.QuitGame:
                        Exit();
                        break;
                    default:
                        CursorVisible = true;
                        break;
                }
            }
        }

        private void HandleKeyboard()
        {
            var keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(Key.Escape))
            {
                GameState = GameStates.QuitMenu;
            }
            // Switches between OpenGL drawing modes
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
                GL.PointSize(3);
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
            // otherwise when resizing to a bigger window, only the smaller part of the window would be rendered to
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer);
            GL.CreateTextures(TextureTarget.Texture2D, 1, out framebufferTexColor);
            GL.TextureStorage2D(framebufferTexColor, 1, SizedInternalFormat.Rgba32f, Width, Height);

            GL.CreateTextures(TextureTarget.Texture2D, 1, out framebufferTexDepth);
            GL.BindTexture(TextureTarget.Texture2D, framebufferTexDepth);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent32f, Width, Height, 0,
                PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            DrawBuffersEnum[] draw_buffers = { DrawBuffersEnum.ColorAttachment0 };
            GL.NamedFramebufferDrawBuffers(framebuffer, 1, draw_buffers);

            GL.NamedFramebufferTexture(framebuffer, FramebufferAttachment.ColorAttachment0, framebufferTexColor, 0);
            GL.NamedFramebufferTexture(framebuffer, FramebufferAttachment.DepthAttachment, framebufferTexDepth, 0);

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
        public Vector2 GetMousePositionRelativeToWindowMiddle()
        {
            var mouse = Mouse.GetCursorState();
            return new Vector2()
            {
                X = mouse.X - (X + Width / 2),
                Y = -(mouse.Y - (Y + Height / 2))
            };
        }

        /// <summary>
        /// Initializes all GUIs
        /// </summary>
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

        public override void Dispose()
        {
            base.Dispose();
            NormalMappingProg.Dispose();
            PostprocessProgram.Dispose();
            FakeNormalMappingProg.Dispose();
            TextProgram.Dispose();
            World.Dispose();
            Font.Dispose();
            GL.DeleteFramebuffer(framebuffer);
            GL.DeleteTexture(framebufferTexColor);
            GL.DeleteTexture(framebufferTexDepth);
        }
    }
}
