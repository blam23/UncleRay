using System.Numerics;

namespace UncleRay;

public struct HitData
{
    public bool Hit;
    public Vector3 Point;
    public Vector3 Normal;
    public float T;
    public bool FrontFace;
    public IMaterial Material;

    public void SetFaceNormal(Ray r, Vector3 normal)
    {
        FrontFace = Vector3.Dot(r.Direction, normal) < 0;
        Normal = FrontFace ? normal : -normal;
    }
}
