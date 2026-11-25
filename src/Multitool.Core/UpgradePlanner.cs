namespace Multitool.Core;

public sealed class UpgradePlanner
{
    public OperationPlan Plan(AppManifest target, InstalledApp? installed)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(target.Id);
        ArgumentException.ThrowIfNullOrWhiteSpace(target.Name);
        ValidateArtifact(target);

        if (installed is not null && !string.Equals(installed.Id, target.Id, StringComparison.Ordinal))
            throw new InvalidOperationException("The installed application does not match the requested release.");

        if (installed is not null && target.Version < installed.Version)
            throw new InvalidOperationException("Downgrades require an explicit, application-specific recovery plan.");

        if (installed is not null && target.ConfigSchema < installed.ConfigSchema)
            throw new InvalidOperationException("The target cannot read the installed configuration schema.");

        var kind = installed is null
            ? OperationKind.Install
            : target.Version == installed.Version
                ? OperationKind.Repair
                : OperationKind.Update;

        var steps = new List<OperationStep>
        {
            new(OperationStepKind.Download),
            new(OperationStepKind.VerifyPackage),
        };

        if (installed?.IsRunning is true)
            steps.Add(new(OperationStepKind.StopApplication));

        if (installed is not null)
            steps.Add(new(OperationStepKind.BackUpConfiguration));

        steps.Add(new(OperationStepKind.ExtractToStaging));

        if (installed is not null && target.ConfigSchema > installed.ConfigSchema)
            steps.Add(new(OperationStepKind.MigrateConfiguration));

        steps.Add(new(OperationStepKind.ValidateStaging));
        steps.Add(new(OperationStepKind.SwitchActiveVersion, IsRollbackBoundary: true));

        if (installed?.IsRunning is true)
            steps.Add(new(OperationStepKind.StartApplication));

        if (target.SupportsHealthCheck)
            steps.Add(new(OperationStepKind.RunHealthCheck));

        steps.Add(new(OperationStepKind.Commit));
        steps.Add(new(OperationStepKind.RetainRollback));

        return new OperationPlan(kind, target.Id, target.Version, steps);
    }

    private static void ValidateArtifact(AppManifest target)
    {
        if (target.ArtifactLength <= 0)
            throw new InvalidOperationException("A release must declare a positive artifact length.");

        if (target.ArtifactSha256.Length != 64 || target.ArtifactSha256.Any(character => !Uri.IsHexDigit(character)))
            throw new InvalidOperationException("A release must declare a 64-character SHA-256 digest.");
    }
}
