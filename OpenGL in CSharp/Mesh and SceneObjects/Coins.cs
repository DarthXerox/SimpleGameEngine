using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assimp;
using ObjLoader.Loader.Data.VertexData;
using OpenGL_in_CSharp.Utils;
using OpenTK;
//using OpenTK.Audio.OpenAL

namespace OpenGL_in_CSharp.Mesh_and_SceneObjects
{
    public class Coins : Collidable
    {
        public static readonly int AllCoinsCount = 5;
        public float RotationYPerFrame { get; } = 1f/3f * 360f / 60f;
        public Vector3 MovementPerFrame { private set; get; } = new Vector3(0, 1f / 180f, 0);
        public Vector3 Offset { private set; get; } = new Vector3(0, 0, 0);

        public int CoinsLeft { private set; get; } = 5;

        public ModelTransformations Movement = new ModelTransformations();

        public List<Vector3> LightsPositions = new List<Vector3>();

        public Coins(NormalMappingMesh mesh, 
            List<ModelTransformations> positions) : base (mesh) 
        {
            if (positions.Count < AllCoinsCount)
            {
                throw new IndexOutOfRangeException("Not enough coin positions");
            }
            ModelTransformations = positions.Take(AllCoinsCount).ToList();
            LightsPositions = positions.Select(trans => trans.Position + 3 * Vector3.UnitY)
                .Take(AllCoinsCount)
                .ToList();
        }

        /// <summary>
        /// To draw the light above the floating objects we use this
        /// </summary>
        /// <param name="prog"></param>
        public void AttachAllLights(LightsProgram normalMappingProg, LightsProgram fakeNormalMappingProg, ref int lightIndex)
        {
            foreach (var position in LightsPositions)
            {
                var light = new ConeLight(position, -Vector3.UnitY, 1f, 0.07f, 0.017f, 12.5f, 17.5f);
                normalMappingProg.AttachLight(light, lightIndex);
                fakeNormalMappingProg.AttachLight(light, lightIndex);
                lightIndex++;
            }
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
            player.CollectCoin();
            Console.WriteLine(player.CoinsCollected);
            //play sound
        }
    }
}
