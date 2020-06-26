using System;
using OpenGL_in_CSharp.Mesh_and_SceneObjects;
using OpenGL_in_CSharp.TextRendering;
using OpenTK;
using OpenTK.Input;

namespace OpenGL_in_CSharp
{
    /// <summary>
    /// Player is just a moveable camera with associated map in which it can move
    /// Thus it cannot escape its borders and its Position.Y equals terrain height
    /// at the specified X and Z coordinates
    /// </summary>
    public class Player : Camera
    {
        public Map AssociatedMap { private set; get; }
        public ConeLight Flashlight { set; get; }
        public float Height { private set; get; } = 5f;
        public int StonesCollected { set; get; } = 0;
        public float Radius { private set; get; } = 2.5f;

        public DateTime LastEPressed = new DateTime(2000, 1, 1);

        public DateTime LastStoneCollected = new DateTime(2000, 1, 1);

        public Player(Vector3 position, Map map) : base(position)
        {
            AssociatedMap = map;
            Flashlight = new ConeLight(new Vector3(position.X, 
                Height + AssociatedMap.GetHeight(Position.X, Position.Z), position.Z), 
                new Vector3(1,1,1), 1f, 0.027f, 0.0028f, 12.5f, 17.5f);
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
            var keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Key.E) && (DateTime.Now - LastEPressed).Milliseconds > 200)
            {
                if (Flashlight.Color != Vector3.Zero)
                {
                    Flashlight.Color = new Vector3(0, 0, 0);
                } else
                {
                    Flashlight.Color = new Vector3(1, 1, 1);
                }
                LastEPressed = DateTime.Now;
            }

            var pos = Position;
            // checks if the player reached map borders
            if (pos.X <= 0 + Radius)
            {
                pos.X = Radius + 0.01f;
            } else if (pos.X >= AssociatedMap.MaxX - Radius)
            {
                pos.X = AssociatedMap.MaxX - Radius - 0.01f;
            }

            if (pos.Z <= 0 + Radius)
            {
                pos.Z = Radius + 0.01f;
            }
            else if (pos.Z >= AssociatedMap.MaxZ - Radius)
            {
                pos.Z = AssociatedMap.MaxZ - Radius - 0.01f;
            }
            pos.Y = AssociatedMap.GetHeight(pos.X, pos.Z) + Height;
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
                    $"Stones: {StonesCollected}/{FloatingStone.AllCoinsCount}", 0.5f, new Vector3(1f), font, false)
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
            Position = new Vector3(newPosition.X, AssociatedMap.GetHeight(newPosition) + Height, newPosition.Y);
        }
    }
}
