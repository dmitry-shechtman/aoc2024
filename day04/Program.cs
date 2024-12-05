using aoc;
using aoc.Grids;

var s = File.ReadAllText("input.txt").Trim();
var m = MultiGrid.Parse(s, "SMXA");

Console.WriteLine(Solve(MooreGrid.Headings
    .Select(h =>
        new[] { new[] { -h, h, h * 2 } })));

Console.WriteLine(Solve(DiagGrid.Headings
    .SelectMany((g, i) =>
        DiagGrid.Headings[..i].Select(h =>
            new[] { new[] { g, -g }, new[] { h, -h } }))));

int Solve(IEnumerable<Vector[][]> hhhh) => m[3].Sum(p =>
    hhhh.Count(hhh =>
        hhh.All(hh =>
            hh.Zip(m, (h, g) =>
                g.Contains(p + h)).All(b => b))));
