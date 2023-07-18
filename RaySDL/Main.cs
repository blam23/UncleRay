using Silk.NET.Maths;
using Silk.NET.SDL;

namespace UncleRay.SDL;

internal unsafe class Main
    : IDisposable
{
    // Setup SDL
    internal static readonly Sdl SDL;

    static Main()
    {
        SDL = Sdl.GetApi();

        var err = SDL.Init(Sdl.InitEverything);
        if (err < 0)
            throw new Exception($"Unable to load SDL, error: {err}");
    }

    private readonly SDLSurfaceWindow window;
    private readonly Renderer* renderer;

    private bool running = true;

    public Main(int width, int height, int scale)
    {
        window = new SDLSurfaceWindow(width, height, scale, "UncleRay");
        renderer = SDL.CreateRenderer(window.handle, -1, (uint)(RendererFlags.Accelerated));

        var frameTimer = new Timer((e) =>
        {
            window.SetTitle($"UncleRay - FPS: {videoFrames}");

            videoFrames = 0;
        });
        frameTimer.Change(0, 1000);

        if (renderer == default(Renderer*))
            throw new Exception("Unable to create SDL renderer.");
    }

    int videoFrames = 0;

    internal static void @throw(Func<int> sdlCall)
    {
        if (sdlCall() < 0)
            throw new Exception("SDL call failed");
    }

    public void SetPixels(byte[] data)
    {
        window.SetPixels(data);
        videoFrames++;
    }

    public void Run()
    {
        ulong clock = 0;
        while (running)
        {
            // Check event queue
            Event evt;
            while (SDL.PollEvent(&evt) == 1)
                HandleEvent(evt);

            //DrawDebug(clock);

            window.BlitSurface();

            clock++;
            clock %= 100;

        }
    }

    private void HandleEvent(Event evt)
    {
        var type = (EventType)evt.Type;
        switch (type)
        {
            case EventType.Quit:
                running = false;
                break;

            default:
                break;
        }
    }

    private bool disposed;

    protected virtual void Dispose(bool disposing)
    {
        if (disposed)
            return;

        if (disposing)
        {
            window.Dispose();
            SDL.Quit();
        }

        disposed = true;
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
