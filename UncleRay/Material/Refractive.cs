using System.Numerics;

namespace UncleRay.Material;

internal class Refractive : IMaterial
{
    public Vector3 Albedo;
    public float RefractionIndex;
    public float Opacity;
    public float Blur;

    public Refractive(Vector3 albedo, float refractionIndex, float opacity, float blur)
    {
        Albedo = albedo;
        RefractionIndex = refractionIndex;
        Opacity = opacity;
        Blur = blur;
    }

    public bool Scatter(Random rng, Ray rayIn, HitData hit, out Vector3 color, out Ray rayOut)
    {

        if (rng.NextDouble() > Opacity)
        {
            rayOut = new Ray { Origin = hit.Point, Direction = VecHelpers.RandomInHemisphere(rng, hit.Normal) };
            color = Albedo;
        }
        else
        {
            var ratio = hit.FrontFace ? 1f / RefractionIndex : RefractionIndex;

            var ud = Vector3.Normalize(rayIn.Direction);

            var cosTheta = MathF.Min(Vector3.Dot(-ud, hit.Normal), 1f);
            var sinTheta = MathF.Sqrt(1f - cosTheta * cosTheta);

            bool canRefract = ratio * sinTheta <= 1.0f;

            var direction = (canRefract || Reflectance(cosTheta, ratio) > (float)rng.NextDouble())
                ? VecHelpers.Refract(ud, hit.Normal, ratio, cosTheta)
                : VecHelpers.Reflect(ud, hit.Normal);

            rayOut = new() { Origin = hit.Point, Direction = direction + VecHelpers.RandomInHemisphere(rng, hit.Normal) * Blur };
            color = Vector3.One;
        }

        return true;
    }

    private float Reflectance(float cosine, float ratio)
    {
        // Schlick's Approx. for reflectance
        var r0 = (1 - ratio) / (1 + ratio);
        r0 *= r0;
        return r0 + (1 - r0) * MathF.Pow((1 - cosine), 5);
    }
}
