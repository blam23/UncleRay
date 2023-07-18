using System.Numerics;

namespace UncleRay;

internal readonly struct Camera
{
    public readonly float AspectRatio;
    public readonly float ViewportHeight = 2;
    public readonly float ViewportWidth;
    public readonly float FocalLength = 1;
    public readonly Vector3 Origin = Vector3.Zero;
    public readonly Vector3 Horizontal;
    public readonly Vector3 Vertical;
    public readonly Vector3 LLC;

    public Camera(float aspectRatio)
    {
        AspectRatio = aspectRatio;
        ViewportWidth = ViewportHeight * AspectRatio;

        Horizontal = new Vector3(ViewportWidth, 0, 0);
        Vertical = new Vector3(0, ViewportHeight, 0);
        LLC = Origin - (Horizontal / 2) - (Vertical / 2) - new Vector3(0, 0, FocalLength);
    }
}
