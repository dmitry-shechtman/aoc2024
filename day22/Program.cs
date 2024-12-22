using System.Collections;

var input = File.ReadAllText("input.txt").Trim().Split('\n')
    .Select(long.Parse)
    .ToArray();

const int MAGIC = 130321;
int[] sums = new int[MAGIC];
BitArray seen = new(MAGIC);

Console.WriteLine(Part1());
Console.WriteLine(Part2());

long Part1() =>
    input.Sum(x => Enumerable.Range(0, 2000).Aggregate(x, MoveNext));

long Part2() =>
    input.Aggregate(0, Max);

long MoveNext(long secret, int _ = 0)
{
    secret ^= secret << 6;
    secret &= 0xFFFFFF;
    secret ^= secret >> 5;
    secret ^= secret << 11;
    secret &= 0xFFFFFF;
    return secret;
}

int Max(int max, long sec)
{
    seen.SetAll(false);
    int key = default;
    for (var (prev, curr, i) = (0, 0, 0); i <= 2000; sec = MoveNext(sec), ++i)
    {
        (prev, curr) = (curr, (int)(sec % 10));
        key = (key * 19 + (curr - prev) + 9) % MAGIC;
        if (i >= 4 && !seen[key] && (seen[key] = true))
            max = Math.Max(max, sums[key] += curr);
    }
    return max;
}
