using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
namespace Shuabao.Core;
public static class SaveCodec
{
    private sealed class Envelope { public string Format { get; set; } = "shuabao-save/2"; public string Checksum { get; set; } = ""; public string Payload { get; set; } = ""; }
    public static string Encode(GameState state) { string payload = JsonSerializer.Serialize(state, Json.Options); return JsonSerializer.Serialize(new Envelope { Payload = payload, Checksum = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(payload))) }, Json.Options); }
    public static GameState Decode(Catalog catalog, string text) { var box = JsonSerializer.Deserialize<Envelope>(text, Json.Options) ?? throw new InvalidDataException("存档为空"); if (box.Format == "shuabao-save/1") { Rules.Require(RandomStream.Hash(box.Payload).ToString("x") == box.Checksum, "浏览器存档校验失败"); return ImportWeb(catalog, box.Payload); } Rules.Require(box.Format == "shuabao-save/2" && Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(box.Payload))).Equals(box.Checksum, StringComparison.OrdinalIgnoreCase), "存档完整性校验失败"); var s = JsonSerializer.Deserialize<GameState>(box.Payload, Json.Options) ?? throw new InvalidDataException("存档主体为空"); Model.Validate(catalog, s); return s; }
    private static GameState ImportWeb(Catalog c, string payload)
    {
        var node = JsonNode.Parse(payload)!; Rules.Require(node["schema"]!.GetValue<int>() == 1 && node["rules"]!.GetValue<string>() == Catalog.WebVersion, "只支持浏览器v0.1.0存档"); node["schema"] = 2; node["rules"] = Catalog.Version;
        // Web loot offers contain raw equipment; native offers carry typed item claims.
        if (node["pending"]?["kind"]?.GetValue<string>() == "loot") { var options = node["pending"]!["options"]!.AsArray(); var wrapped = new JsonArray(); foreach (var item in options) wrapped.Add(new JsonObject { ["id"] = item!["id"]!.GetValue<string>(), ["type"] = "item", ["item"] = item.DeepClone() }); node["pending"]!["options"] = wrapped; }
        var s = node.Deserialize<GameState>(Json.Options)!; Model.Validate(c, s); return s;
    }
    public static void WriteAtomic(string path, GameState s) { Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path))!); string temp = path + ".tmp", backup = path + ".backup"; byte[] bytes = Encoding.UTF8.GetBytes(Encode(s)); using (var file = new FileStream(temp, FileMode.Create, FileAccess.Write, FileShare.None)) { file.Write(bytes); file.Flush(true); } if (File.Exists(path)) File.Replace(temp, path, backup); else File.Move(temp, path); }
    public static (GameState? State, bool Recovered) Read(Catalog c, string path) { if (!File.Exists(path) && !File.Exists(path + ".backup")) return (null, false); try { return (Decode(c, File.ReadAllText(path)), false); } catch { if (File.Exists(path + ".backup")) { try { return (Decode(c, File.ReadAllText(path + ".backup")), true); } catch { } } throw new InvalidDataException("主存档与备份都无法读取；未清空进度。请导入有效备份。"); } }
}
