using CupriFace;
using CupriFace.Diagnostics;
using CupriFace.Shell;
using Multitool.App;
using SkiaSharp;

var app = new MultitoolApp();

if (args.Contains("--doctor", StringComparer.OrdinalIgnoreCase))
{
    var report = CupriDoctor.Check(app.Html, app.Css, [(1120, 760), (720, 900)], model: app.Model, configure: app.Configure);
    Console.WriteLine(report);
    return report.IsClean ? 0 : 2;
}

var snapshotIndex = Array.FindIndex(args, argument => string.Equals(argument, "--snapshot", StringComparison.OrdinalIgnoreCase));
if (snapshotIndex >= 0)
{
    var output = snapshotIndex + 1 < args.Length ? args[snapshotIndex + 1] : Path.Combine("artifacts", "multitool.png");
    var report = CupriDoctor.Check(app.Html, app.Css, [(1120, 760), (720, 900)], model: app.Model, configure: app.Configure);
    Console.WriteLine(report);
    if (!report.IsClean)
        return 2;

    var fullPath = Path.GetFullPath(output);
    Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
    using var document = app.CreateDocument();
    using (document.RenderToImage(1120, 760)) { }
    using var image = document.RenderToImage(1120, 760);
    using var data = image.Encode(SKEncodedImageFormat.Png, 95);
    using var file = File.Create(fullPath);
    data.SaveTo(file);
    File.WriteAllText(Path.ChangeExtension(fullPath, ".tree.txt"), document.DumpTree());
    Console.WriteLine($"Snapshot: {fullPath}");
    return 0;
}

DesktopHost.Run(app);
return 0;
