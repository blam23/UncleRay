using UncleRay;
using UncleRay.SDL;

var (width, height) = (620, 360);

var renderer = new Engine(width, height);
var display = new Main(width, height, 2);
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
