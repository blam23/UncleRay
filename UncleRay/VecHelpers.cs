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
    
}
