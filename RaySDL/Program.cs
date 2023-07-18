using UncleRay;
using UncleRay.SDL;

var (width, height) = (248, 144);

var renderer = new Engine(width, height);
var display = new Main(width, height, 5);
var running = true;

var dispThread = new Thread(() =>
{
    while (running)
    {
        renderer.Render();
        display.SetPixels(renderer.Data);
    }
});

dispThread.Start();
display.Run();

running = false;

display.Dispose();
