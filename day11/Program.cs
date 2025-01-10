Dictionary<long, long> cache = new();

var input = File.ReadAllText("input.txt").Trim()
    .Split(' ')
    .Select(long.Parse);

Console.WriteLine(Solve(25));
Console.WriteLine(Solve(75));

long Solve(int n) =>
    input.Sum(v => Calc(v, n));

long Calc(long v, long d)
{
    if (d == 0)
        return 1;
    long p, m;
    return cache.GetOrAdd(v << 7 | --d, _ =>
        v == 0
            ? Calc(1, d)
            : ((p = (long)Math.Log10(v) + 1) & 1) != 0
                ? Calc(v * 2024, d)
                : Calc(v / (m = (long)Math.Pow(10, p >> 1)), d) + Calc(v % m, d));
}
