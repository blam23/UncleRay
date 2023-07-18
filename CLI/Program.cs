using System.Diagnostics;
using UncleRay;

var engine = new Engine(1240, 720);
if (engine.TryDebugScene(out var img))
{
    Process.Start(
        new ProcessStartInfo(img)
        {
            UseShellExecute = true
        });
}
