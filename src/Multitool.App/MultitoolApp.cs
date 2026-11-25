using CupriFace;
using SkiaSharp;

namespace Multitool.App;

public sealed class MultitoolApp : CupriApp
{
    private readonly MultitoolModel model = new();

    public override string Title => "Multitool";
    public override int Width => 1120;
    public override int Height => 760;
    public override SKColor Background => new(0x0d, 0x11, 0x17);
    public override bool DarkWindowChrome => true;
    public override object Model => model;

    public override void Configure(CupriDocument document)
    {
        document.OnClick(".refresh", _ =>
        {
            model.Status = "Catalog checked. This foundation uses review data and has not contacted a release provider.";
            document.Refresh();
        });
        document.OnClick(".manage", info =>
        {
            var id = info.Element.GetAttribute("data-id");
            model.Status = $"{model.NameFor(id)} is locked until its package and migration contract pass adoption tests.";
            document.Refresh();
        });
    }

    public override string Html => """
        <body>
          <main class="shell">
            <header class="topbar">
              <div class="brand">
                <div class="mark">M</div>
                <div><h1>Multitool</h1><p>Wixely application manager</p></div>
              </div>
              <button class="refresh">Check for updates</button>
            </header>

            <section class="summary">
              <div><span>Managed apps</span><strong>{{ManagedCount}}</strong></div>
              <div><span>Updates ready</span><strong>{{UpdateCount}}</strong></div>
              <div><span>Attention</span><strong>{{AttentionCount}}</strong></div>
            </section>

            <section class="content">
              <aside class="rail">
                <button class="selected">Library</button>
                <button>Updates</button>
                <button>History</button>
                <button>Settings</button>
              </aside>

              <section class="library">
                <div class="section-title"><div><h2>Your tools</h2><p>Install, update, and carry settings forward from one place.</p></div><span class="channel">Stable channel</span></div>
                <div class="app-list">
                  <article class="app-card" data-repeat="Apps">
                    <div class="app-icon">{{Initial}}</div>
                    <div class="app-copy"><h3>{{Name}}</h3><p>{{Summary}}</p><span>{{VersionLine}}</span></div>
                    <span class="state {{StateClass}}">{{State}}</span>
                    <button class="manage" data-id="{{Id}}">{{Action}}</button>
                  </article>
                </div>
                <p class="status">{{Status}}</p>
              </section>
            </section>
          </main>
        </body>
        """;

