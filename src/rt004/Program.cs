using Util;
using OpenTK.Mathematics;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using Microsoft.VisualBasic;
//using System.Numerics;

namespace rt004;

internal class Program
{
    public struct Ray
    {
        public Vector3d Origin { get; set; }
        public Vector3d Direction { get; set; }
        public Ray(Vector3d origin, Vector3d direction )
        {
            Origin = origin;
            Direction = direction.Normalized();
        }
    }
    public class Material
    {
        public Vector3d Color { get; set; }
        public double DiffuseCoeficient { get; set; }
        public double SpecularCoeficient { get; set; }
        public double Glossiness { get; set; }
        public double KR { get; set; } //relfection coef


        public Material(Vector3d color, double diffuseCoeficient, double specularCoeficient, double glossiness, double reflectionCoef)
        {
            Color = color;
            DiffuseCoeficient = diffuseCoeficient;
            SpecularCoeficient = specularCoeficient;
            Glossiness = glossiness;
            KR = reflectionCoef;
        }
    }
    public class LightSource
    {
        public Vector3d Origin { get; set; }
        public Vector3d Direction { get; set; }
        public double Intensity { get; set; }
        public Vector3d Color { get; set;}
        public LightSource(Vector3d origin, Vector3d direction,  Vector3d color, double intensity)
        {
            Origin = origin;
            Direction = direction.Normalized();
            Color = color;
            Intensity = intensity;
        }
    }

    public static class Phong
    {
        public static Vector3d Compute(List<LightSource> lightSources, ISolid solid, List<ISolid> solids, Vector3d intersectionPoint, Ray ray, double ambientCoeficient) 
        {
            //ambient light
            Vector3d Ea = solid.Material.Color * ambientCoeficient;

            Vector3d normal = solid.GetNormal(intersectionPoint).Normalized();

            foreach (LightSource lightSource in lightSources)
            {
                if(!Shadow(solid,intersectionPoint, lightSource, solids))
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
            foreach(ISolid solid in solids)
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

            (ISolid?, double?) intersection = Camera.ThrowRay(ray, scene.Solids);
            if (intersection.Item1 == null)
            {
                return new Vector3d(0, 0, 0); //return scene background // no interscections
            }

            ISolid intersectedSolid = intersection.Item1;
            Vector3d intersectedPoint = (Vector3d)(ray.Origin + (intersection.Item2 * ray.Direction));
            Vector3d color = default; //result color

            foreach(LightSource light in scene.LightSources)
            {
                color += Compute(scene.LightSources, intersectedSolid, scene.Solids, intersectedPoint, ray, 0.2);//ambient coeffient should be given 
            }
            if( depth > maxdepth)
            {
                return color;
            }
            if(intersectedSolid.Material.KR > 0)
            {
                Vector3d reflectionVector = 2 * Vector3d.Dot(intersectedSolid.GetNormal(intersectedPoint), -(ray.Direction)) * intersectedSolid.GetNormal(intersectedPoint) + ray.Direction;
                color += intersectedSolid.Material.KR * Shade(scene, new Ray(intersectedPoint, reflectionVector), depth + 1, maxdepth);
            }
            return color;
        }

    }

    public interface ISolid
    {
        Material Material { get; set; }
        double? Intersection(Ray ray);
        Vector3d GetNormal(Vector3d point);
    }

    public class Sphere : ISolid
    {
        public Material Material { get; set; }
        public Vector3d Origin { get; set; }
        public double Size { get; set; }
        public Sphere(Material material, Vector3d origin, double size)
        {
            Material = material;
            Origin = origin;
            Size = size;
        }

        public double? Intersection( Ray ray )
        {
            double a = Vector3d.Dot( ray.Direction, ray.Direction );
            double b = 2 * Vector3d.Dot( ray.Direction, ray.Origin - this.Origin );
            double c = Vector3d.Dot( ray.Origin - this.Origin, ray.Origin - this.Origin ) - this.Size * this.Size;

            double d = b * b - ( 4 * a * c );
            if( d < 0 )
            {
                return null;
            }
            else
            {
                double t1 = ( (-b) + Math.Sqrt(d) ) / ( 2 * a );
                double t2 = ( (-b) - Math.Sqrt(d) ) / ( 2 * a );

                double result = Math.Min(t1, t2);
                
                
                if( result < 0)
                {
                    result = Math.Max(t1, t2);
                    if( result < 0 ) { return null; }
                }
                
                return result;
            }
        }

        public Vector3d GetNormal( Vector3d point )
        {
            return Vector3d.Subtract( Origin, point ).Normalized();
        }
    }

    public class Cylinder : ISolid
    {
        public Material Material { get; set; }
        public double Origin { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public double? Intersection(Ray ray)
        {
            return 0;
        }
        public Vector3d GetNormal( Vector3d point)
        {
            return point; //todo
        }
    }

    public class Plane : ISolid
    {
        public Material Material { get; set; }
        public Vector3d Origin { get; set; }
        public Vector3d Vector { get; set; }
        public Plane(Material material, Vector3d origin, Vector3d vector)
        {
            Material = material;
            Origin = origin;
            Vector = vector.Normalized();
        }

