using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGL_in_CSharp.Utils
{
    /// <summary>
    /// Used only for debugging
    /// </summary>
    public static class HelpPrinter
    {
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
