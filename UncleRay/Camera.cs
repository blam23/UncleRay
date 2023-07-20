using System.Numerics;

namespace UncleRay;

internal readonly struct Camera
{
    private readonly float aspectRatio;
    private readonly float viewportHeight;
    private readonly float viewportWidth;
    private readonly Vector3 origin;
    private readonly Vector3 hortizontal;
    private readonly Vector3 vertical;
    private readonly Vector3 lowerLeftCorner;
    private readonly Vector3 w, u, v;
    private readonly float lensRadius;

    public Camera(Vector3 position, Vector3 lookAt, Vector3 up, float fov, float aspectRatio, float aperture, float focusDistance)
    {
        this.aspectRatio = aspectRatio;

        // Setup FoV
        var theta = (MathF.PI / 180) * fov;
        var h = MathF.Tan(theta / 2);
        viewportHeight = 2f * h;
        viewportWidth = viewportHeight * this.aspectRatio;

        // Setup positon & angle
        w = Vector3.Normalize(position- lookAt);
        u = Vector3.Normalize(Vector3.Cross(up, w));
        v = Vector3.Cross(w, u);

        origin = position;
        hortizontal = focusDistance * viewportWidth * u;
        vertical = focusDistance * viewportHeight * v;
        lowerLeftCorner = origin - (hortizontal / 2) - (vertical / 2) - focusDistance * w;

        lensRadius = aperture / 2;
    }

    public Ray CastRay(Random rng, float s, float t)
    {
        var randDisk = lensRadius * VecHelpers.RandomUnitDisk(rng);
        var offset = u * randDisk.X + v * randDisk.Y;

        return new()
        {
            Origin = origin + offset,
            Direction = lowerLeftCorner + s * hortizontal + t * vertical - origin - offset
        };
    }
}
