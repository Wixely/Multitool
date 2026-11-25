# Architecture

## Product boundary

Multitool manages applications that explicitly opt into its contract. It is an application manager, not a general package manager: it does not search the machine for unrelated software or infer how an unknown installer works.

The first release is interactive and per-user. Applications live under `%LOCALAPPDATA%\Programs\Wixely`, while Multitool keeps its registry, operation journal, package cache, and redacted diagnostics under `%LOCALAPPDATA%\Wixely\Multitool`. Existing portable installs may be adopted only after the user confirms the detected location and Multitool validates the application identity.

## Components

```text
CupriFace desktop shell
        |
application service
        |
catalog + inventory + policy
        |
deterministic operation planner
        |
transaction executor and journal
   |          |           |
providers   app adapters  Windows integration
GitHub      discovery     process/session checks
Graft       health        atomic directory swaps
local       migration     shortcuts and uninstall
```

`Multitool.Core` owns policy and produces an operation plan with no filesystem or network side effects. The desktop edge gathers inventory, downloads bytes through a provider, and executes the approved plan. This split keeps the dangerous part small and makes every transition testable before it changes a machine.

## Package trust

An application entry pins its publisher, release provider, artifact pattern, architecture, and digest policy. A candidate release is usable only when:

1. the provider response belongs to the configured project;
2. the release channel is allowed;
3. the artifact matches the expected application and Windows architecture;
4. its declared length and SHA-256 match the downloaded bytes;
5. its Authenticode signer matches the application's policy when signing is required; and
6. the embedded manifest agrees with the catalog identity and version.

Provider credentials stay in Windows-protected storage and are never passed to managed applications, command lines, logs, or diagnostic bundles. A GitHub or Graft outage must leave the installed application usable.

## Install and update transaction

Every operation is journalled and resumable:

1. resolve a release and produce a reviewable plan;
2. download into a new staging directory;
3. verify length, digest, publisher, manifest, and archive paths;
4. ask the application to exit, then stop only its registered processes;
5. copy config into a versioned backup outside the install directory;
6. extract into a fresh version directory with traversal and link checks;
7. run any migration against a copied config in staging;
8. validate the staged files and config;
9. atomically switch the active-version pointer;
10. start the application when policy requests it and run its bounded health check;
11. commit the new registry state and retain the previous version for rollback.

Failure before the switch removes staging. Failure after the switch restores the previous version and config backup. Recovery on the next Multitool launch reads the journal and finishes rollback before another operation begins. Cache cleanup never removes the active version, the rollback version, or an operation referenced by an incomplete journal.

Multitool updates itself with the same transaction. A staged copy of `Multitool.exe` runs an internal apply role after the normal process exits, replaces the installed version, verifies startup, and either commits or rolls back. No permanently installed helper is needed.

## Configuration migration

Config remains owned by each application. The preferred contract is a bounded maintenance command in the already-verified application package:

```text
App.exe maintenance migrate-config --source <copy> --target <staging> --from-schema N --to-schema N
App.exe maintenance validate-config --path <staging> --schema N
App.exe maintenance health-check --install <staging>
```

Multitool supplies paths and schema numbers through an internal invocation boundary, applies time and output limits, runs without elevation, and never gives the migration access to provider credentials. The source config is always a copy. A legacy adapter may implement the same contract in Multitool code for an older app, but the adapter must be versioned, tested with fixtures, and removed once the app adopts the common protocol.

Schema changes must be monotonic. Skipping schema versions is allowed only when the app advertises a direct migration path. Downgrades are blocked by default because an older executable may not understand newer config.

## Concurrency and user control

Only one mutation runs at a time. Catalog refresh and downloads may overlap, but install-directory and config changes hold a machine-local operation lock. The interface shows the exact applications that will stop, whether elevation is needed, config paths in neutral form, expected download size, restart behavior, and available rollback before confirmation.

Multitool never restarts the computer, closes an app with unsaved work, changes release channel, or elevates without a separate user decision. Automatic background installation is outside the first release. Scheduled checks may later notify without applying changes.

## Diagnostics and privacy

Logs use application IDs, step names, durations, result codes, versions, and redacted provider errors. They exclude access tokens, user names, machine names, full local paths, config values, command output that may contain secrets, and the contents of managed files. Exported diagnostics receive a second redaction pass.

## Release shape

The Windows package should be self-contained and single-file. NativeAOT is the preferred target and must be tested against CupriFace, JSON source generation, Windows signature inspection, archive handling, and the self-update role. If a measured blocker remains, the first release may use a trimmed or framework-compiled self-contained executable while retaining the one-file boundary; the exception must be recorded before release.
