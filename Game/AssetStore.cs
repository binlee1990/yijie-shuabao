using Godot;
using System.Text.Json;
namespace Shuabao.Game;
public sealed class AssetStore
{
    private readonly Dictionary<string, string> _paths = [];
    private readonly Dictionary<string, Texture2D> _textures = [];
    public AssetStore() { using var json = JsonDocument.Parse(Godot.FileAccess.GetFileAsString("res://Assets/manifest.json")); foreach (var a in json.RootElement.GetProperty("assets").EnumerateObject()) { string path = a.Value.GetProperty("path").GetString()!; _paths[a.Name] = "res://Assets/" + path["assets/".Length..]; } }
    public Texture2D? Texture(string id) { if (_textures.TryGetValue(id, out var texture)) return texture; if (!_paths.TryGetValue(id, out var path)) return null; var loaded = ResourceLoader.Load<Texture2D>(path); if (loaded != null) _textures[id] = loaded; return loaded; }
    public AudioStream? Audio(string id) => _paths.TryGetValue(id, out var p) ? ResourceLoader.Load<AudioStream>(p) : null;
    public Font Font() { var list = new Godot.Collections.Array<Font>(); foreach (var path in _paths.Where(x => x.Key.StartsWith("font.")).Select(x => x.Value.Replace(".woff2", ".otf")).Order()) { var f = ResourceLoader.Load<FontFile>(path); if (f != null) list.Add(f); } var font = new SystemFont { FontNames = ["Microsoft YaHei", "Noto Serif CJK SC", "Arial"], Fallbacks = list }; return font; }
}
