using aoc;
using aoc.Grids;

var s = File.ReadAllText("input.txt").Trim();
var m = MultiGrid.Parse(s, "SAMX");

Console.WriteLine(Solve(Init1()));
Console.WriteLine(Solve(Init2()));

Vector[][][] Init1() => MooreGrid.Headings
    .Select(h => new[] { new[] { -h, default, h, h * 2 } })
    .ToArray();

Vector[][][] Init2() => DiagGrid.Headings
    .SelectMany((g, i) =>
        DiagGrid.Headings[..i].Select(h =>
            new[] { new[] { g, Vector.Zero, -g }, new[] { h, Vector.Zero, -h }}))
    .ToArray();

int Solve(Vector[][][] hhhh) => m[1].Sum(p =>
    hhhh.Count(hhh =>
        hhh.All(hh =>
            hh.Zip(m, (h, g) =>
                g.Contains(p + h)).All(b => b))));
