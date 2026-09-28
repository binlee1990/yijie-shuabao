using System.Text.Json;
namespace Shuabao.Core;

public sealed class HeroDef
{
    public string Id { get; set; } = ""; public string Name { get; set; } = ""; public int Region { get; set; }
    public string Role { get; set; } = "guard";
    public int[] Attrs { get; set; } = []; public string Rarity { get; set; } = "N"; public bool Story { get; set; }
    public int Unlock { get; set; }
    public string Asset { get; set; } = ""; public string Description { get; set; } = ""; public string Source { get; set; } = "authored_fixture";
}
public sealed class StageDef { public int Id { get; set; } public int Region { get; set; } public bool Boss { get; set; } public string Name { get; set; } = ""; public int Waves { get; set; } = 3; }
public sealed class BuildingDef { public string Id { get; set; } = ""; public string Name { get; set; } = ""; public int Unlock { get; set; } public string Resource { get; set; } = "gold"; public string Desc { get; set; } = ""; public string Asset { get; set; } = ""; }
public sealed record SkillDef(string Name, string Text, int Cost, int Cd);
public sealed record PublicSkill(string Name, string Type, string Text);
public sealed class Catalog
{
    public const string Version = "shuabao-godot-0.2.0";
    public const string WebVersion = "shuabao-web-0.1.0";
    public List<HeroDef> Heroes { get; set; } = []; public List<StageDef> Stages { get; set; } = []; public List<BuildingDef> Buildings { get; set; } = [];
    [System.Text.Json.Serialization.JsonIgnore] public Dictionary<string, HeroDef> ById { get; private set; } = [];
    public static readonly int[] Caps = [20, 40, 60, 80, 100, 120], Costs = [20, 40, 60, 100, 160], Gates = [10, 30, 50, 80, 110];
    public static readonly string[] Ranks = ["潜龙", "见龙", "惕龙", "跃龙", "飞龙", "亢龙"], Regions = ["群雄逐鹿", "山河一统", "盛世长歌", "万象归墟"], Attrs = ["武", "体", "敏", "统", "智", "政", "学", "艺", "魅", "灵"], Slots = ["weapon", "armor", "feet", "accessory"], Sets = ["破军", "长生", "玄策", "流风"];
    public static readonly Dictionary<string, string> Roles = new() { ["guard"] = "守御", ["striker"] = "强攻", ["archer"] = "远袭", ["mage"] = "谋略", ["healer"] = "医者", ["controller"] = "控制" };
    public static readonly Dictionary<string, string> SlotNames = new() { ["weapon"] = "兵器", ["armor"] = "衣甲", ["feet"] = "靴履", ["accessory"] = "佩饰" };
    public static readonly Dictionary<string, SkillDef> Skills = new()
    {
        ["guard"] = new("山岳为屏", "获得护盾并嘲讽敌军，保护后排。", 12, 2),
        ["striker"] = new("破阵长锋", "击破护盾，对前排造成高额伤害。", 14, 2),
        ["archer"] = new("百步穿杨", "优先攻击后排，附加破绽标记。", 12, 2),
        ["mage"] = new("星火连营", "全体法术伤害与灼烧。", 20, 3),
        ["healer"] = new("青囊济世", "治疗生命最低的队友并清除灼烧。", 14, 2),
        ["controller"] = new("奇策断势", "优先打断蓄力目标，眩晕一回合。", 15, 2)
    };
    public static readonly Dictionary<string, PublicSkill> PublicSkills = new() { ["vigor"] = new("固本", "passive", "生命 +12%"), ["focus"] = new("凝神", "passive", "法攻 +15%"), ["riposte"] = new("反击", "reaction", "每轮首次受击反击35%攻击"), ["cleanse"] = new("净心", "active", "主动技清除自身灼烧"), ["charge"] = new("奋勇", "active", "主动技伤害 +10%") };
    public static Catalog Load(string json) { var c = JsonSerializer.Deserialize<Catalog>(json, Json.Options) ?? throw new InvalidDataException("内容包为空"); c.ById = c.Heroes.ToDictionary(x => x.Id); c.Validate(); return c; }
    public void Validate() { Rules.Require(Heroes.Count == 96 && ById.Count == 96, "主线名册必须有96个唯一人物"); Rules.Require(Stages.Count == 120 && Stages.Select(x => x.Id).SequenceEqual(Enumerable.Range(1, 120)), "关卡顺序非法"); Rules.Require(Stages.All(x => x.Waves == 3 && x.Boss == (x.Id % 10 == 0)), "三波/Boss规则不一致"); Rules.Require(Buildings.Count == 12, "建筑表不完整"); foreach (var h in Heroes) Rules.Require(h.Attrs.Length == 10 && Roles.ContainsKey(h.Role) && (!h.Story || h.Unlock < 120), "人物定义非法"); for (int i = 0; i < 4; i++) Rules.Require(Heroes.Count(h => h.Region == i && !h.Story) == 18 && Heroes.Count(h => h.Region == i && h.Story) == 6, "区域池不闭合"); }
}
public static class Json
{
    public static readonly JsonSerializerOptions Options = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, PropertyNameCaseInsensitive = true };
    public static T Copy<T>(T value) => JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(value, Options), Options)!;
}
public static class Rules
{
    public static void Require(bool valid, string message) { if (!valid) throw new InvalidOperationException(message); }
    public static int Round(double n) => (int)Math.Floor(n + .5);
    public static int MaxClear(GameState s) => s.Cleared.Count == 0 ? 0 : s.Cleared.Max();
    public static string Id(GameState s, string prefix) => $"{prefix}_{++s.Serial}";
}
