# Managed application manifest

Each managed application needs a catalog entry controlled by Multitool and an embedded `multitool.json` in every release artifact. The catalog says where a release may come from and which publisher is trusted. The embedded manifest says what the verified package contains and how that version behaves.

## Proposed fields

```json
{
  "schema": 1,
  "id": "wixely.example",
  "name": "Example",
  "version": "1.4.0",
  "architectures": ["win-x64"],
  "entryPoint": "Example.exe",
  "processNames": ["Example"],
  "config": {
    "schema": 3,
    "locations": ["local-app-data:Wixely/Example"],
    "migration": "application-maintenance-v1"
  },
  "health": {
    "mode": "application-maintenance-v1",
    "timeoutSeconds": 20
  },
  "shortcuts": [
    { "location": "start-menu", "name": "Example" }
  ]
}
```

Paths use named roots and forward slashes. Absolute paths, parent traversal, network shares, shell commands, environment expansion, registry writes, services, drivers, and arbitrary scripts are rejected by schema 1.

## Provider catalog

The provider catalog adds the source project, release channel, artifact naming rule, expected signing identity, digest requirement, and whether older portable installs may be adopted. Private provider credentials are local state and never appear in either manifest.

The first adapters are candidates rather than promises:

| Application | Current source | Adoption work |
| --- | --- | --- |
| Shade | GitHub | Confirm release archive, executable identity, and config roots |
| Bantz | GitHub | Define model/config migration and running-process behavior |
| CursorGoblin | Existing private repository | Confirm its authoritative release host and package format |
| Kino | GitHub | Define media/config cache ownership and migration boundary |
| portsnuffer | GitHub | Confirm whether it is installed, portable, or command-line only |
| MCPHub | GitHub | Separate app binaries from encrypted server settings and preserve credential storage |

Each application joins the default catalog only after package inspection, clean install, update, failed-update rollback, config migration, uninstall-with-config-retention, and privacy review pass on Windows.
