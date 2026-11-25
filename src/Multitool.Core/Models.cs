namespace Multitool.Core;

public sealed record AppManifest(
    string Id,
    string Name,
    Version Version,
    int ConfigSchema,
    string ArtifactSha256,
    long ArtifactLength,
    bool SupportsHealthCheck = true);

public sealed record InstalledApp(
    string Id,
    Version Version,
    int ConfigSchema,
    bool IsRunning = false);

public enum OperationKind
{
    Install,
    Update,
    Repair
}

public enum OperationStepKind
{
    Download,
    VerifyPackage,
    StopApplication,
    BackUpConfiguration,
    ExtractToStaging,
    MigrateConfiguration,
    ValidateStaging,
    SwitchActiveVersion,
    StartApplication,
    RunHealthCheck,
    Commit,
    RetainRollback
}

public sealed record OperationStep(OperationStepKind Kind, bool IsRollbackBoundary = false);

public sealed record OperationPlan(
    OperationKind Kind,
    string AppId,
    Version TargetVersion,
    IReadOnlyList<OperationStep> Steps);
