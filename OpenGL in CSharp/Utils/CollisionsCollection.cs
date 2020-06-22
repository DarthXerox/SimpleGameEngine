using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGL_in_CSharp.Utils
{

    public class MeshMinXComparator : Comparer<Mesh>
    {
        public override int Compare(Mesh x, Mesh y)
        {
            return x.Model.MinX.CompareTo(y.Model.MinX);
        }
    }

    public class CollisionsCollection<T>
        where T : Mesh
    {

    }
}
