using aoc;
using System.Collections.Concurrent;

var input = File.ReadAllText("input.txt").Trim().Split('\n')
    .Select(long.Parse)
    .ToArray();

ConcurrentDictionary<Vector4D, int> sums = new();

Console.WriteLine(Part1());
Console.WriteLine(Part2());

long Part1() =>
    input.Sum(x => Enumerable.Range(0, 2000).Aggregate(x, MoveNext));

long Part2() =>
    input.AsParallel().Max(Max);

long MoveNext(long secret, int _ = 0)
{
    secret = (secret ^ (secret << 6)) % 16777216;
    secret = (secret ^ (secret >> 5)) % 16777216;
    secret = (secret ^ (secret << 11)) % 16777216;
    return secret;
}

int Max(long sec)
{
    int max = 0;
    HashSet<Vector4D> seen = new();
    Vector4D key = default;
    for (var (prev, curr, i) = (0, 0, 0); i <= 2000; sec = MoveNext(sec), ++i)
    {
        (prev, curr) = (curr, (int)(sec % 10));
        key = (key.y, key.z, key.w, curr - prev);
        if (i >= 4 && seen.Add(key))
            max = Math.Max(max, sums.AddOrUpdate(key, curr, (_, sum) => sum + curr));
    }
    return max;
}
