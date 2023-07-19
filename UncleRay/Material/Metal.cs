using System.Numerics;

namespace UncleRay.Material;

internal class Metal : IMaterial
{
    public Vector3 Albedo;
    public readonly float Fuzz; // Todo: make a property so it can be changed programatically

    public Metal(Vector3 albedo, float fuzz)
    {
        Albedo = albedo;
        Fuzz = fuzz < 1 ? fuzz : 1;
    }

    public bool Scatter(Random rng, Ray rayIn, HitData hit, out Vector3 color, out Ray rayOut)
    {
        var reflected = VecHelpers.Reflect(rayIn.Direction, hit.Normal);

        if (Fuzz < 1)
            rayOut = new Ray { Origin = hit.Point, Direction = reflected + VecHelpers.RandomUnitSphere(rng) };
        else
            rayOut = new Ray { Origin = hit.Point, Direction = reflected };

        color = Albedo;

        return Vector3.Dot(rayOut.Direction, hit.Normal) > 0;
    }
}
