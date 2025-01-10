using aoc.Grids;

var s = File.ReadAllText("input.txt").Trim();
var m = MultiGrid.Parse(s, char.IsLetterOrDigit, out var z);

Console.WriteLine(Solve(1, 1));
Console.WriteLine(Solve(0, Math.Max(z.Width, z.Height)));

int Solve(int start, int count) => m[..^1]
    .SelectMany(g => g
        .SelectMany(a => g
            .Where(b => a != b)
            .SelectMany(b => Enumerable.Range(start, count)
                .Select(i => b + (b - a) * i)
                .TakeWhile(z.Contains))))
    .Distinct()
    .Count();