        public double? Intersection(Ray ray)
        {
            double d = -Vector3d.Dot(Origin, Vector);
            if( Vector3d.Dot(Vector, ray.Direction) == 0 )
            {
                return null;
            }
            double offset = -(Vector3d.Dot(Vector,ray.Origin) + d ) / Vector3d.Dot(Vector, ray.Direction);
            if( offset < 0 ) { return null; }
            return offset;
        }
        public Vector3d GetNormal(Vector3d point)
        {
            return Vector;
        }
    }

    public class Scene
    {
        public double AmbientCoeficient { get; set; }
        public Camera Camera { get; set; }
        public Dictionary<string, Material> Materials { get; set; }
        public List<ISolid> Solids { get; set; }
        public List<LightSource> LightSources { get; set; }
        public Scene( double ambientCoeficient, Camera camera, List<ISolid> solids, List<LightSource> lightSources)
        {
            AmbientCoeficient = ambientCoeficient;
            Camera = camera;
            Solids = solids;
            LightSources = lightSources;
        }
    }

    public class Camera
    {
        Vector3d Origin { get; set; }
        Vector3d Forward { get; set; }
        Vector3d Right { get; set; } = new Vector3d(1, 0, 0).Normalized();
        Vector3d Up { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }

        Vector3d[,] ResultPixels;
        
        public Camera(Vector3d origin, Vector3d forward, Vector3d up, double height, double width) //todo rotation, up and right vector, user shouldnt be able to choose any vector as the up and right vector, they have to be perpendicular to each other
        {
            Origin = origin;
            Forward = forward.Normalized();
            Up = up.Normalized();
            //Right = right.Normalized();
            Height = height;
            Width = width;

        }

        public static (ISolid?, double?) ThrowRay(Ray ray, List<ISolid> solids)
        {
            ISolid? result = null;
            double? t = null;
            foreach (ISolid solid in solids)
            {
                double? tmp = solid.Intersection(ray);
                if( tmp != null && t == null && tmp > 0.6)
                {
                    t = tmp;
                    result = solid;
                }
                else if ( tmp != null && t != null && tmp > 0.6)
                {
                    if( tmp < t )
                    {
                        t = tmp;
                        result = solid;
                    }
                }
            }
            return (result, t);  
        }
        public static (ISolid?, double?) BounceRay(Ray ray, List<ISolid> solids, ISolid bouncedOf)
        {
            ISolid? result = null;
            double? t = null;
            foreach (ISolid solid in solids)
            {
                if(bouncedOf != solid)
                {
                    double? tmp = solid.Intersection(ray);
                    if (tmp != null && t == null)
                    {
                        t = tmp;
                        result = solid;
                    }
                    else if (tmp != null && t != null)
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

        private Ray GetRayFromCamera(int x, int y, int width, int height)
        {
            Ray ray = new Ray(Origin, (Forward + (x - width / 2) * (Width / width) * Right + (y - height / 2) * (Height / height) * Up).Normalized());
            return ray;
        }

        public Vector3d[,] RayCast( Scene scene, int width, int height )
        {
            Vector3d[,] pixels = new Vector3d[width, height];

            for ( int x = 0; x < width; x++ )
            {
                for ( int y = 0; y < height; y++ )
                {
                    Ray ray = GetRayFromCamera(x, y, width, height);

                    (ISolid?, double?) intersection = ThrowRay(ray, scene.Solids);

                    if(intersection.Item1 != null && intersection.Item2 != null)
                    {
                        Vector3d intersectionPoint = (Vector3d)(ray.Origin + ( intersection.Item2 * ray.Direction ));
                        pixels[x,y] = Phong.Shade( scene, ray, 0, 10); //maxdepth todo dej si to neka jiman 
                    }
                }
            }
            return pixels;
        }
    }
    /*
    static public class Parser
    {
        public Scene LoadScene(string path)
        {
            Dictionary<string, Material> Materials = new();
            Scene scene = new Scene(0.5, new Camera((0,0,0), (0,1,0), (0,0,1), 2, 2), new List<ISolid>(), new List<LightSource>());

            using (StreamReader sr = new StreamReader(path))
            {
                while( true)
                {
                    string? line = sr.ReadLine();
                    if(line == null)
                    {
                        break;
                    }
                    string[] tokens = line.Split(' ');
                    switch (tokens[0])
                    {
                        case "scene":
                            break;

                        case "camera":

                            Vector3d cameraOrigin = (0,0,0);
                            Vector3d cameraForward = (0,1,0);
                            Vector3d cameraUp = (0,0,1);
                            double cameraWidth = 2;
                            double cameraHeight = 2;
                            
                            for( int i = 1; i < tokens.Length; i++ )
                            {
                                string[] token = tokens[i].Split(":");
                                switch (token[0])
                                {
                                    case "origin":
                                        string[] origin = token[1].Split(",");
                                        cameraOrigin.X = Convert.ToDouble(origin[0]);
                                        cameraOrigin.Y = Convert.ToDouble(origin[1]);
                                        cameraOrigin.Z = Convert.ToDouble(origin[2]);
                                        break;
                                    case "width":
                                        cameraWidth = Convert.ToDouble(token[1]);
                                        break;
                                    case "height":
                                        cameraHeight = Convert.ToDouble(token[1]);
                                        break;
                                    case "forward":
                                        string[] forward = token[1].Split(",");
                                        cameraForward.X = Convert.ToDouble(forward[0]);
                                        cameraForward.Y = Convert.ToDouble(forward[1]);
                                        cameraForward.Z = Convert.ToDouble(forward[2]);
                                        break;
                                    case "up":
                                        string[] up = token[1].Split(",");
                                        cameraUp.X = Convert.ToDouble(up[0]);
                                        cameraUp.Y = Convert.ToDouble(up[1]);
                                        cameraUp.Z = Convert.ToDouble(up[2]);
                                        break;
                                    default:
                                        break;
                                }
                            }
                            camera = new Camera(cameraOrigin, cameraForward, cameraUp, cameraHeight, cameraWidth);
                            break;

                        case "material":
                            string name;
                            Vector3d color;
                            double diffusion;
                            double specularity;
                            double glossiness;

                            break;

                        case "solids(":
                            //Solids = LoadSolids();
                            break;

                        default:
                            Console.WriteLine("This line is a no no:\n " + line);
                            break;
                    }    
                }
            }
            return scene = new Scene(ambient, camera, );
        }

    }
    */
    private static FloatImage GenerateCoolImage(FloatImage image)
    {
        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                float value1 = (float)(Math.Cos(y / 2));
                float value2 = (float)(Math.Cos(x*x));
                float[] pixel = new float[3] { value1, value1 * value2, value2 };
                image.PutPixel(x, y, pixel);
            }
        }
        return image;
    }

