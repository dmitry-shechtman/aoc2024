var vv = File.ReadAllLines("input.txt")
    .Select(ParseOne);

Console.WriteLine(Solve(1));
Console.WriteLine(Solve(2));

(long z, long[] v) ParseOne(string t)
{
    var z = 0L;
    var j = 0;
    for (; t[j] != ':'; j++)
        z = z * 10 + t[j] - '0';
    var v = new long[t.Count(c => c == ' ')];
    var l = 0L;
    for ((var i, j) = (0, j + 2); j < t.Length; ++j)
        if (t[j] == ' ')
            (v[i++], l) = (l, 0L);
        else
            l = l * 10 + t[j] - '0';
    v[^1] = l;
    return (z, v);
}

long Solve(int p) =>
    vv.AsParallel().Sum(t => IsMatch(t.v[0], t, 1, p + 1) ? t.z : 0);

bool IsMatch(long a, (long z, long[] v) t, int k, int n) =>
    k < t.v.Length
        ? t.z >= a && IsMatch2(a, t, k, n)
        : t.z == a;

bool IsMatch2(long a, (long z, long[] v) t, int k, int n)
{
    for (int i = 0; i < n; i++)
        if (IsMatch(Calc(a, t, k, i), t, k + 1, n))
            return true;
    return false;
}

long Calc(long a, (long z, long[] v) t, int k, int i) => i switch
{
    0 => a + t.v[k],
    1 => a * t.v[k],
    2 => Concat(a, t.v[k]),
    _ => throw new NotImplementedException()
};

long Concat(long x, long y)
{
    for (var z = y; z > 0; z /= 10)
        x *= 10;
    return x + y;
}
