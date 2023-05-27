using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static rt004.Program;

namespace rt004
{
    //so yes, we apply the inverse transformation to a ray, instead of applying transformations to objects
    // but we still need to apply a transformation matrix to the normal vector of the object

    public static class Phong
    {
        
        /*public static (ISolid?, double?) ThrowRay(Ray ray, List<ISolid> solids)
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
        }*/
        /*
        public static (ISolid?, double?) ThrowRay(Ray ray, Node node)
        {
            ISolid? result = null;
            double? t = null;
            
            Matrix4d currentTransformation = node.TransformationMatrix;
            
            ray.Direction = Matrix4d.Mult(currentTransformation, (Matrix4d)ray.Direction);
            foreach( var solid in node.Solids)
            {

            }




            //for each solid list in a node
            if(node.Solids.Any())
            {
                foreach (ISolid solid in node.Solids)
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
            }

            return (result, t);
        
        }
        */
        public static (ISolid?, double?) ThrowRay(Ray ray, Node node)
        {
            Ray newRay = new Ray(Vector3d.TransformPosition(ray.Origin, node.InvertedMatrix), Vector3d.TransformPosition(ray.Direction, node.InvertedMatrix).Normalized());
            //newRay.Transformation = Matrix4d.Mult(node.InvertedMatrix, ray.Transformation);

            double? offset;
            (ISolid?, double?) closestSolid = new(null, -1);

            foreach (var solid in node.Solids)
            {
                offset = solid.Intersection(newRay);
                if (offset != null)
                {
                    if (closestSolid.Item2 > offset || closestSolid.Item2 == -1)
                    {
                        closestSolid = new(solid, offset);                  
                    }
                }
            }

            (ISolid?, double?) kidClosestSolid;

            if (node.Nodes.Count > 0)
            {
                foreach (var nodeKid in node.Nodes)
                {
                    kidClosestSolid = ThrowRay(ray, nodeKid);
                    if (closestSolid.Item2 == -1 || (kidClosestSolid.Item2 != -1 && kidClosestSolid.Item2 < closestSolid.Item2))
                    {
                        closestSolid = kidClosestSolid;
                    }
                }
            }
            return closestSolid;
        }
        public static Vector3d Compute(List<ILightSource> lightSources, ISolid solid, List<ISolid> solids, Vector3d intersectionPoint, Ray ray, double ambientCoeficient)
        {
            //ambient light
            Vector3d Ea = solid.Material.Color * ambientCoeficient;

            Vector3d normal = solid.GetNormal(intersectionPoint).Normalized();

            foreach (DirectionLightSource lightSource in lightSources)
            {
                if (!Shadow(solid, intersectionPoint, lightSource, solids))
                {
                    //diffuse component
                    double dotDiffusion = Vector3d.Dot(lightSource.Direction, normal);
                    Vector3d Ed = lightSource.Intensity * solid.Material.Color * solid.Material.DiffusionCoefficient * (dotDiffusion > -1.0e-6 ? dotDiffusion : 0); ;

                    //specular component
                    double dotReflection = Vector3d.Dot((2 * normal * (Vector3d.Dot(normal, lightSource.Direction)) - lightSource.Direction).Normalized(), ray.Direction);
                    Vector3d Es = lightSource.Intensity * lightSource.Color * solid.Material.SpecularCoefficient * Math.Pow((dotReflection > 0 ? dotReflection : 0), solid.Material.Glossiness);

                    Ea += Ed + Es;
                }
            }
            return Ea;
        }

        public static bool Shadow(ISolid sourceSolid, Vector3d point, DirectionLightSource light, List<ISolid> solids) //directional light
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
            if (intersection.Item1 == null || intersection.Item2 == null)
            {
                return new Vector3d(0, 0, 0); //return scene background // no interscections
            }

            ISolid intersectedSolid = intersection.Item1;
            Vector3d intersectedPoint = (Vector3d)(ray.Origin + (intersection.Item2 * ray.Direction));
            Vector3d color = default; //result color

            foreach (DirectionLightSource light in scene.LightSources)
            {
                color += Compute(scene.LightSources, intersectedSolid, scene.Solids, intersectedPoint, ray, 0.2);//ambient coeffient should be given 
            }
            if (depth > maxdepth)
            {
                return color;
            }
            if (intersectedSolid.Material.SpecularCoefficient > 0)
            {
                Vector3d reflectionVector = 2 * Vector3d.Dot(intersectedSolid.GetNormal(intersectedPoint), -(ray.Direction)) * intersectedSolid.GetNormal(intersectedPoint) + ray.Direction;
                color += intersectedSolid.Material.SpecularCoefficient * Shade(scene, new Ray(intersectedPoint, reflectionVector), depth + 1, maxdepth);
            }
            return color;
        }
    }
}
