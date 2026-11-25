using Multitool.Core;
using Xunit;

namespace Multitool.Tests;

public sealed class UpgradePlannerTests
{
    private const string Digest = "0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef";
    private readonly UpgradePlanner planner = new();

    [Fact]
    public void Update_backs_up_and_migrates_before_switching_versions()
    {
        var target = new AppManifest("wixely.bantz", "Bantz", new Version(2, 0), 3, Digest, 1024);
        var installed = new InstalledApp("wixely.bantz", new Version(1, 5), 2, IsRunning: true);

        var plan = planner.Plan(target, installed);
        var steps = plan.Steps.Select(step => step.Kind).ToArray();

        Assert.Equal(OperationKind.Update, plan.Kind);
        Assert.True(Array.IndexOf(steps, OperationStepKind.BackUpConfiguration) < Array.IndexOf(steps, OperationStepKind.MigrateConfiguration));
        Assert.True(Array.IndexOf(steps, OperationStepKind.MigrateConfiguration) < Array.IndexOf(steps, OperationStepKind.SwitchActiveVersion));
        Assert.True(Array.IndexOf(steps, OperationStepKind.SwitchActiveVersion) < Array.IndexOf(steps, OperationStepKind.RunHealthCheck));
    }

    [Fact]
    public void Install_has_no_config_backup_or_migration()
    {
        var target = new AppManifest("wixely.kino", "Kino", new Version(1, 0), 1, Digest, 2048);

        var plan = planner.Plan(target, installed: null);

        Assert.Equal(OperationKind.Install, plan.Kind);
        Assert.DoesNotContain(plan.Steps, step => step.Kind is OperationStepKind.BackUpConfiguration or OperationStepKind.MigrateConfiguration);
    }

    [Fact]
    public void Downgrade_is_refused()
    {
        var target = new AppManifest("wixely.shade", "Shade", new Version(1, 0), 1, Digest, 512);
        var installed = new InstalledApp("wixely.shade", new Version(2, 0), 1);

        var error = Assert.Throws<InvalidOperationException>(() => planner.Plan(target, installed));

        Assert.Contains("Downgrades", error.Message);
    }

    [Fact]
    public void Package_without_sha256_is_refused()
    {
        var target = new AppManifest("wixely.mcphub", "MCPHub", new Version(1, 0), 1, "missing", 512);

        Assert.Throws<InvalidOperationException>(() => planner.Plan(target, installed: null));
    }
}
