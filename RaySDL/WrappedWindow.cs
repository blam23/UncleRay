using Silk.NET.SDL;

namespace UncleRay.SDL;

internal unsafe class WrappedWindow
    : IDisposable
{
    public Window* handle;

    private readonly int width;
    private readonly int height;

    public WrappedWindow(int width, int height, string title)
    {
        this.width = width;
        this.height = height;

        handle = Main.SDL.CreateWindow(title, width, height, width, height, (uint)(WindowFlags.Opengl));
    }

    public void SetTitle(string title) => Main.SDL.SetWindowTitle(handle, title);

    private bool disposed;
    protected virtual void Dispose(bool disposing)
    {
        if (disposed)
            return;

        if (disposing)
        {
            if (handle != default(Window*))
            {
                Main.SDL.DestroyWindow(handle);
                handle = default;
            }
        }

        disposed = true;
    }
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
