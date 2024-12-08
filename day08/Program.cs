using aoc;
using aoc.Grids;

var s = File.ReadAllText("input.txt");
var t = Enumerable.Range(48, 75).Select(i => (char)i);
var m = MultiGrid.Parse(s, t.ToArray());
var r = VectorRange.FromField(s);

Console.WriteLine(Solve(1, 1));
Console.WriteLine(Solve(0, Math.Max(r.Max.X, r.Max.Y)));

int Solve(int start, int count) => m[..^1]
    .SelectMany(g => g
        .SelectMany(a => g
            .Where(b => a != b)
            .Select(b => (b, v: b - a))
            .SelectMany(t => Enumerable.Range(start, count)
                .Select(i => t.b + t.v * i)
                .Where(r.Contains))))
    .Distinct()
    .Count();
