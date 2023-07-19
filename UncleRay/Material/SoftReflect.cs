using System.Numerics;

namespace UncleRay.Material;

internal class SoftReflect : IMaterial
{
    public Vector3 Albedo;
    public float ReflectChance;
    public float Blur;

    public SoftReflect(Vector3 albedo, float reflectChance, float blur)
    {
        Albedo = albedo;
        ReflectChance = reflectChance;
        Blur = blur;
    }

    public bool Scatter(Random rng, Ray rayIn, HitData hit, out Vector3 color, out Ray rayOut)
    {
        if (rng.NextDouble() > ReflectChance)
        {
            rayOut = new Ray { Origin = hit.Point, Direction = VecHelpers.RandomInHemisphere(rng, hit.Normal) * Blur };
        }
        else
        {
            rayOut = new Ray { Origin = hit.Point, Direction = VecHelpers.Reflect(rayIn.Direction, hit.Normal) + VecHelpers.RandomInHemisphere(rng, hit.Normal) * Blur };
        }
        color = Albedo;

        return true;
    }
}
