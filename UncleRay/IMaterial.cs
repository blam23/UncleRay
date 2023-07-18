using System.Numerics;

namespace UncleRay;

public interface IMaterial
{
    public bool Scatter(Random rng, Ray rayIn, HitData hit, out Vector3 color, out Ray rayOut);
}
