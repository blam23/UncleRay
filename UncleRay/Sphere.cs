using System.Numerics;

namespace UncleRay;

internal class Sphere : IObject
{
    public Vector3 Origin;
    public float Radius;
    public IMaterial Material;

    public Sphere(Vector3 origin, float radius, IMaterial material)
    {
        Origin = origin;
        Radius = radius;
        Material = material;
    }

    public bool Hit(Ray r, float minT, float maxT, out HitData hit)
    {
        hit = default;
        var oc = r.Origin - Origin;

        // Solve the quadratic equation to see if we have intersections
        var a = Vector3.DistanceSquared(Vector3.Zero, r.Direction);
        var halfB = Vector3.Dot(oc, r.Direction);
        var c = Vector3.DistanceSquared(Vector3.Zero, oc) - (Radius * Radius);
        var disc = (halfB * halfB) - (a * c);

        // No intersections
        if (disc < 0)
            return false;

        // Get the first intersection (and it's data)
        var sqrtDisc = MathF.Sqrt(disc);

        var root = (-halfB - sqrtDisc) / a;
        if (root < minT || root >= maxT)
        {
            root = (-halfB + sqrtDisc) / a;
            if (root < minT || root >= maxT)
                return false;
        }

        hit.Hit = true;
        hit.T = root;
        hit.Point = r.At(root);
        hit.SetFaceNormal(r, (hit.Point - Origin) / Radius);
        hit.Material = Material;

        return true;
    }
}
