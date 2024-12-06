using aoc;
using static aoc.Grids.Grid;

var s = File.ReadAllText("input.txt").Trim();
var g = Parse(s);
var r = VectorRange.FromField(s);
var o = Vector.FindChar(s, '^');

HashSet<Vector> qq;
HashSet<Vector3D> rr;
Walk(g);

Console.WriteLine(qq.Count);
Console.WriteLine(qq.Count(IsLoop));

bool Walk(HashSet<Vector> pp)
{
    (qq, rr) = (new(), new());
    for (var (p, k) = (o, 0); r.Contains(p); p += Headings[k])
        if (!pp.Contains(p))
            qq.Add(p);
        else if (!rr.Add((p, k) = (p - Headings[k], (k + 1) & 3)))
            return true;
    return false;
}

bool IsLoop(Vector q) =>
    Walk(new(g.Append(q)));
