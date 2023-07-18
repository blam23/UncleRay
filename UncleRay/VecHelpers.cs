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
    public static Vector3 RandomInHemisphere(Random rng, Vector3 normal)
    {
        var ruv = RandomUnitVector(rng);
        return Vector3.Dot(ruv, normal) > 0 ? ruv : -ruv;
    }

    internal static bool NearZero(Vector3 v)
    {
        var s = 1e-8;
        v = Vector3.Abs(v);
        return v.X < s && v.Y < s && v.Z < s;
    }
}
