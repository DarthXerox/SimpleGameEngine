using System;
using OpenTK;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenGL_in_CSharp.Utils;

namespace UnitTests
{
    [TestClass]
    public class MathOperationsTests
    {
        [TestMethod]
        public void IsWithinDistanceInPlane()
        {
            var vec1 = new Vector3(-7, 0, -4);
            var vec2 = new Vector3(17, 30, 6.5f);
            Assert.IsTrue(vec1.IsWithinDistanceInPlane(vec2, 26.2f));
            Assert.IsFalse(vec1.IsWithinDistanceInPlane(vec2, 26.1f));

            vec1.MoveFromInPlane(vec2, 30f);
            Assert.IsTrue(vec1.IsWithinDistanceInPlane(vec2, 30.011f));
            Assert.IsFalse(vec1.IsWithinDistanceInPlane(vec2, 30.0f));
        }

        [TestMethod]
        public void CollisionTest()
        {

        }
    }
}
