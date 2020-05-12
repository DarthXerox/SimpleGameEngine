using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assimp;
using GlmSharp;
using OpenTK.Graphics.OpenGL4;
using OpenTK;


namespace OpenGL_in_CSharp.Utils
{
    public class Animator : IDisposable
    {
        public Scene Scene { private set; get; }

        public Animator(string filename)
        {
            Scene?.Clear();
            Scene = new AssimpContext().ImportFile(filename, PostProcessSteps.FlipUVs 
                | PostProcessSteps.GenerateNormals 
                | PostProcessSteps.Triangulate);

            //Scene.Meshes[0].HasTextureCoords

            Console.WriteLine( "Anim count: " +  Scene.Animations.First().DurationInTicks / Scene.Animations.First().TicksPerSecond);
            Console.WriteLine("has MEsh coords: " + Scene.Meshes[0].HasTextureCoords(0));
            Console.WriteLine("Has cam: " + Scene.CameraCount);
            Console.WriteLine("Sons: " + Scene.RootNode.ChildCount);

            int vbo;
            //GL.GenVertexArray(1, )


            //if (Scene.)
        }

        private void CreateInitialScene()
        {

        }


        public void Update()
        {
            //GL.Un
        }

        public void Lol()
        {
            //Scene.Animations.First().MeshAnimationChanne;
            //vec4 x;
            Assimp.Matrix4x4 x;
            Matrix4x4 y;
            //Matrix4 e = x.;
           
        }

        /*private bool LoadMesh(string filename)
        {
            // Release the previously loaded mesh (if it exists)
            //Clear();
        }*/

        public void Dispose()
        {
            Scene.Clear();
        }


    }
}
