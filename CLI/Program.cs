using System.Diagnostics;
using UncleRay;

var img = "test.bmp";
var engine = new Engine(1240, 720);
if (engine.TryDebugScene(img))
{
    Process.Start(
        new ProcessStartInfo(img)
        {
            UseShellExecute = true
        });
}
