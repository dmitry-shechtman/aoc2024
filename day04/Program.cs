using aoc;
using aoc.Grids;

var s = File.ReadAllText("input.txt").Trim();
var m = MultiGrid.Parse(s, "SAMX");

Console.WriteLine(Solve(Init1()));
Console.WriteLine(Solve(Init2()));

Vector[][][] Init1() => MooreGrid.Headings
    .Select(h => "SAMX".Select((_, i) => new[] { h * (i - 1) }).ToArray())
    .ToArray();

Vector[][][] Init2() => DiagGrid.Headings
    .SelectMany((g, i) =>
        DiagGrid.Headings[..i].Select(h =>
            new[] { new[] { g, h }, new[] { Vector.Zero }, new[] { -g, -h }}))
    .ToArray();

int Solve(Vector[][][] hhhh) => m[1].Sum(p =>
    hhhh.Count(hhh =>
        hhh.Zip(m, (hh, g) =>
            hh.All(h =>
                g.Contains(p + h))).All(b => b)));
