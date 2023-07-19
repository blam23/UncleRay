using System.Numerics;

namespace UncleRay.Material;

internal class Metal : IMaterial
{
    public Vector3 Albedo;
    public readonly float Blur; // Todo: make a property so it can be changed programatically

    public Metal(Vector3 albedo, float blur)
    {
        Albedo = albedo;
        Blur = blur < 1 ? blur : 1;
    }

    public bool Scatter(Random rng, Ray rayIn, HitData hit, out Vector3 color, out Ray rayOut)
    {
        var reflected = VecHelpers.Reflect(rayIn.Direction, hit.Normal);

        if (Blur > 0)
            rayOut = new Ray { Origin = hit.Point, Direction = reflected + (VecHelpers.RandomUnitSphere(rng) * Blur) };
        else
            rayOut = new Ray { Origin = hit.Point, Direction = reflected };

        color = Albedo;

        return Vector3.Dot(rayOut.Direction, hit.Normal) > 0;
    }
}
