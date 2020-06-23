using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace OpenGL_in_CSharp.Utils
{
    /// <summary>
    /// Used only for debugging
    /// </summary>
    public static class HelpPrinter
    {

		/// <summary>
		/// Searches a list of 3D vectors (OpenTK's Vector3 or Assimp's Vector3D)
		/// and finds min/max X,Y,Z
		/// </summary>
		/// <typeparam name="T">Must have properties: X, Y, Z</typeparam>
	 /*
		public static void FindModelBorders<T>(ObjModel model, List<T> vertices3D)
		{
			foreach (var vec in vertices3D)
			{
				if (vec.X > Model.MaxX)
				{
					Model.MaxX = vec.X;
				}
				if (vec.Y > Model.MaxY)
				{
					Model.MaxY = vec.Y;
				}
				if (vec.Z > Model.MaxZ)
				{
					Model.MaxZ = vec.Z;
				}
				///
				if (vec.X < Model.MinX)
				{
					Model.MinX = vec.X;
				}
				if (vec.Y < Model.MinY)
				{
					Model.MinY = vec.Y;
				}
				if (vec.Z < Model.MinZ)
				{
					Model.MinZ = vec.Z;
				}
			}
		}
		*/
		public static void PrintList<T>(List<T> lst)
        {
            Console.WriteLine("Printing list, length: " + lst.Count);

            foreach (T el in lst)
            {
                Console.Write(el.ToString());
                Console.Write(" ");
            }
            Console.WriteLine();
        }

        public static void PrintArray<T>(T[] arr)
        {
            foreach (T el in arr)
            {
                Console.Write(el.ToString());
                Console.Write(" ");
            }
            Console.WriteLine();
        }
    }
}
