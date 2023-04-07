using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static rt004.Program;

namespace rt004
{
    public static class Phong
    {
        public static (ISolid?, double?) ThrowRay(Ray ray, List<ISolid> solids)
        {
            ISolid? result = null;
            double? t = null;
            foreach (ISolid solid in solids)
            {
                double? tmp = solid.Intersection(ray);
                if (tmp != null && t == null && tmp > 0.6)
                {
                    t = tmp;
                    result = solid;
                }
                else if (tmp != null && t != null && tmp > 0.6)
                {
                    if (tmp < t)
                    {
                        t = tmp;
                        result = solid;
                    }
                }
            }
            return (result, t);
        }
        public static Vector3d Compute(List<LightSource> lightSources, ISolid solid, List<ISolid> solids, Vector3d intersectionPoint, Ray ray, double ambientCoeficient)
        {
            //ambient light
            Vector3d Ea = solid.Material.Color * ambientCoeficient;

            Vector3d normal = solid.GetNormal(intersectionPoint).Normalized();

            foreach (LightSource lightSource in lightSources)
            {
                if (!Shadow(solid, intersectionPoint, lightSource, solids))
                {
                    //diffuse component
                    double dotDiffusion = Vector3d.Dot(lightSource.Direction, normal);
                    Vector3d Ed = lightSource.Intensity * solid.Material.Color * solid.Material.DiffuseCoeficient * (dotDiffusion > -1.0e-6 ? dotDiffusion : 0); ;

                    //specular component
                    double dotReflection = Vector3d.Dot((2 * normal * (Vector3d.Dot(normal, lightSource.Direction)) - lightSource.Direction).Normalized(), ray.Direction);
                    Vector3d Es = lightSource.Intensity * lightSource.Color * solid.Material.SpecularCoeficient * Math.Pow((dotReflection > 0 ? dotReflection : 0), solid.Material.Glossiness);

                    Ea += Ed + Es;
                }
            }
            return Ea;
        }

        public static bool Shadow(ISolid sourceSolid, Vector3d point, LightSource light, List<ISolid> solids) //directional light
        {
            bool intersects = false;
            Ray shadowRay = new Ray(point, -(light.Direction));
            foreach (ISolid solid in solids)
            {
                if (solid != sourceSolid)
                {
                    double? intersection = solid.Intersection(shadowRay);
                    if (intersection is not null)
                    {
                        intersects = true;
                    }
                }
            }
            return intersects;
        }

        public static Vector3d Shade(Scene scene, Ray ray, int depth, int maxdepth)
        {

            (ISolid?, double?) intersection = Phong.ThrowRay(ray, scene.Solids);
            if (intersection.Item1 == null)
            {
                return new Vector3d(0, 0, 0); //return scene background // no interscections
            }

            ISolid intersectedSolid = intersection.Item1;
            Vector3d intersectedPoint = (Vector3d)(ray.Origin + (intersection.Item2 * ray.Direction));
            Vector3d color = default; //result color

            foreach (LightSource light in scene.LightSources)
            {
                color += Compute(scene.LightSources, intersectedSolid, scene.Solids, intersectedPoint, ray, 0.2);//ambient coeffient should be given 
            }
            if (depth > maxdepth)
            {
                return color;
            }
            if (intersectedSolid.Material.SpecularCoeficient > 0)
            {
                Vector3d reflectionVector = 2 * Vector3d.Dot(intersectedSolid.GetNormal(intersectedPoint), -(ray.Direction)) * intersectedSolid.GetNormal(intersectedPoint) + ray.Direction;
                color += intersectedSolid.Material.SpecularCoeficient * Shade(scene, new Ray(intersectedPoint, reflectionVector), depth + 1, maxdepth);
            }
            return color;
        }

    }
}
