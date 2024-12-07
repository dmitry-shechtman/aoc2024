using aoc;
using static aoc.Grids.Grid;

var s = File.ReadAllText("input.txt").Trim();
var g = Parse(s);
var r = VectorRange.FromField(s);
var o = Vector.FindChar(s, '^');

Dictionary<Vector, int> qq = new();
Walk(g, o, 0, qq);

Console.WriteLine(qq.Count);
Console.WriteLine(qq.ToArray()[1..]
    .AsParallel()
    .Count(IsLoop));

bool Walk(HashSet<Vector> pp, Vector p, int k, Dictionary<Vector, int>? qq)
{
    HashSet<(Vector, int)> rr = new();
    for (; r.Contains(p); p += Headings[k])
        if (!pp.Contains(p))
            qq?.TryAdd(p, k);
        else if (!rr.Add((p, k) = (p - Headings[k], (k + 1) & 3)))
            return true;
    return false;
}

bool IsLoop(KeyValuePair<Vector, int> t) =>
    Walk(new(g.Append(t.Key)), t.Key, t.Value, null);
