using aoc;
using aoc.Grids;

var ss = File.ReadAllText("input.txt").Trim().Split('\n');

Console.WriteLine(Solve("XMAS", Init1()));
Console.WriteLine(Solve("AMSMS", Init2()));

Vector[][] Init1() => MooreGrid.Headings
    .Select(h => "XMAS".Select((_, i) => h * i).ToArray())
    .ToArray();

Vector[][] Init2() => DiagGrid.Headings
    .SelectMany((g, i) =>
        DiagGrid.Headings[..i].Select(h =>
            new[] { Vector.Zero, g, -g, h, -h }))
    .ToArray();

int Solve(string t, Vector[][] hhh) => ss.Sum((s, y) =>
    s.Sum((_, x) =>
        hhh.Count(hh =>
            hh.Zip(t, (h, c) =>
                IsMatch(x + h.X, y + h.Y, c)).All(b => b))));

bool IsMatch(int x, int y, char c) =>
    y >= 0 && y < ss.Length &&
    x >= 0 && x < ss[y].Length &&
    ss[y][x] == c;
