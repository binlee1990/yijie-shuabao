namespace Shuabao.Core;
public sealed class RandomStream
{
    private uint _n;
    public RandomStream(long seed, string address) { _n = Hash($"{seed}/{address}"); }
    public static uint Hash(string text) { unchecked { uint n = 2166136261; foreach (var rune in text.EnumerateRunes()) { n ^= (uint)(rune.Value > 0xffff ? char.ConvertFromUtf32(rune.Value)[0] : rune.Value); n *= 16777619; } return n; } }
    public double Next() { unchecked { _n += 0x6D2B79F5; uint t = _n; t = (t ^ (t >> 15)) * (t | 1); t ^= t + ((t ^ (t >> 7)) * (t | 61)); return (t ^ (t >> 14)) / 4294967296d; } }
    public T Pick<T>(IReadOnlyList<T> values) => values[(int)(Next() * values.Count)];
    public string Weighted(IReadOnlyList<(string Id, int Weight)> entries) { double n = Next() * entries.Sum(e => e.Weight); foreach (var e in entries) { n -= e.Weight; if (n < 0) return e.Id; } return entries[^1].Id; }
}
