using aoc;
using aoc.Grids;

var s = File.ReadAllText("input.txt").Trim();
var m = MultiGrid.Builder.Parse(s, "SMXA");

Console.WriteLine(Solve(MooreGrid.Headings
    .Select(h =>
        new[] { new[] { -h, h, h * 2 } })));

var hh2 = DiagGrid.Headings;
Console.WriteLine(Solve(hh2
    .Zip(hh2.Prepend(hh2[^1]), (h, g) =>
        new[] { new[] { g, -g }, new[] { h, -h } })));

int Solve(IEnumerable<Vector[][]> hhhh) => m[3].Sum(p =>
    hhhh.Count(hhh =>
        hhh.All(hh =>
            hh.Zip(m, (h, g) =>
                g.Contains(p + h)).All(b => b))));
