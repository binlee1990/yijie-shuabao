using System.Text.Json;
namespace Shuabao.Core;
public sealed class HeroState { public string Id { get; set; } = ""; public int Level { get; set; } = 1; public int Rank { get; set; } public int Xp { get; set; } public Dictionary<string, string> Equipment { get; set; } = []; public List<string> Skills { get; set; } = []; public int Mastery { get; set; } }
public sealed class Affix { public string Key { get; set; } = "atk"; public int Value { get; set; } }
public sealed class Item { public string Id { get; set; } = ""; public string Slot { get; set; } = "weapon"; public string Set { get; set; } = "破军"; public int Quality { get; set; } = 1; public int Level { get; set; } = 1; public int Power { get; set; } public int Enhance { get; set; } public bool Locked { get; set; } public Affix Affix { get; set; } = new(); public string Name { get; set; } = ""; }
public sealed class OfferOption { public string Id { get; set; } = ""; public string Type { get; set; } = "hero"; public int Amount { get; set; } = 20; public Item? Item { get; set; } }
public sealed class DrawResult { public string Id { get; set; } = ""; public string Type { get; set; } = "hero"; public bool IsNew { get; set; } public int Amount { get; set; } }
public sealed class Offer { public string Id { get; set; } = ""; public string Kind { get; set; } = "recruit"; public int Region { get; set; } public int Stage { get; set; } public List<OfferOption> Options { get; set; } = []; public List<DrawResult> Results { get; set; } = []; }
public sealed class Preset { public string Name { get; set; } = ""; public string?[] Slots { get; set; } = new string?[6]; }
public sealed class Job { public long Elapsed { get; set; } public long Index { get; set; } }
public sealed class DispatchState { public string Id { get; set; } = ""; public int Stage { get; set; } public List<string> Team { get; set; } = []; public long CycleMs { get; set; } public long Elapsed { get; set; } public long Index { get; set; } public bool Paused { get; set; } }
public sealed class JournalEntry { public long Revision { get; set; } public string Type { get; set; } = ""; public long At { get; set; } }
public sealed class GameState
{
    public int Schema { get; set; } = 2; public string Rules { get; set; } = Catalog.Version; public long Revision { get; set; }
    public long Seed { get; set; }
    public long Serial { get; set; }
    public long Created { get; set; }
    public long LastClock { get; set; }
    public Dictionary<string, long> Resources { get; set; } = new() { ["ticket"] = 30, ["gold"] = 2400, ["ore"] = 120, ["herb"] = 60, ["xp"] = 2500, ["echo"] = 0 };
    public Dictionary<string, HeroState> Heroes { get; set; } = []; public Dictionary<string, int> Fragments { get; set; } = []; public string?[] Formation { get; set; } = new string?[6]; public List<Preset> Presets { get; set; } = [];
    public List<int> Cleared { get; set; } = []; public Dictionary<int, int> ClearCounts { get; set; } = []; public List<string> Story { get; set; } = []; public Dictionary<string, int> Trials { get; set; } = []; public List<string> Mainline { get; set; } = [];
    public int[] Draws { get; set; } = new int[4]; public string?[] Wishlist { get; set; } = new string?[4]; public Offer? Pending { get; set; }
    public List<Item> Items { get; set; } = []; public List<Item> Inbox { get; set; } = []; public Dictionary<string, int> Buildings { get; set; } = new() { ["summon"] = 1, ["hall"] = 1 }; public Dictionary<string, string> Workers { get; set; } = []; public Dictionary<string, Job> Jobs { get; set; } = [];
    public DispatchState? Dispatch { get; set; }
    public BattleState? Encounter { get; set; }
    public Dictionary<string, JsonElement> Receipts { get; set; } = []; public List<JournalEntry> Journal { get; set; } = []; public bool Ending { get; set; }
}
public sealed class Stats
{
    public int Hp { get; set; }
    public int Atk { get; set; }
    public int Magic { get; set; }
    public int Def { get; set; }
    public int Mdef { get; set; }
    public int Speed { get; set; }
    public int Mp { get; set; }
    public int Stamina { get; set; }
    public int Crit { get; set; } = 8;
    public void Add(string key, int n) { switch (key) { case "hp": Hp += n; break; case "atk": Atk += n; break; case "magic": Magic += n; break; case "def": Def += n; break; case "mdef": Mdef += n; break; case "speed": Speed += n; break; } }
}
public sealed class Unit { public string Id { get; set; } = ""; public string? Source { get; set; } public string Name { get; set; } = ""; public string Role { get; set; } = "guard"; public string Side { get; set; } = "ally"; public int Slot { get; set; } public Stats Stats { get; set; } = new(); public int Hp { get; set; } public int Mp { get; set; } public int Sp { get; set; } public int Shield { get; set; } public int Cd { get; set; } public int Stun { get; set; } public int Burn { get; set; } public int Mark { get; set; } public int Taunt { get; set; } public List<string> Skills { get; set; } = []; public int Mastery { get; set; } public int Rank { get; set; } public bool Boss { get; set; } public bool Charge { get; set; } public bool Reacted { get; set; } }
public sealed class CombatEvent { public string Actor { get; set; } = ""; public string Target { get; set; } = ""; public int Amount { get; set; } public bool Crit { get; set; } public string Kind { get; set; } = "damage"; }
public sealed class BattleState { public string Id { get; set; } = ""; public int Stage { get; set; } public List<string> Team { get; set; } = []; public int Wave { get; set; } = 1; public int Round { get; set; } public int Step { get; set; } public List<Unit> Units { get; set; } = []; public List<string> Queue { get; set; } = []; public string Status { get; set; } = "active"; public List<string> Log { get; set; } = []; public CombatEvent? LastEvent { get; set; } public Dictionary<string, int> Roles { get; set; } = []; }
public sealed class Reward { public int Stage { get; set; } public int Tickets { get; set; } public int Gold { get; set; } public int Ore { get; set; } public int Xp { get; set; } public List<string> Items { get; set; } = []; public bool First { get; set; } public bool BossBonus { get; set; } public bool Defeat { get; set; } }
public sealed class ChoiceResult { public string Id { get; set; } = ""; public bool IsNew { get; set; } public int Amount { get; set; } public string? Item { get; set; } public string? Name { get; set; } }
public sealed class ClockResult { public long Elapsed { get; set; } public long Discarded { get; set; } public int Cycles { get; set; } public Dictionary<string, long> Outputs { get; set; } = []; }
public sealed record Command(string Type, string? Id = null, int Number = 0, int Index = 0, string? Other = null, bool Single = false, string? Action = null);
