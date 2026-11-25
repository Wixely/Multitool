# Multitool

![Multitool banner](docs/assets/multitool-banner.png)

Multitool is the Windows home for Wixely desktop applications. It will discover supported apps, install them, compare installed and available versions, apply updates, and carry configuration forward without making every application invent its own updater.

This foundation contains the CupriFace desktop shell and the update planner. The shell uses a synthetic inventory so its states can be reviewed without touching installed software. Install and update buttons are intentionally locked until an application has a reviewed release manifest, publisher policy, migration adapter, and recovery test.

## Repository shape

| Path | Purpose |
| --- | --- |
| `src/Multitool.Core` | Application manifests, installed-state models, and deterministic install/update planning |
| `src/Multitool.App` | Portable CupriFace interface and view model |
| `src/Multitool.Desktop` | Windows host, diagnostics, snapshots, and later operating-system integration |
| `tests/Multitool.Tests` | Planner safety and ordering tests |
| `docs/ARCHITECTURE.md` | Trust, transaction, migration, recovery, and provider design |
| `docs/MANIFEST.md` | Proposed application contract and adoption path |
| `external/CupriFace` | Pinned CupriFace source dependency |

## Build and inspect

Use Windows PowerShell 5.1 and .NET SDK 10.0.404 or a later .NET 10 patch.

```powershell
git submodule update --init --recursive
dotnet build Multitool.slnx -c Release
dotnet test tests/Multitool.Tests/Multitool.Tests.csproj -c Release
dotnet run --project src/Multitool.Desktop -c Release -- --doctor
```

Open the desktop shell:

```powershell
dotnet run --project src/Multitool.Desktop -c Debug
```

Render a review image without opening a window:

```powershell
dotnet run --project src/Multitool.Desktop -c Release -- --snapshot artifacts/multitool.png
```

VS Code includes build, test, doctor, snapshot, and desktop debug entries.

## Current boundary

- Windows per-user applications and installs are the first target.
- Shade, Bantz, CursorGoblin, Kino, portsnuffer, and MCPHub are the first adoption candidates.
- GitHub Releases and private Graft releases are planned providers. A provider only supplies metadata and bytes; the core enforces the same trust and transaction policy for both.
- Multitool will not scrape arbitrary repositories, run remote scripts, guess config locations, or silently elevate.
- Linux support, machine-wide installs, background scheduling, and an optional command-line interface come after the Windows interactive flow is proven.
- No source licence has been selected. The repository is private on Graft.

See [Architecture](docs/ARCHITECTURE.md) for the implementation sequence and [Manifest](docs/MANIFEST.md) for the contract each managed application needs.
