using System.Text.Json;
namespace Shuabao.Core;
public static class Recruitment
{
    public static IReadOnlyList<DrawResult> Draw(Catalog c, GameState s, int region, bool single)
    {
        Rules.Require(region >= 0 && region < 4 && Rules.MaxClear(s) >= region * 30, "区域未解锁"); Rules.Require(s.Pending == null, "请先完成待选结果"); int count = single ? 1 : 10 - s.Draws[region] % 10; Rules.Require(s.Resources["ticket"] >= count, $"需要 {count} 张招募券"); s.Resources["ticket"] -= count; var eligible = c.Heroes.Where(h => h.Region == region && !h.Story).ToList(); var results = new List<DrawResult>();
        for (int i = 0; i < count; i++)
        {
            int index = s.Draws[region]++; var r = new RandomStream(s.Seed, $"recruit/{region}/{index}"); if (index % 10 == 9)
            {
                var missing = eligible.Where(h => !s.Heroes.ContainsKey(h.Id)).ToList(); List<OfferOption> options;
                if (missing.Count > 0) { var ranks = new Dictionary<string, int> { { "N", 0 }, { "R", 1 }, { "SR", 2 }, { "SSR", 3 } }; var first = missing.OrderByDescending(h => h.Id == s.Wishlist[region]).ThenByDescending(h => ranks[h.Rarity]).ThenBy(h => h.Id, StringComparer.Ordinal).First(); var others = eligible.Where(h => h.Id != first.Id).OrderBy(h => s.Heroes.ContainsKey(h.Id)).ThenBy(h => h.Id, StringComparer.Ordinal).Take(2); options = new[] { first }.Concat(others).Select(h => new OfferOption { Id = h.Id }).ToList(); }
                else options = eligible.OrderByDescending(h => h.Id == s.Wishlist[region]).ThenBy(h => s.Heroes[h.Id].Rank).ThenBy(h => h.Id, StringComparer.Ordinal).Take(3).Select(h => new OfferOption { Id = h.Id, Type = "fragment", Amount = 30 }).ToList();
                s.Pending = new() { Id = Rules.Id(s, "offer"), Kind = "recruit", Region = region, Options = options, Results = results };
            }
            else { var tier = r.Weighted(new (string, int)[] { ("N", 15), ("R", 45), ("SR", 35), ("SSR", 5) }.Where(t => eligible.Any(h => h.Rarity == t.Item1)).ToArray()); var h = r.Pick(eligible.Where(h => h.Rarity == tier).ToArray()); bool whole = r.Next() < .6, isNew = whole && !s.Heroes.ContainsKey(h.Id); if (whole) Model.GrantHero(c, s, h.Id); else Model.Fragments(c, s, h.Id, 20); results.Add(new() { Id = h.Id, Type = whole ? "hero" : "fragment", IsNew = isNew, Amount = isNew ? 0 : 20 }); }
        }
        return results;
    }
    public static ChoiceResult Choose(Catalog c, GameState s, string offer, int index)
    {
        if (s.Receipts.TryGetValue(offer, out var receipt)) return receipt.Deserialize<ChoiceResult>(Json.Options)!; Rules.Require(s.Pending?.Id == offer, "待选记录不存在"); var o = s.Pending!; Rules.Require(index >= 0 && index < o.Options.Count, "候选无效"); var option = o.Options[index]; ChoiceResult result;
        if (o.Kind == "loot") { Economy.StoreItem(s, option.Item!); result = new() { Item = option.Item!.Id, Name = option.Item.Name }; }
        else { bool isNew = option.Type == "hero" && !s.Heroes.ContainsKey(option.Id); if (option.Type == "hero") Model.GrantHero(c, s, option.Id); else Model.Fragments(c, s, option.Id, option.Amount); result = new() { Id = option.Id, IsNew = isNew, Amount = isNew ? 0 : option.Amount }; }
        s.Receipts[offer] = JsonSerializer.SerializeToElement(result, Json.Options); s.Pending = null; return result;
    }
}
