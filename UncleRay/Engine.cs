using System.Diagnostics;
using System.Numerics;
using UncleRay.Material;

namespace UncleRay;

public class Engine
{
    // Image
    public readonly byte[] Data;
    private readonly int width;
    private readonly int height;
    private const int BytesPerPixel = 3;

    // Camera
    private readonly Camera camera;

    // Quality
    private const int raysPerPixel = 150;
    private const int maxDepth = 50;

    // World
    private readonly ObjectList objects = new();

    // Timer
    private readonly long startTime = 0;
    private TimeSpan deltaTime;

    public Engine(int width, int height)
    {
        this.width = width;
        this.height = height;
        Data = new byte[width * height * BytesPerPixel];

        camera = new Camera((float)width / height);

        startTime = Stopwatch.GetTimestamp();

        var redMatte = new Matte(new Vector3(1, 0, 0));
        var blueMatte = new Matte(new Vector3(0, 0, 1));
        objects.Add(new Sphere(new(0, 0, -1), 0.5f,  redMatte));
        objects.Add(new Sphere(new(0, -100.5f, -1), 100, blueMatte));
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

    private Vector3 RayColor(Random rng, Ray r, int depth = 0)
    {
        if (depth >= maxDepth)
            return Vector3.Zero;

        if (objects.Hit(r, 0.001f, float.PositiveInfinity, out var hit))
        {
            if (hit.Material.Scatter(rng, r, hit, out var color, out var newRay))
            {
                return color * RayColor(rng, newRay, depth + 1);
            }

            return Vector3.Zero;
        }

        var ud = Vector3.Normalize(r.Direction);
        var t = 0.5f * (ud.Y + 1);
        return (1.0f - t) * Vector3.One + t * new Vector3(0.5f, 0.7f, 1.0f);
    }

    class RenderChunkData
    {
        public int StartY, EndY;
        public EventWaitHandle WaitHandle;

        public RenderChunkData(int startY, int endY, EventWaitHandle waitHandle)
        {
            StartY = startY;
            EndY = endY;
            WaitHandle = waitHandle;
        }
    }

    public void Render()
    {
        var start = Stopwatch.GetTimestamp();
        deltaTime = new TimeSpan(start - startTime);

        var handles = new List<EventWaitHandle>();
        var chunkSize = height / Environment.ProcessorCount;
        for (var y = height - 1; y >= 0; y -= chunkSize)
        {
            var end = y - chunkSize;
            if (end < 0)
                end = 0;

            var ewh = new EventWaitHandle(false, EventResetMode.ManualReset);
            handles.Add(ewh);

            ThreadPool.QueueUserWorkItem(RenderCallback, new RenderChunkData(y, end, ewh));
        }

        foreach (var handle in handles)
        {
            handle.WaitOne();
        }
    }

    public void RenderCallback(object? handle)
    {
        if (handle is RenderChunkData chunk)
        {
            RenderChunk(chunk.StartY, chunk.EndY);
            chunk.WaitHandle.Set();
        }
    }

    public void RenderChunk(int startY, int endY)
    {
        // Same seed each frame
        Random rng = new(startY);

        if (objects[0] is Sphere s1)
            s1.Radius = 0.1f * MathF.Sin((float)deltaTime.TotalMilliseconds * 0.001f) + 0.5f;

        for(int y = startY; y >= endY; --y)
        {
            for (int x = 0; x < width; ++x)
            {
                Vector3 pixel = Vector3.Zero;
                for (var i = 0; i < raysPerPixel; ++i)
                {
                    var u = (x + (float)rng.NextDouble()) / (width - 1);
                    var v = (y + (float)rng.NextDouble()) / (height - 1);

                    Ray r = new()
                    {
                        Origin = camera.Origin,
                        Direction = camera.LLC + u * camera.Horizontal + v * camera.Vertical - camera.Origin
                    };

                    pixel += RayColor(rng, r);
                }

                pixel /= raysPerPixel;
                pixel = Vector3.SquareRoot(pixel);
                Vector3.Clamp(pixel, Vector3.Zero, Vector3.One);

                WritePixel(x, y, pixel);
            }
        }
    }

    public bool TryDebugScene(string img)
    {
        Render();
        SaveImage(img);
        return true;
    }
}