using System.Numerics;

namespace UncleRay;

internal readonly struct Camera
{
    public readonly float AspectRatio;
    public readonly float ViewportHeight;
    public readonly float ViewportWidth;
    public readonly Vector3 Origin;
    public readonly Vector3 Horizontal;
    public readonly Vector3 Vertical;
    public readonly Vector3 LLC;

    public Camera(Vector3 position, Vector3 lookAt, Vector3 up, float fov, float aspectRatio)
    {
        AspectRatio = aspectRatio;

        // Setup FoV
        var theta = (MathF.PI / 180) * fov;
        var h = MathF.Tan(theta / 2);
        ViewportHeight = 2f * h;
        ViewportWidth = ViewportHeight * AspectRatio;

        // Setup positon & angle
        var w = Vector3.Normalize(position- lookAt);
        var u = Vector3.Normalize(Vector3.Cross(up, w));
        var v = Vector3.Cross(w, u);

        Origin = position;
        Horizontal = ViewportWidth * u;
        Vertical = ViewportHeight * v;
        LLC = Origin - (Horizontal / 2) - (Vertical / 2) - w;
    }
}
