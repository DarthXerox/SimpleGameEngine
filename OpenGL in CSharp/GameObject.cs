using System;
using System.IO;
using System.Linq;
using ObjLoader;
using ObjLoader.Loader.Loaders;

namespace OpenGL_in_CSharp
{


	public class GameObject : IDisposable
	{
		public LoadResult Data { get; }

		public GameObject(string objFileName)
		{
			var objLoader = new ObjLoaderFactory().Create();
			var fileStream = new FileStream(objFileName, FileMode.Open);

			Data = objLoader.Load(fileStream);
			fileStream.Close();
			PrintCounts();
		}


		public void PrintCounts()
		{
			Console.WriteLine("Vertices Count: " + Data.Vertices.Count);
			Console.WriteLine("Textures  Count: " + Data.Textures.Count);
			Console.WriteLine("Normals  Count: " + Data.Normals.Count);
			Console.WriteLine("Faces  Count: " + Data.Groups.First().Faces.Count);
			Console.WriteLine("Materials   Count: " + Data.Materials.Count);
			Console.WriteLine("Groups name: ", Data.Groups.First().Name);
			//Data.Groups.First().Faces.Count();
			//Data.Materials.First();
		}


		public void Dispose()
		{
			//Data.Dis
		}
	}

}