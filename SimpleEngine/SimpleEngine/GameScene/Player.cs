using System;
using SimpleEngine.Text;
using OpenTK;
using OpenTK.Input;
using SimpleEngine.WorldObjects;

namespace SimpleEngine.GameScene
{
    /// <summary>
    /// Player is just a movable camera with associated world in which it can move
    /// Thus it cannot escape its borders and its Position.Y equals to terrain height (+players height)
    /// at the specified X and Z coordinates
    /// </summary>
    public class Player : Camera
    {
        public World AssociatedWorld { get; }
        public ConeLight Flashlight { set; get; }
        public float Height { get; } = 5f;
        public int StonesCollected { set; get; }
        public float Radius { get; } = 2.5f;
        public DateTime LastEPressed { private set; get; } = new DateTime(2000, 1, 1);
        public DateTime LastStoneCollected { private set; get; } = new DateTime(2000, 1, 1);

        public Player(Vector3 position, World world) : base(position)
        {
            AssociatedWorld = world;
            Flashlight = new ConeLight(new Vector3(position.X,
                Height + AssociatedWorld.GetHeight(Position.X, Position.Z), position.Z),
                new Vector3(1, 1, 1), 1f, 0.027f, 0.0028f, 12.5f, 17.5f);
            Speed = 0.50f;
        }

        private void SwitchFlashlight()
        {
            var keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(Key.E) && (DateTime.Now - LastEPressed).Milliseconds > 300)
            {
                Flashlight.Color = Flashlight.Color != Vector3.Zero ? new Vector3(0, 0, 0) : new Vector3(1, 1, 1);
                LastEPressed = DateTime.Now;
            }
        }

        public override void Move(MouseState mouse)
        {
            base.UpdateAngles(mouse);

            Front = Vector3.Normalize(new Vector3(
                (float)Math.Cos(MathHelper.DegreesToRadians(Yaw)),
                0,
                (float)Math.Sin(MathHelper.DegreesToRadians(Yaw))
                ));

            base.Move();
            SwitchFlashlight();

            var pos = Position;
            // checks if the player reached world borders
            if (pos.X <= Radius)
            {
                pos.X = Radius + 0.01f;
            }
            else if (pos.X >= AssociatedWorld.MaxX - Radius)
            {
                pos.X = AssociatedWorld.MaxX - Radius - 0.01f;
            }

            if (pos.Z <= Radius)
            {
                pos.Z = Radius + 0.01f;
            }
            else if (pos.Z >= AssociatedWorld.MaxZ - Radius)
            {
                pos.Z = AssociatedWorld.MaxZ - Radius - 0.01f;
            }
            pos.Y = AssociatedWorld.GetHeight(pos.X, pos.Z) + Height;
            Position = pos;

            Flashlight.Position = new Vector4(Position, 1.0f);
            Flashlight.Direction = GetEyeFront();
        }

        public Vector3 GetEyeFront()
        {
            return Vector3.Normalize(new Vector3(
                (float)Math.Cos(MathHelper.DegreesToRadians(Pitch)) * (float)Math.Cos(MathHelper.DegreesToRadians(Yaw)),
                (float)Math.Sin(MathHelper.DegreesToRadians(Pitch)),
                (float)Math.Cos(MathHelper.DegreesToRadians(Pitch)) * (float)Math.Sin(MathHelper.DegreesToRadians(Yaw))
                ));
        }

        public override Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(Position, Position + GetEyeFront(), Up);
        }

        public void CollectStone()
        {
            StonesCollected++;
            LastStoneCollected = DateTime.Now;
        }

        public void DrawCollectedStonesGUI(int windowWidth, int windowHeight, FreeTypeFont font)
        {
            new TextBox(windowWidth / 2 - 150, windowHeight / 2 - 50,
                    $"Stones: {StonesCollected}/{FloatingStone.AllStonesCount}", 0.5f, new Vector3(1f), font, false)
                    .Draw(Vector2.Zero);
        }

        public void DrawCollectedStonesGUIWithTime(int windowWidth, int windowHeight, FreeTypeFont font)
        {
            if ((DateTime.Now - LastStoneCollected).TotalSeconds <= 5)
            {
                DrawCollectedStonesGUI(windowWidth, windowHeight, font);
            }
        }

        public void SetNewPositionOnMap(Vector2 newPosition)
        {
            Position = new Vector3(newPosition.X, AssociatedWorld.GetHeight(newPosition) + Height, newPosition.Y);
        }
    }
}
