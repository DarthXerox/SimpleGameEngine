﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using OpenTK;

namespace SimpleEngine.Data
{
    /// <summary>
    /// Simple parser for .mtl files
    /// </summary>
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
                            currentMat.Shininess = float.Parse(parts[1], CultureInfo.InvariantCulture);
                            break;
                        case "d":
                            currentMat.Transparency = 1.0f - float.Parse(parts[1], CultureInfo.InvariantCulture);
                            break;
                        case "Tr":
                            currentMat.Transparency = float.Parse(parts[1], CultureInfo.InvariantCulture);
                            break;
                    }
                }
            }

            return ret;
        }

       
        private static Vector3 ParseVector3(ref string[] parts)
        {
            float x, y, z;
            try
            {
                x = (float)double.Parse(parts[1], CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to parse: " + parts[1]);
                throw e;
            }
            try
            {
                y = float.Parse(parts[2], CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to parse: " + parts[2]);
                throw e;
            }
            try
            {
                z = float.Parse(parts[3], CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to parse: " + parts[3]);
                throw e;
            }

            return new Vector3(x, y, z);
        }
    }
}
