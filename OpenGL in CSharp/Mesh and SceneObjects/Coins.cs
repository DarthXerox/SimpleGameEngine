using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assimp;
using OpenGL_in_CSharp.Utils;
using OpenTK;
//using OpenTK.Audio.OpenAL

namespace OpenGL_in_CSharp.Mesh_and_SceneObjects
{
    public class Coins : Collidable
    {
        public static readonly int AllCoinsCount = 10;
        public float RotationYPerFrame { get; } = 1f/3f * 360f / 60f;
        public Vector3 MovementPerFrame { private set; get; } = new Vector3(0, 1f / 180f, 0);
        public Vector3 Offset { private set; get; } = new Vector3(0, 0, 0);

        public int CoinsLeft { private set; get; } = 10;

        public ModelTransformations Movement = new ModelTransformations();


        public Coins(NormalMappingMesh mesh, 
            List<ModelTransformations> positions) : base (mesh) 
        {
            ModelTransformations = positions.Take(AllCoinsCount).ToList();
        }

        public override void Draw(LightsProgram lightsProgram, Player player, float maxDistance = 100)
        {
            base.Draw(lightsProgram, player, maxDistance);

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

        public override void ReactToCollision(Player player, ModelTransformations transformations)
        {
            ModelTransformations.Remove(transformations);
            CoinsLeft--;
            player.CoinsCollected++;
            Console.WriteLine(player.CoinsCollected);
            //play sound
        }
    }
}
