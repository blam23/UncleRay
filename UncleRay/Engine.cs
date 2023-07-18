using System.Drawing;

namespace UncleRay;

public class Engine
{
    private byte[] data;
    private int width;
    private int height;

    private const int BytesPerPixel = 4;

    private Random rng = new Random(123);

    public Engine(int width, int height)
    {
        this.width = width;
        this.height = height;
        data = new byte[width * height * BytesPerPixel];
    }

    void WriteBMP(BinaryWriter writer)
    {
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

    private void WritePixel(int x, int y, Color color)
    {
        var index = (x + ((height - y - 1) * width)) * BytesPerPixel;
        data[index + 0] = color.B;
        data[index + 1] = color.G;
        data[index + 2] = color.R;
    }

    public bool TryDebugScene(out string img)
    {
        // TODO: This should obviously be passed in (and not a random full path)
        img = $"D:\\Test\\debug.bmp";

        using var writer = new BinaryWriter(File.Create(img));
        WriteBMP(writer);

        return true;
    }
}