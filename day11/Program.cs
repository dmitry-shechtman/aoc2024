Dictionary<long, long> cache = new();

var input = File.ReadAllText("input.txt").Trim()
    .Split(' ')
    .Select(long.Parse)
    .ToArray();

Console.WriteLine(Solve(25));
Console.WriteLine(Solve(75));

long Solve(int n)
{
    long sum = 0;
    for (int i = 0; i < input.Length; i++)
        sum += Calc(input[i], n);
    return sum;
}

long Calc(long value, long depth)
{
    if (depth == 0)
        return 1;
    if (value == 0)
        return Calc(1, depth - 1);
    long key, div, mod;
    if (!cache.TryGetValue(key = value << 7 | --depth, out long r))
    {
        for ((div, mod) = (10, 10); r == 0; (div, mod) = (div * 100, mod * 10))
            r = value < div
                ? Calc(value * 2024, depth)
                : value < div * 10
                    ? Calc(value / mod, depth) + Calc(value % mod, depth)
                    : 0;
        cache.Add(key, r);
    }
    return r;
}