    public override string Css => """
        :root { --bg:#0d1117; --panel:#151b24; --raised:#1c2430; --line:#303b49; --text:#f3f6fa; --muted:#9ba9b8; --accent:#7ee4c4; --blue:#8bc8ff; --warn:#ffc978; }
        * { box-sizing:border-box; }
        body { margin:0; background:var(--bg); color:var(--text); font-family:"Segoe UI",sans-serif; }
        button { font-family:inherit; font-size:14px; }
        .shell { min-height:100vh; padding:28px; flex-direction:column; gap:22px; background-image:radial-gradient(circle at 15% -20%,#27475a 0%,#111821 36%,#0d1117 68%); }
        .topbar { display:flex; align-items:center; justify-content:space-between; gap:20px; }
        .brand { display:flex; align-items:center; gap:14px; }
        .mark { width:48px; height:48px; border-radius:14px; display:flex; align-items:center; justify-content:center; background-image:linear-gradient(135deg,#9df3d9 0%,#4f96bf 100%); color:#081019; font-size:24px; font-weight:bold; box-shadow:0 10px 30px #00000066; }
        h1,h2,h3,p { margin:0; }
        h1 { font-size:24px; letter-spacing:0.02em; }
        .brand p,.section-title p,.app-copy p,.app-copy span,.status { color:var(--muted); }
        .refresh,.manage { min-height:42px; padding:0 16px; border:1px solid #5e8da3; border-radius:11px; background:#173247; color:#dff5ff; display:flex; align-items:center; justify-content:center; }
        .summary { display:grid; grid-template-columns:repeat(3,1fr); gap:14px; }
        .summary div { padding:18px; border:1px solid var(--line); border-radius:16px; background:var(--panel); display:flex; align-items:center; justify-content:space-between; }
        .summary span { color:var(--muted); }
        .summary strong { font-size:26px; color:var(--accent); }
        .content { min-height:0; display:flex; align-items:stretch; flex:1; gap:18px; }
        .rail { width:170px; padding:10px; border:1px solid var(--line); border-radius:16px; background:var(--panel); flex-direction:column; gap:6px; }
        .rail button { min-height:44px; padding:0 14px; border:0; border-radius:10px; background:transparent; color:var(--muted); text-align:left; }
        .rail .selected { background:var(--raised); color:var(--text); }
        .library { min-width:0; padding:20px; border:1px solid var(--line); border-radius:16px; background:var(--panel); flex:1; flex-direction:column; gap:16px; }
        .section-title { display:flex; align-items:center; justify-content:space-between; gap:20px; }
        .section-title h2 { margin-bottom:4px; }
        .channel { padding:7px 10px; border:1px solid var(--line); border-radius:999px; color:var(--muted); font-size:12px; }
        .app-list { min-height:0; overflow:auto; flex-direction:column; gap:9px; }
        .app-card { min-height:84px; margin-bottom:9px; padding:12px; border:1px solid #293442; border-radius:14px; background:var(--raised); display:flex; align-items:center; gap:14px; }
        .app-icon { width:48px; height:48px; flex-shrink:0; border-radius:12px; background:#26394a; color:var(--blue); display:flex; align-items:center; justify-content:center; font-size:21px; font-weight:bold; }
        .app-copy { min-width:0; flex:1; flex-direction:column; gap:3px; }
        .app-copy h3 { font-size:16px; }
        .app-copy p,.app-copy span { font-size:12px; }
        .state { min-width:92px; color:var(--muted); font-size:12px; text-align:right; }
        .state.ready { color:var(--accent); }
        .state.update { color:var(--warn); }
        .manage { min-width:92px; }
        .status { min-height:20px; font-size:12px; }
        @media (max-width:760px) { .shell{padding:18px}.summary{grid-template-columns:1fr}.content{flex-direction:column}.rail{width:100%;flex-direction:row}.rail button{flex:1}.app-card{flex-wrap:wrap}.app-copy{min-width:220px}.state{text-align:left} }
        """;
}

public sealed class MultitoolModel
{
    public List<AppRow> Apps { get; } =
    [
        new("shade", "Shade", "S", "Desktop shading and display control", "Installed 0.9.2 · latest 0.9.2", "Ready", "ready", "Open"),
        new("bantz", "Bantz", "B", "Local dictation and speech tools", "Installed 0.6.0 · latest 0.6.1", "Update ready", "update", "Review"),
        new("cursorgoblin", "CursorGoblin", "C", "Cursor utility", "Not managed yet", "Adoption needed", "", "Inspect"),
        new("kino", "Kino", "K", "Desktop capture and video workspace", "Not installed · latest 0.1.0", "Available", "ready", "Install"),
        new("portsnuffer", "portsnuffer", "P", "Local port inspection utility", "Installed 1.2.0 · latest 1.2.0", "Ready", "ready", "Open"),
        new("mcphub", "MCPHub", "M", "Local MCP service manager", "Installed 0.8.0 · latest 0.8.1", "Migration review", "update", "Review")
    ];

    public string ManagedCount => "4";
    public string UpdateCount => "2";
    public string AttentionCount => "2";
    public string Status { get; set; } = "No software changes have been made.";

    public string NameFor(string? id) => Apps.FirstOrDefault(app => app.Id == id)?.Name ?? "This application";
}

public sealed record AppRow(string Id, string Name, string Initial, string Summary, string VersionLine, string State, string StateClass, string Action);
