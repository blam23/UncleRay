using System.Diagnostics;
using UncleRay;
using UncleRay.SDL;

//var (width, height) = (620, 360);
var (width, height) = (1240, 720);
//var (width, height) = (1920, 1080);

var renderer = new Engine(width, height);
using var display = new Main(width, height, 1);
var running = true;
var oneshot = true;

var dispThread = new Thread(() =>
{
    if (oneshot)
    {
        renderer.RaysPerPixel = 4000;
        renderer.MaxDepth = 100;

        var start = Stopwatch.GetTimestamp();
        
        renderer.RenderWithPartials(display.SetPixels);
        
        var end = Stopwatch.GetTimestamp();
        var duration = (end - start) / (Stopwatch.Frequency / 1000.0);

        Console.WriteLine($"Render time: {duration:0.000}ms");
        Console.WriteLine($"Size: {width},{height}");
        Console.WriteLine($"Rays per Pixel: {renderer.RaysPerPixel}");
        Console.WriteLine($"Ray Bounce Depth: {renderer.MaxDepth}");

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
