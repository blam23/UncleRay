using Silk.NET.Maths;
using Silk.NET.SDL;

namespace UncleRay.SDL;

internal unsafe class SDLSurfaceWindow : WrappedWindow
{
    private readonly Surface* windowSurface;
    private readonly Surface* pixelSurface;

    private Rectangle<int> windowRect;
    private Rectangle<int> pixelRect;

    public SDLSurfaceWindow(int width, int height, int scale, string title) : base(width * scale, height * scale, title)
    {
        pixelSurface = Main.SDL.CreateRGBSurfaceWithFormat(0, width, height, 0, (uint)PixelFormatEnum.Bgr24);
        windowSurface = Main.SDL.GetWindowSurface(handle);
        windowRect = new Rectangle<int>(0, 0, width * scale, height * scale);
        pixelRect = new Rectangle<int>(0, 0, width, height);
    }

    public void SetPixels(byte[] data)
    {
        Main.SDL.Memcpy(pixelSurface->Pixels, in data[0], (nuint)data.Length);
    }

    public void BlitSurface()
    {
        Main.@throw(() => Main.SDL.LowerBlitScaled(pixelSurface, ref pixelRect, windowSurface, ref windowRect));
        Main.SDL.UpdateWindowSurface(handle);
    }
}
