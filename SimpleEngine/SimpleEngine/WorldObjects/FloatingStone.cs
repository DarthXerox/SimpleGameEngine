using System;
using System.Collections.Generic;
using System.Linq;
using SimpleEngine.GameScene;
using SimpleEngine.Utils;
using OpenTK;
using SimpleEngine.Data;

namespace SimpleEngine.WorldObjects
{
    public class FloatingStone : Collidable
    {
        public static readonly int AllStonesCount = 5;
        public int CoinsLeft { private set; get; } = 5;
        public float RotationYPerFrame { get; } = 1f/3f * 360f / 60f;
        public Vector3 MovementPerFrame { private set; get; } = new Vector3(0, 1f / 180f, 0);
        public Vector3 Offset { private set; get; } = new Vector3(0, 0, 0);

        public FloatingStone(NormalMappingMesh mesh, List<Transformations> positions) 
            : base (mesh, positions.Take(AllStonesCount).ToList()) 
        {
            if (positions.Count < AllStonesCount)
            {
                throw new IndexOutOfRangeException("Not enough coin positions!");
            }
        }

        /// <summary>
        /// Moves stones in a way they look like they are floating and rotating
        /// </summary>
        public void Move()
        {
            if (Offset.Y >= 1.5 || Offset.Y < 0) // change direction of movement
            {
                MovementPerFrame = -MovementPerFrame;
            }
            foreach (var trans in ModelTransformations)
            {
                trans.RotY += trans.RotY >= 360 ? -360 : RotationYPerFrame;
                trans.Position += MovementPerFrame;
            }
            Offset += MovementPerFrame;
        }
        
        public override void ReactToCollision(Player player, Transformations transformations)
        {
            ModelTransformations.Remove(transformations);
            CoinsLeft--;
            player.CollectStone();
            Sounder.PlaySound(FilePaths.SoundStonePickUp);
        }
    }
}
