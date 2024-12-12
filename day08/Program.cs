using aoc;
using aoc.Grids;

var s = File.ReadAllText("input.txt");
var m = MultiGrid.Parse(s, char.IsLetterOrDigit, out var z);

Console.WriteLine(Solve(1, 1));
Console.WriteLine(Solve(0, Math.Max(z.Width, z.Height)));

int Solve(int start, int count) => m[..^1]
    .SelectMany(g => g
        .SelectMany(a => g
            .Where(b => a != b)
            .Select(b => (b, v: b - a))
            .SelectMany(t => Enumerable.Range(start, count)
                .Select(i => t.b + t.v * i)
                .TakeWhile(z.Contains))))
    .Distinct()
    .Count();
