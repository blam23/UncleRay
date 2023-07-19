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
    public int RaysPerPixel = 500;
    public int MaxDepth = 200;

    // World
    private readonly ObjectList objects = new();

    public Engine(int width, int height)
    {
        this.width = width;
        this.height = height;
        Data = new byte[width * height * BytesPerPixel];

        camera = new Camera(70, (float)width / height);

        var miniMainBall = new Refractive(new Vector3(1f, 1f, 1f), 1.1f, 1f, 0.1f);
        var bigSideBalls = new Metal(new Vector3(1f, 1f, 1f), 0.1f);
        var mainBall = new SoftReflect(new Vector3(0.2f, 0.2f, 1f), 0.9f, 0.1f);
        var sideBalls = new Metal(new Vector3(1f, 0f, 1f), 0.1f);
        var floorBall = new SoftReflect(new Vector3(1f, 1f, 1f), 1f, 0.1f);
        objects.Add(new Sphere(new(0, 0.4f, -2f), 0.8f, mainBall));
        objects.Add(new Sphere(new(0, -0.2f, -0.9f), 0.25f, miniMainBall));
        objects.Add(new Sphere(new(-0.4f, -0.2f, -1.3f), 0.25f, sideBalls));
        objects.Add(new Sphere(new(0.4f, -0.2f, -1.3f), 0.25f, sideBalls));
        objects.Add(new Sphere(new(1, 0, -1.5f), 0.38f, bigSideBalls));
        objects.Add(new Sphere(new(-1, 0, -1.5f), 0.38f, bigSideBalls));
        objects.Add(new Sphere(new(0, -100.5f, -1), 100, floorBall));

        //GenerateSpheres();
    }

    private void GenerateSpheres()
    {
        Random rng = new(1233);

        var GenMetal = (Random rng) =>
            new Metal(VecHelpers.RandomUnitVector(rng), (float)rng.NextDouble());

        var GenMatte = (Random rng) =>
            new Matte(VecHelpers.RandomUnitVector(rng));

        for (var i = 2; i < 15; i++)
        {
            IMaterial material = rng.NextDouble() > 0.5 ? GenMetal(rng) : GenMatte(rng);
            var pos = new Vector3((float)rng.NextDouble(), -0.5f, -i * 0.1f);
            objects.Add(new Sphere(pos, (float)rng.NextDouble() * 0.1f, material));
        }
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
        if (depth >= MaxDepth)
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
        return (1.0f - t) * new Vector3(0.9f, 0.9f, 0.9f) + t * new Vector3(0.0f, 0.9f, 0.9f);
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
        RenderWithPartials(null);
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
        Random rng = new(startY);

        for(int y = startY; y >= endY; --y)
        {
            for (int x = 0; x < width; ++x)
            {
                Vector3 pixel = Vector3.Zero;
                for (var i = 0; i < RaysPerPixel; ++i)
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

                pixel /= RaysPerPixel;
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

    public void RenderWithPartials(Action<byte[]>? renderUpdate, int renderUpdateIntervalMS = 50)
    {
        bool rendering = true;
        var periodicUpdate = new Thread(() =>
            {
                while(rendering)
                    renderUpdate?.Invoke(Data);

                Thread.Sleep(renderUpdateIntervalMS);
            });
        periodicUpdate.Start();

        var start = Stopwatch.GetTimestamp();

        var handles = new List<EventWaitHandle>();
        var chunkSize = 6;
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

        rendering = false;
        periodicUpdate.Join();
    }
}