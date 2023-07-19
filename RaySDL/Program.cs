using System.Diagnostics;
using UncleRay;
using UncleRay.SDL;

var (width, height) = (620, 360);

var renderer = new Engine(width, height);
using var display = new Main(width, height, 1);
var running = true;
var oneshot = true;

var dispThread = new Thread(() =>
{
    if (oneshot)
    {
        renderer.RaysPerPixel = 250;
        renderer.MaxDepth = 250;

        var start = Stopwatch.GetTimestamp();
        {
            renderer.Render();
            display.SetPixels(renderer.Data);
        }
        var end = Stopwatch.GetTimestamp();
        var duration = (end - start) / (Stopwatch.Frequency / 1000.0);

        Console.WriteLine($"Render time: {duration:0.000}ms\nSize: {width},{height}\nRays per Pixel: {renderer.RaysPerPixel}\nRay Bounce Depth: {renderer.MaxDepth}\nCores: {Environment.ProcessorCount}");

        return;
    }

    while (running)
    {
        renderer.Render();
        display.SetPixels(renderer.Data);
    }
});


dispThread.Start();

display.Run();

running = false;
