namespace Shuabao.Core;
public sealed class GameEngine
{
    public Catalog Catalog { get; }
    public GameState State { get; private set; }
    private readonly Action<GameState> _persist;
    public GameEngine(Catalog catalog, GameState state, Action<GameState>? persist = null) { Model.Validate(catalog, state); Catalog = catalog; State = Json.Copy(state); _persist = persist ?? (_ => { }); }
    public object? Execute(Command cmd, long now)
    {
        var s = Json.Copy(State); var c = Catalog; object? result = null; string id = cmd.Id ?? ""; if (cmd.Type is not ("clock" or "choose" or "step" or "settle")) Economy.Advance(c, s, now); if (s.Pending != null && cmd.Type is not ("choose" or "clock")) throw new InvalidOperationException("请先完成已保存的待选奖励");
        switch (cmd.Type)
        {
            case "draw": result = Recruitment.Draw(c, s, cmd.Number, cmd.Single); break;
            case "choose": result = Recruitment.Choose(c, s, id, cmd.Index); break;
            case "wish": Rules.Require(c.ById.TryGetValue(id, out var wished) && !wished.Story && wished.Region == cmd.Number, "许愿目标不在普通池"); s.Wishlist[cmd.Number] = id; break;
            case "formation": Rules.Require(s.Encounter == null && s.Dispatch == null, "请先结束出征再换阵"); Rules.Require(cmd.Number >= 0 && cmd.Number < 6 && (cmd.Id == null || s.Heroes.ContainsKey(id)), "阵位或人物非法"); int old = Array.IndexOf(s.Formation, cmd.Id); if (cmd.Id != null && old >= 0) s.Formation[old] = s.Formation[cmd.Number]; s.Formation[cmd.Number] = cmd.Id; break;
            case "preset-save": Rules.Require(s.Presets.Count < 5, "最多五套预设"); s.Presets.Add(new() { Name = $"阵容 {s.Presets.Count + 1}", Slots = [.. s.Formation] }); break;
            case "preset-load": Rules.Require(s.Encounter == null && s.Dispatch == null && cmd.Index >= 0 && cmd.Index < s.Presets.Count, "当前不能载入预设"); s.Formation = [.. s.Presets[cmd.Index].Slots]; break;
            case "train": result = Economy.Train(s, id, Math.Clamp(cmd.Number, 1, 120)); break;
            case "promote": result = Economy.Promote(c, s, id); break;
            case "story": result = Economy.Story(c, s, id); break;
            case "trial": result = Economy.Trial(c, s, id); break;
            case "battle": result = Battle.Start(c, s, cmd.Number); break;
            case "step": result = Battle.Step(c, s, cmd.Action, cmd.Other); break;
            case "settle": result = Economy.Settle(c, s); break;
            case "retreat": Rules.Require(s.Encounter != null, "没有战斗"); s.Encounter = null; break;
            case "equip": Economy.Equip(s, id, cmd.Other!); break;
            case "unequip": Rules.Require(s.Heroes.ContainsKey(id) && !Model.Busy(s, id), "当前不能卸装"); s.Heroes[id].Equipment.Remove(cmd.Other!); break;
            case "enhance": Economy.Enhance(s, id); break;
            case "lock": var item = s.Items.Find(x => x.Id == id); Rules.Require(item != null, "装备不存在"); item!.Locked = !item.Locked; break;
            case "dismantle": Economy.Dismantle(s, id); break;
            case "craft": result = Economy.Craft(s, id, cmd.Other!); break;
            case "inbox": while (s.Items.Count < 500 && s.Inbox.Count > 0) { s.Items.Add(s.Inbox[0]); s.Inbox.RemoveAt(0); } break;
            case "building": Economy.Upgrade(s, id); break;
            case "worker": Economy.Worker(s, id, cmd.Other); break;
            case "learn": Economy.Learn(s, id, cmd.Other!); break;
            case "dispatch": Economy.StartDispatch(s, cmd.Number); break;
            case "dispatch-stop": s.Dispatch = null; break;
            case "dispatch-resume": Rules.Require(s.Dispatch != null && s.Encounter == null, "当前不能恢复委托"); foreach (var k in s.Workers.Where(x => s.Dispatch!.Team.Contains(x.Value)).Select(x => x.Key).ToArray()) s.Workers.Remove(k); s.Dispatch!.Paused = false; break;
            case "clock": result = Economy.Advance(c, s, now); break;
            default: throw new InvalidOperationException($"未知命令 {cmd.Type}");
        }
        s.Ending = s.Mainline.All(s.Heroes.ContainsKey) && s.Cleared.Contains(120); s.Revision++; if (cmd.Type is not ("step" or "clock")) { s.Journal.Add(new() { Revision = s.Revision, Type = cmd.Type, At = now }); if (s.Journal.Count > 150) s.Journal.RemoveAt(0); }
        Model.Validate(c, s); _persist(s); State = s; return result;
    }
    public void Import(GameState state) { Model.Validate(Catalog, state); var candidate = Json.Copy(state); _persist(candidate); State = candidate; }
}
