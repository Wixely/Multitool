# Multitool agent instructions

Read `README.md`, `docs/ARCHITECTURE.md`, and `docs/MANIFEST.md` before changing the project.

Use C# and .NET 10. Keep the portable application and update-planning code free of Win32 types. Multitool must never execute release-supplied scripts before the package passes its publisher and digest checks. Preserve the staged transaction, config backup, health check, and rollback boundaries when implementation begins.

CupriFace ignores unsupported markup and CSS at runtime. Run the doctor and inspect a rendered snapshot after interface changes:

```powershell
git submodule update --init --recursive
dotnet build Multitool.slnx -c Release
dotnet test tests/Multitool.Tests/Multitool.Tests.csproj -c Release
dotnet run --project src/Multitool.Desktop -c Release -- --doctor
dotnet run --project src/Multitool.Desktop -c Release -- --snapshot artifacts/multitool.png
```

Do not add a second installed executable merely to update Multitool. A staged copy of the same `Multitool.exe` may run an internal updater role after the main process exits.
