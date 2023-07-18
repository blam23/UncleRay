using System.Numerics;

namespace UncleRay.Material;

internal class Matte : IMaterial
{
    public Vector3 Albedo;

    public Matte(Vector3 albedo)
    {
        Albedo = albedo;
    }

    public bool Scatter(Random rng, Ray rayIn, HitData hit, out Vector3 color, out Ray rayOut)
    {
        var scatterDir = hit.Normal + VecHelpers.RandomUnitVector(rng);

        if (VecHelpers.NearZero(scatterDir))
            scatterDir = hit.Normal;

        rayOut = new Ray { Origin = hit.Point, Direction = scatterDir };
        color = Albedo;

        return true;
    }
}
