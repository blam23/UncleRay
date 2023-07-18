using System.Diagnostics;
﻿using System.Numerics;

namespace UncleRay;

public class Engine
{
    // Image
    private readonly byte[] data;
    private readonly int width;
    private readonly int height;
    private const int BytesPerPixel = 4;

    private Camera camera;

    private readonly Random rng = new(123);

    public Engine(int width, int height)
    {
        this.width = width;
        this.height = height;
        data = new byte[width * height * BytesPerPixel];

        camera = new Camera((float)width / height);
    }

    void WriteBMP(BinaryWriter writer)
    {
        // Manually write out a BMP file so we don't have to include any external libs

        // BMP Header
        writer.Write((byte)'B');
        writer.Write((byte)'M');
        writer.Write((uint)data.Length);

        // BMP Core Header
        writer.Seek(14, SeekOrigin.Begin);
        writer.Write(40);
        writer.Write(width);
        writer.Write(height);
        writer.Write((ushort)1);
        writer.Write((ushort)(BytesPerPixel * 8));
        writer.Seek(34, SeekOrigin.Begin);
        writer.Write((uint)data.Length);

        // Data
        writer.Seek(54, SeekOrigin.Begin);
        writer.Write(data);
    }

    private void WritePixel(int x, int y, Vector3 color)
    {
        var index = (x + y * width) * BytesPerPixel;
        data[index + 0] = (byte)(color.Z * 255);
        data[index + 1] = (byte)(color.Y * 255);
        data[index + 2] = (byte)(color.X * 255);
    }

    public void SaveImage(string path)
    {
        using var writer = new BinaryWriter(File.Create(path));
        WriteBMP(writer);
    }

    private Vector3 RayColor(Ray r)
    {
        var ud = Vector3.Normalize(r.Direction);

        // Just lerp between blue and white depending on Y
        var t = 0.5f * (ud.Y + 1);
        return (1f - t) * Vector3.One + t * new Vector3(0.4f, 0.7f, 1.0f);
    }

    public void Render()
    {
        var start = Stopwatch.GetTimestamp();

        for(int y = height - 1; y >= 0; --y)
        {
            for (int x = 0; x < width; ++x)
            {
                var u = (float)x / (width - 1);
                var v = (float)y / (height - 1);

                Ray r = new()
                {
                    Origin = camera.Origin,
                    Direction = camera.LLC + u*camera.Horizontal + v*camera.Vertical - camera.Origin
                };

                var pixel = RayColor(r);
                WritePixel(x, y, pixel);
            }
        }
        var elapsed = new TimeSpan(Stopwatch.GetTimestamp() - start);
        Console.WriteLine($"Render time: {elapsed.Microseconds/1000f:0.00}ms");
    }

    public bool TryDebugScene(string img)
    {
        Render();
        SaveImage(img);
        return true;
    }
}