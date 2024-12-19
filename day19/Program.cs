var input = File.ReadAllText("input.txt").Trim().Split("\n\n");
var towels = input[0].Split(", ").ToHashSet();
var patterns = input[1].Split("\n");

Dictionary<string, long> counts = new();

Console.WriteLine(Part1());
Console.WriteLine(Part2());

int Part1() =>
    patterns.Count(s => Count(s) > 0);

long Part2() =>
    patterns.Sum(s => counts[s]);

long Count(string s) => counts.TryGetValue(s, out var count)
    ? count
    : counts[s] = (towels.Contains(s) ? 1 : 0) +
        Enumerable.Range(1, s.Length < 8 ? s.Length - 1 : 8)
            .Sum(i => towels.Contains(s[..i]) ? Count(s[i..]) : 0);
