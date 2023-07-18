using System.Numerics;

namespace UncleRay;

internal struct Ray
{
    public Vector3 Origin;
    public Vector3 Direction;

    public Vector3 At(float t) => Origin + t * Direction;
}
