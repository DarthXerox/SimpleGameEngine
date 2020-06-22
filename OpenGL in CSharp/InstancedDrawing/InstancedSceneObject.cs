using System.Collections.Generic;
using OpenGL_in_CSharp.Utils;
using OpenTK;

namespace OpenGL_in_CSharp.InstancedDrawing
{
    public class InstancedSceneObject
    {
        public List<ModelTransformations> ModelTransformations { protected set; get; } = new List<ModelTransformations>();
        public Mesh CommonMesh { get; }

        public InstancedSceneObject(string objFileName, string textureFileName)
        {
            CommonMesh = new Mesh(objFileName, textureFileName);
        }

        public InstancedSceneObject(Mesh commonMesh)
        {
            CommonMesh = commonMesh;
        }

        public void AddPosition(Vector3 pos)
        {
            var temp = new ModelTransformations()
            {
                Position = pos
            };
            ModelTransformations.Add(temp);
        }

       

        public void Draw(ShaderProgram program)
        {
            foreach (var sceneObject in ModelTransformations) 
            {
                program.AttachModelMatrix(sceneObject.GetModelMatrix());
                CommonMesh.Draw();
            }
        }


        public class InstancedCollidable 
        {
            public List<Collidable> Collidables { private set; get; } = new List<Collidable>();
            public Mesh CommonMesh { get; }

            public InstancedCollidable(Mesh mesh) 
            {
                CommonMesh = mesh;
            }

            public InstancedCollidable(string objFileName, string textureFileName)
            {
                CommonMesh = new Mesh(objFileName, textureFileName);
            }

            public void AddPosition(Vector3 pos)
            {
                var temp = new Collidable(CommonMesh)
                {
                    Position = pos
                };
                Collidables.Add(temp);
            }


            public void Draw(ShaderProgram program)
            {
                foreach (var coll in Collidables)
                {
                    program.AttachModelMatrix(coll.GetModelMatrix());
                    CommonMesh.Draw();
                }
            }

            public void AddToCollisionQueue(CollisionManager collManager)
            {
                foreach (var coll in Collidables)
                {
                    collManager.CollisionChecking += coll.OnCollisionCheck;
                }
            }

        }
        
    }
}
