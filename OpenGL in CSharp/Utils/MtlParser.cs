using System;
using System.Collections.Generic;
using System.IO;
using OpenTK;

namespace OpenGL_in_CSharp.Utils
{

    public class Material
    {
        public string Name { set; get; }
        public Vector3 Ambient { set; get; }
        public Vector3 Diffuse { set; get; }
        public Vector3 Specular { set; get; }
        public float Shininess { set; get; }
        public float? Transparecny { set; get; }
    }

    public static class MtlParser
    {
        public static List<Material> ParseMtl(string path)
        {
            int index = 0;
            var ret = new List<Material>();
            Material currentMat = null;


            bool beingCreated = false;
            foreach (string line in File.ReadAllLines(path))
            {

                string[] parts = line.Split(' ');
                if (parts[0].ToLower() == "newmtl")
                {
                    beingCreated = true;
                    ret.Add(new Material());
                    currentMat = ret[index];
                    index++;
                    currentMat.Name = parts[1];
                    continue;
                }

                if (beingCreated)
                {
                    switch (parts[0])
                    {
                        case "Ka":
                            currentMat.Ambient = ParseVector3(ref parts);
                            break;
                        case "Kd":
                            currentMat.Diffuse = ParseVector3(ref parts);
                            break;
                        case "Ks":
                            currentMat.Specular = ParseVector3(ref parts);
                            break;
                        case "Ns":
                            Console.WriteLine(parts[1]);
                            currentMat.Shininess = float.Parse(parts[1]);
                            //currentMat.Shininess /= 1000;
                            break;
                        case "d":
                            currentMat.Transparecny = 1.0f - float.Parse(parts[1]);
                            break;
                        case "Tr":
                            currentMat.Transparecny = float.Parse(parts[1]);
                            break;
                    }
                }
            }

            return ret;

        }
        private static Vector3 ParseVector3(ref string[] parts)
        {
            return new Vector3(
                float.Parse(parts[1]),
                float.Parse(parts[2]),
                float.Parse(parts[3])
                );
        }
    }
}
