using OpenTK.Mathematics;
using rt004.Solids;

namespace rt004
{

    public static class Phong
    {
        //TODO Refactor to camera/different class later
        public static (ISolid?, double?) ThrowRay(Ray ray, List<ISolid> solids)
        {
            ISolid? result = null;
            double? t = null;
            foreach (ISolid solid in solids)
            {
                double? tmp = solid.Intersection(ray);
                if (tmp != null && t == null && tmp > 0.06)
                {
                    t = tmp;
                    result = solid;
                }
                else if (tmp != null && t != null && tmp > 0.06)
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

        //TODO Could remake to, so the transformations happen based around an explicit point in space
        // Refactor to camera/different class later
        //version that uses hiearchy
        public static (ISolid?, double?, Matrix4d) ThrowRayHierarchy(Ray ray, Node root)
        {
            (ISolid?, double?, Matrix4d) result = new(null, null, Matrix4d.Identity);
            result = _throwRayRecursiveHierarchy(ray, root, Matrix4d.Identity, result, 0);
            return result;
        }

        private static (ISolid?, double?, Matrix4d) _throwRayRecursiveHierarchy(Ray ray, Node node, Matrix4d currentTransformation,
            (ISolid?, double?, Matrix4d) result, int dbgLayer)
        {
            //CurrentTransformation starts as an identity matrix,
            //and then is multiplied by inverted matrices, therefore, it doesnt need to be inverted again.
            currentTransformation = currentTransformation * node.TransformationMatrixInverse; //TODO:order of transformation matters, figure out which one is correct


            /*
            //transform scale
            Vector3d scale = currentTransformation.ExtractScale();
            transformedRay.Direction = scale * ray.Direction;
            
            //transform rotation
            Quaterniond rotation = currentTransformation.ExtractRotation(); //TODO might cause some light jankinness later
            transformedRay.Direction = rotation * ray.Direction;
            */
            //transform translation
            /*
            Matrix4d translation = currentTransformation.ExtractTranslationMatrix();
            Ray transformedRay = ray.TransformOrigin(translation);
            //transform rotation and scale
            Matrix4d rotationAndScale = currentTransformation.Transposed().ClearTranslation().Transposed();
            transformedRay.TransformedDirection(rotationAndScale);
            */
            Ray transformedRay = ray.TransformRay(currentTransformation);

            foreach (Node child in node.Nodes)
            {
                result = _throwRayRecursiveHierarchy(ray, child, currentTransformation, result, ++dbgLayer);
            }
            double? tmp = null;
            if (node.Solid is not null)
            {
                tmp = node.Solid.Intersection(transformedRay);
            }

            if (tmp is not null && result.Item2 is null && tmp > 0.06)
            {
                result.Item2 = tmp;
                result.Item1 = node.Solid;
                result.Item3 = currentTransformation;
            }
            else if (tmp is not null && result.Item2 is not null && tmp > 0.06)
            {
                if (tmp < result.Item2)
                {
                    result.Item2 = tmp;
                    result.Item1 = node.Solid;
                    result.Item3 = currentTransformation;
                }
            }
            return result;
        }
        
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
        /*
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
        */
        
        public static Vector3d Compute(List<ILightSource> lightSources, ISolid intersectedSolid,
            List<ISolid> solidsInScene, Vector3d intersectionPoint, Ray ray, double ambientCoeficient)
        {
            bool isInsideSolid = false;
            if( intersectedSolid == ray.OriginSolid ) { isInsideSolid = true; }

            //ambient light
            Vector3d Ea = intersectedSolid.Material.Colour(intersectedSolid.GetUVCoords(intersectionPoint, isInsideSolid)) * ambientCoeficient;

            Vector3d normal = intersectedSolid.GetNormal(intersectionPoint, isInsideSolid).Normalized();

            foreach (ILightSource lightSource in lightSources)
            {
                int success = 0;
                int shadowRayNum = 10; //parametrize l8r b8r

                Vector3d directionToLight = -lightSource.DirectionToLight(intersectionPoint); //
                //diffuse component
                double dotDiffusion = Vector3d.Dot(directionToLight, normal);
                Vector3d Ed = lightSource.Intensity * intersectedSolid.Material.Colour(intersectedSolid.GetUVCoords(intersectionPoint, isInsideSolid))* intersectedSolid.Material.DiffusionCoefficient * (dotDiffusion > -1.0e-6 ? dotDiffusion : 0); ;

                //specular component
                double dotReflection = Vector3d.Dot((2 * normal * Vector3d.Dot(normal, directionToLight) - directionToLight).Normalized(), ray.Direction);
                Vector3d Es = lightSource.Intensity * lightSource.Color * intersectedSolid.Material.SpecularCoefficient * Math.Pow((dotReflection > 0 ? dotReflection : 0), intersectedSolid.Material.Glossiness);

                for(int i = 0; i < shadowRayNum; i++)
                {
                    if (!Shadow(intersectedSolid, intersectionPoint, lightSource, solidsInScene))
                    {
                        success++;
                    }
                }
                Ea += (Ed + Es) * success / shadowRayNum;
            }

            return Ea;
        }
        
        public static bool Shadow(ISolid sourceSolid, Vector3d point, ILightSource light, List<ISolid> solids) //directional light
        {
            bool intersects = false;
            Ray shadowRay = new Ray(point, (light.DirectionToLight(point)), null); //this could pose a problem? what if the direction to light is outside and the origin inside? not sure if valid thought
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

            bool isInsideSolid = false;
            if (intersectedSolid == ray.OriginSolid) { isInsideSolid = true; }

            Vector3d intersectedPoint = (Vector3d)(ray.Origin + (intersection.Item2 * ray.Direction));
            Vector3d color = default; //result color

            foreach (ILightSource light in scene.LightSources)
            {
                color += Compute(scene.LightSources, intersectedSolid, scene.Solids, intersectedPoint, ray, scene.AmbientCoefficient);
            }

            if (depth > maxdepth)
            {
                return color;
            }

            //reflection
            Vector3d reflectionColor = default;
            if (intersectedSolid.Material.SpecularCoefficient > 0)
            {
                Vector3d reflectionVector = 2 * Vector3d.Dot(intersectedSolid.GetNormal(intersectedPoint, isInsideSolid), -(ray.Direction)) * intersectedSolid.GetNormal(intersectedPoint, isInsideSolid) + ray.Direction;
                reflectionColor += intersectedSolid.Material.SpecularCoefficient * Shade(scene, new Ray(intersectedPoint, reflectionVector, ray.OriginSolid), depth + 1, maxdepth);
            }

            //refraction
            Vector3d refractionColor = default;
            double originRefractiveIndex;
            if (ray.OriginSolid != null) { originRefractiveIndex = ray.OriginSolid.Material.RefractiveIndex; }
            else { originRefractiveIndex = 1; }

            double intersectedSolidRefractiveIndex;
            if (ray.OriginSolid != null) { intersectedSolidRefractiveIndex = ray.OriginSolid.Material.RefractiveIndex; }
            else { intersectedSolidRefractiveIndex = intersectedSolid.Material.RefractiveIndex; }

            double kR = originRefractiveIndex / intersectedSolidRefractiveIndex; //refractive coeficient
            Vector3d normalVector = -intersectedSolid.GetNormal(intersectedPoint, isInsideSolid);
            double normalRayDotProduct = Vector3d.Dot(normalVector, -ray.Direction);

            double refractiveVectorAngleCosine = Math.Sqrt(1 - (kR * kR) * (1 - (normalRayDotProduct * normalRayDotProduct)));

            if ((originRefractiveIndex > intersectedSolidRefractiveIndex) && refractiveVectorAngleCosine <= (intersectedSolidRefractiveIndex / originRefractiveIndex))
            {
                return color;
            }
            else
            {
                Vector3d refractiveVector = (kR * normalRayDotProduct - refractiveVectorAngleCosine) * normalVector - kR * (-ray.Direction);
                Ray refractedRay = new(intersectedPoint, refractiveVector, intersectedSolid);

                refractionColor += Shade(scene, refractedRay, depth + 1, maxdepth);
            }

            color += (reflectionColor * (1 - intersectedSolid.Material.Transparency)) + (refractionColor * intersectedSolid.Material.Transparency);

            return color;
        }
        
        /// <summary>
        /// Checks if there is a solid blocking a r ILighSource
        /// </summary>
        /// <param name="intersectionPoint"></param>
        /// <param name="light"></param>
        /// <param name="intersectedSolid"></param>
        /// <returns> True if there is a solid blocking the ray from the intersectionPoint</returns>

        public static bool ShadowHierarchy(Vector3d intersectionPoint, ILightSource light, Node intersectedSolid)
        {
            bool intersects = false;
            Ray shadowRay = new Ray(intersectionPoint, (light.DirectionToLight(intersectionPoint)), intersectedSolid.Solid); //this could pose a problem? what if the direction to light is outside and the origin inside? not sure if valid thought

            foreach (var child in intersectedSolid.Nodes)
            {
                _checkShadowRecursiveHierarchy(ref intersects, intersectedSolid, child, shadowRay);
            }

            return intersects;
        }

        private static void _checkShadowRecursiveHierarchy(ref bool intersects, Node sourceSolid, Node solid, Ray shadowRay)
        {
            if(intersects == true)
            {
                return;
            }

            if (solid.Solid is not null)
            {
                if(solid != sourceSolid)
                {
                    double? intersection = solid.Solid.Intersection(shadowRay);
                    if (intersection is not null)
                    {
                        intersects = true;
                        return;
                    }
                }
            }

            foreach (var sol in solid.Nodes)
            {
                _checkShadowRecursiveHierarchy(ref intersects, sourceSolid, sol, shadowRay);
            }
        }
        
        public static Vector3d ComputeHierarchy(Scene scene, ISolid intersectedSolid,
            Vector3d intersectionPoint, bool isInsideSolid, Vector3d normal, Ray ray, int sampleSize)
        {
            //ambient light
            Vector3d Ea = intersectedSolid.Material.Colour(intersectedSolid.GetUVCoords(intersectionPoint, isInsideSolid)) * scene.AmbientCoefficient;

            foreach (ILightSource lightSource in scene.LightSources)
            {
                int success = 0;
                int shadowRayNum = sampleSize;

                Vector3d directionToLight = -lightSource.DirectionToLight(intersectionPoint); //
                //diffuse component
                double dotDiffusion = Vector3d.Dot(directionToLight, normal);
                Vector3d Ed = lightSource.Intensity * intersectedSolid.Material.Colour(intersectedSolid.GetUVCoords(intersectionPoint, isInsideSolid)) * intersectedSolid.Material.DiffusionCoefficient * (dotDiffusion > -1.0e-6 ? dotDiffusion : 0); ;

                //specular component
                double dotReflection = Vector3d.Dot((2 * normal * Vector3d.Dot(normal, directionToLight) - directionToLight).Normalized(), ray.Direction);
                Vector3d Es = lightSource.Intensity * lightSource.Color * intersectedSolid.Material.SpecularCoefficient * Math.Pow((dotReflection > 0 ? dotReflection : 0), intersectedSolid.Material.Glossiness);
                
                for (int i = 0; i < shadowRayNum; i++)
                {
                    if (!ShadowHierarchy(intersectionPoint, lightSource, scene.Root))
                    {
                        success++;
                    }
                }
                //Ea += (Ed + Es) * success / shadowRayNum;
                               //shadows are off
                Ea += (Ed + Es); //delete this later, when you want to uncomment shadows
            }
            return Ea;
        }
        
        public static Vector3d ShadeHierarchy(Scene scene, Ray ray, int depth, int sampleSize, int maxdepth) 
        {

            (ISolid? solid, double? rayLenght, Matrix4d invertedTransformationMatrix) intersection = Phong.ThrowRayHierarchy(ray, scene.Root);
            if (intersection.solid is null || intersection.rayLenght is null)
            {
                return new Vector3d(0, 0, 0); //return scene background // no interscections //dbg = (1,0,0)
            }

            Matrix4d invTrans = intersection.invertedTransformationMatrix; //inverted transformation matrix

            //does the ray need to be transformed?
            //Ray transformedRay = ray.TransformRay(intersection.invertedTransformationMatrix);
            //ray = transformedRay;
            ISolid intersectedSolid = intersection.solid;

            bool isInsideSolid = false;
            if (intersectedSolid == ray.OriginSolid) { isInsideSolid = true; }

            Vector3d intersectedPoint = (Vector3d)(ray.Origin + (intersection.rayLenght * ray.Direction));
            Vector3d color = new(0,0,0); //result color
            
            //transform normal
            Vector4d tmpNormal = (Matrix4d.Transpose(invTrans) * new Vector4d(intersectedSolid.GetNormal(intersectedPoint, isInsideSolid),1)).Normalized();
            Vector3d normal = tmpNormal.Xyz;

            foreach (ILightSource light in scene.LightSources)
            {
                color += ComputeHierarchy(scene, intersectedSolid, intersectedPoint, isInsideSolid, normal, ray, sampleSize);//ambient coeffient should be given 
            }

            if (depth > maxdepth)
            {
                return color;
            }
            //this till the return color probably doesnt work? idk was commented
            /*
            //reflection
            Vector3d reflectionColor = default;
            if (intersectedSolid.Material.SpecularCoefficient > 0)
            {
                Vector3d reflectionVector = 2 * Vector3d.Dot(normal, -(ray.Direction)) * normal + ray.Direction;
                reflectionColor += intersectedSolid.Material.SpecularCoefficient * ShadeHierarchy(scene, new Ray(intersectedPoint, reflectionVector, ray.OriginSolid), depth + 1, sampleSize, maxdepth);
            }
            
            //refraction
            Vector3d refractionColor = default;
            double originRefractiveIndex;
            if (ray.OriginSolid != null) { originRefractiveIndex = ray.OriginSolid.Material.RefractiveIndex; }
            else { originRefractiveIndex = 1; }

            double intersectedSolidRefractiveIndex;
            if (ray.OriginSolid != null) { intersectedSolidRefractiveIndex = ray.OriginSolid.Material.RefractiveIndex; }
            else { intersectedSolidRefractiveIndex = intersectedSolid.Material.RefractiveIndex; }

            double kR = originRefractiveIndex / intersectedSolidRefractiveIndex; //refractive coeficient

            Vector3d normalVector = -normal; //redundant, refactor later
            double normalRayDotProduct = Vector3d.Dot(normalVector, -ray.Direction);

            double refractiveVectorAngleCosine = Math.Sqrt(1 - (kR * kR) * (1 - (normalRayDotProduct * normalRayDotProduct)));

            if ((originRefractiveIndex > intersectedSolidRefractiveIndex) && refractiveVectorAngleCosine <= (intersectedSolidRefractiveIndex / originRefractiveIndex))
            {
                return color;
            }
            else
            {
                Vector3d refractiveVector = (kR * normalRayDotProduct - refractiveVectorAngleCosine) * normalVector - kR * (-ray.Direction);
                Ray refractedRay = new(intersectedPoint, refractiveVector, intersectedSolid);

                refractionColor += ShadeHierarchy(scene, refractedRay, depth + 1, sampleSize, maxdepth);
            }

            color += (reflectionColor * (1 - intersectedSolid.Material.Transparency)) + (refractionColor * intersectedSolid.Material.Transparency);
            */
            return color;
            
        }
    }
}
