using System.Numerics;

namespace UncleRay;

internal static class VecHelpers
{
    private static float RandFloat(Random rng) => (float)rng.NextDouble();

    public static Vector3 RandomRange(Random rng, float range) =>
        new(
            RandFloat(rng) * range - (range / 2f),
            RandFloat(rng) * range - (range / 2f),
            RandFloat(rng) * range - (range / 2f));

    public static Vector3 RandomUnitSphere(Random rng) => RandomRange(rng, 1);
    public static Vector3 RandomUnitVector(Random rng) => Vector3.Normalize(RandomRange(rng, 1));
    
    public static Vector3 RandomUnitDisk(Random rng)
    {
        while(true)
        {
            var v = new Vector3(RandFloat(rng) * 2f - 0.5f, RandFloat(rng) * 2f - 0.5f, 0f);

            if (v.LengthSquared() < 1)
                return v;
        }
    }

    public static Vector3 RandomInHemisphere(Random rng, Vector3 normal)
    {
        var ruv = RandomUnitVector(rng);
        return Vector3.Dot(ruv, normal) > 0 ? ruv : -ruv;
    }

    public static bool NearZero(Vector3 v)
    {
        var s = 1e-8;
        v = Vector3.Abs(v);
        return v.X < s && v.Y < s && v.Z < s;
    }

    public static Vector3 Reflect(Vector3 v, Vector3 n) => v - 2 * Vector3.Dot(v, n) * n;

    public static Vector3 Refract(Vector3 uv, Vector3 normal, float ratio, float? cosThetaIn = null)
    {
        var cosTheta = cosThetaIn ?? MathF.Min(Vector3.Dot(-uv, normal), 1f);
        var perpendicular = ratio * (uv + cosTheta * normal);
        var parallel = -MathF.Sqrt(MathF.Abs(1f - perpendicular.LengthSquared())) * normal;
        return perpendicular + parallel;
    }
}
