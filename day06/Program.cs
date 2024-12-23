using aoc;
using static aoc.Grids.Grid;

var s = File.ReadAllText("input.txt").AsSpan().Trim();
var p = new Vector[4];
var g = Parse(s, "^>v<", p, out var z);
var h = p.FindIndex(r => r != default);
var o = p[h];

Dictionary<Vector, int> qq = new();
Walk(g, o, h, qq);

Console.WriteLine(qq.Count);
Console.WriteLine(qq.ToArray()[1..]
    .AsParallel()
    .Count(IsLoop));

bool Walk(HashSet<Vector> pp, Vector p, int k, Dictionary<Vector, int>? qq)
{
    HashSet<(Vector, int)> rr = new();
    for (; z.Contains(p); p += Headings[k])
        if (!pp.Contains(p))
            qq?.TryAdd(p, k);
        else if (!rr.Add((p, k) = (p - Headings[k], (k + 1) & 3)))
            return true;
    return false;
}

bool IsLoop(KeyValuePair<Vector, int> t) =>
    Walk(new(g.Append(t.Key)), t.Key, t.Value, null);
