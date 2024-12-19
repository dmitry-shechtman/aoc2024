using System.Collections.Concurrent;

var input = File.ReadAllText("input.txt").Trim().Split("\n\n");
var towels = input[0].Split(", ");
var patterns = input[1].Split("\n");

ConcurrentDictionary<string, long> counts = new();

Console.WriteLine(Part1());
Console.WriteLine(Part2());

int Part1() =>
    patterns.AsParallel().Count(s => Count(s) > 0);

long Part2() =>
    patterns.Sum(s => counts[s]);

long Count(string s) => counts.GetOrAdd(s,
    s => (towels.Contains(s) ? 1 : 0) +
        towels.Where(t => t.Length < s.Length && s[..t.Length] == t)
            .Sum(t => Count(s[t.Length..])));
