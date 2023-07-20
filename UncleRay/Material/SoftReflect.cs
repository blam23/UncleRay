using System.Numerics;

namespace UncleRay.Material;

internal class SoftReflect : IMaterial
{
    public Vector3? ReflectAlbedo = Vector3.One;
    public Vector3 FlatAlbedo;
    public float ReflectChance;
    public float Blur;

    public SoftReflect(Vector3 albedo, float reflectChance, float blur)
    {
        FlatAlbedo = albedo;
        ReflectChance = reflectChance;
        Blur = blur;
    }

    public bool Scatter(Random rng, Ray rayIn, HitData hit, out Vector3 color, out Ray rayOut)
    {
        if (rng.NextDouble() > ReflectChance)
        {
            rayOut = new Ray { Origin = hit.Point, Direction = VecHelpers.RandomInHemisphere(rng, hit.Normal) * Blur };
            color = FlatAlbedo;
        }
        else
        {
            rayOut = new Ray { Origin = hit.Point, Direction = VecHelpers.Reflect(rayIn.Direction, hit.Normal) + VecHelpers.RandomInHemisphere(rng, hit.Normal) * Blur };
            color = ReflectAlbedo ?? FlatAlbedo;
        }

        return true;
    }
}
