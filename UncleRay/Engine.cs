using System.Diagnostics;
using System.Numerics;

namespace UncleRay;

public class Engine
{
    // Image
    public readonly byte[] Data;
    private readonly int width;
    private readonly int height;
    private const int BytesPerPixel = 3;

    private Camera camera;
    private int RaysPerPixel = 15;

    private long startTime = 0;
    private TimeSpan deltaTime;

    private readonly Random rng = new(123);

    public Engine(int width, int height)
    {
        this.width = width;
        this.height = height;
        Data = new byte[width * height * BytesPerPixel];

        camera = new Camera((float)width / height);

        startTime = Stopwatch.GetTimestamp();
    }

    void WriteBMP(BinaryWriter writer)
    {
        // Manually write out a BMP file so we don't have to include any external libs

        // BMP Header
        writer.Write((byte)'B');
        writer.Write((byte)'M');
        writer.Write((uint)Data.Length);

        // BMP Core Header
        writer.Seek(14, SeekOrigin.Begin);
        writer.Write(40);
        writer.Write(width);
        writer.Write(height);
        writer.Write((ushort)1);
        writer.Write((ushort)(BytesPerPixel * 8));
        writer.Seek(34, SeekOrigin.Begin);
        writer.Write((uint)Data.Length);

        // Data
        writer.Seek(54, SeekOrigin.Begin);
        writer.Write(Data);
    }

    private void WritePixel(int x, int y, Vector3 color)
    {
        var index = (x + (height - y - 1) * width) * BytesPerPixel;
        Data[index + 0] = (byte)(color.Z * 255);
        Data[index + 1] = (byte)(color.Y * 255);
        Data[index + 2] = (byte)(color.X * 255);
    }

    public void SaveImage(string path)
    {
        using var writer = new BinaryWriter(File.Create(path));
        WriteBMP(writer);
    }

    private float HitSphere(Vector3 center, float radius, Ray r)
    {
        var oc = r.Origin - center;

        // Solve the quadratic equation to see if we have intersections
        var a = Vector3.DistanceSquared(Vector3.Zero, r.Direction);
        var halfB = Vector3.Dot(oc, r.Direction);
        var c = Vector3.DistanceSquared(Vector3.Zero, oc) - (radius * radius);
        var disc = (halfB * halfB) - (a * c);

        if (disc < 0)
            return -1f;

        return (-halfB - MathF.Sqrt(disc)) / a;
    }

    private Vector3 RayColor(Ray r)
    {
        var t = HitSphere(new Vector3(0f, 0f, -1f), 0.1f * MathF.Sin((float)deltaTime.TotalMilliseconds * 0.001f) + 0.5f, r);

        if (t > 0f)
        {
            var n = Vector3.Normalize(r.At(t) - new Vector3(0, 0, -1));
            return 0.5f * new Vector3(n.X + 1, n.Y + 1, n.Z + 1);
        }

        var ud = Vector3.Normalize(r.Direction);
        t = 0.5f * (ud.Y + 1);
        return (1.0f - t) * Vector3.One + t * new Vector3(0.5f, 0.7f, 1.0f);
    }

    public void Render()
    {
        var start = Stopwatch.GetTimestamp();
        deltaTime = new TimeSpan(start - startTime);

        for(int y = height - 1; y >= 0; --y)
        {
            for (int x = 0; x < width; ++x)
            {
                var u = (float)x / (width - 1);
                var v = (float)y / (height - 1);

                Vector3 pixel = Vector3.Zero;
                for (var i = 0; i < RaysPerPixel; ++i)
                {
                    Ray r = new()
                    {
                        Origin = camera.Origin + VecHelpers.RandomRange(rng, 0.002f),
                        Direction = camera.LLC + u * camera.Horizontal + v * camera.Vertical - camera.Origin
                    };

                    pixel += RayColor(r);
                }

                pixel /= RaysPerPixel;

                WritePixel(x, y, pixel);
            }
        }

        //var elapsed = new TimeSpan(Stopwatch.GetTimestamp() - start);
        //Console.WriteLine($"Render time: {elapsed.Microseconds/1000f:0.00}ms");
    }

    public bool TryDebugScene(string img)
    {
        Render();
        SaveImage(img);
        return true;
    }
}