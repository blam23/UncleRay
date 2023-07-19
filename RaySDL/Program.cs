using System.Diagnostics;
using UncleRay;
using UncleRay.SDL;

var (width, height) = (620, 360);

var renderer = new Engine(width, height);
var display = new Main(width, height, 2);
var running = true;
var oneshot = true;

var dispThread = new Thread(() =>
{
    while (running)
    {
        renderer.Render();
        display.SetPixels(renderer.Data);
    }
});

if (oneshot)
{
    var start = Stopwatch.GetTimestamp();
    renderer.Render();
    display.SetPixels(renderer.Data);

    var end = Stopwatch.GetTimestamp();
    var duration = new TimeSpan(end - start);
    Console.WriteLine($"Render time: {duration.Milliseconds:0.000}ms");
}
else
{
    dispThread.Start();
}

display.Run();

running = false;

display.Dispose();