    static void Main(string[] args)
    {
        // Parameters.
        // TODO: parse command-line arguments and/or your config file.
        //Config config = new Config();
        //config.LoadFromFile("config.txt");

        Scene demo = new Scene(0.2, new Camera((0,0,0), (0,1,0), (0,0,-1), 2, 2), new List<ISolid>(), new List<LightSource>()); //ambientLight, camera(origin, forward, up, width, height)

        Material mat1 = new Material(new Vector3d(0, 0, 1), 0.4, 0.5, 50, 0); //color, diffusionCoeff, specularCoeff, glossiness 
        Material mat3 = new Material(new Vector3d(1, 0, 0), 0.4, 0.4, 5, 0.5);
        Material mat2 = new Material(new Vector3d(0, 1.5, 0), 0.4, 0.3, 5, 0.5);

        Plane planius = new Plane(mat3, (0, 100, 0), new Vector3d(0, 1, 0).Normalized()); //material, origin, normalVector
        Plane planius2 = new Plane(mat1, (0, 800, 0), new Vector3d(1, 1, 0).Normalized());

        Sphere spherocious = new Sphere(mat1, new Vector3d(-25, 40, -10), 10); //material, origin, size
        Sphere spherocious2 = new Sphere(mat3, new Vector3d(0, 40, 0), 10);
        Sphere babz = new Sphere(mat2, new Vector3d(25, 40, 10), 10);

        LightSource light = new LightSource((0, 0, 0), (-1, 0, 0), (1, 1, 1), 1); //origin, direction, color, intensity
        LightSource light2 = new LightSource((0, 0, 0), (0, 500, 300), (1, 1, 1), 1);

        //demo.Solids.Add(planius);
        //demo.Solids.Add(planius2);
        //demo.Solids.Add(new Plane(mat2, (0, 800, 0), new Vector3d(0, -1, 1).Normalized()));
        
        demo.Solids.Add(spherocious);
        demo.Solids.Add(spherocious2);
        demo.Solids.Add(babz);
        
        demo.LightSources.Add(light);
        //demo.LightSources.Add(light2);



        FloatImage fi = new FloatImage(1200, 1200, 3);

        Vector3d[,] grid = demo.Camera.RayCast(demo, fi.Width, fi.Height);

        for (int y = 0; y < fi.Height; y++)
        {
            for (int x = 0; x < fi.Width; x++)
            {
                float[] color = new float[3];
                color[0] = (float)grid[x, y].X;
                color[1] = (float)grid[x, y].Y;
                color[2] = (float)grid[x, y].Z;

                fi.PutPixel(x, y, color);
            }
        }

        string fileName = "config.pfm";

        /* HDR image.
        FloatImage fi = new FloatImage(wid, hei, 3);
        fi = GenerateCoolImage(fi);
        */


        // TODO: put anything interesting into the image.
        // TODO: use fi.PutPixel() function, pixel should be a float[3] array [R, G, B]

        //fi.SaveHDR(fileName);   // Doesn't work well yet...
        fi.SavePFM(fileName);

        Console.WriteLine("HDR image is finished.");
    }
}
