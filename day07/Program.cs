var vv = File.ReadAllLines("input.txt")
    .Select(ParseOne);

Console.WriteLine(Solve(1));
Console.WriteLine(Solve(2));

long[] ParseOne(string t)
{
    var l = 0L;
    var j = 0;
    for (; t[j] != ':'; j++)
        l = l * 10 + t[j] - '0';
    var v = new long[t.Count(c => c == ' ') + 1];
    v[0] = l;
    for ((var i, j, l) = (1, j + 2, 0); j < t.Length; ++j)
        if (t[j] == ' ')
            (v[i++], l) = (l, 0L);
        else
            l = l * 10 + t[j] - '0';
    v[^1] = l;
    return v;
}

long Solve(int p) =>
    vv.AsParallel().Sum(t => IsMatch(t[1], t, 2, p + 1) ? t[0] : 0);

bool IsMatch(long a, long[] t, int k, int n)
{
    if (k == t.Length)
        return t[0] == a;
    if (t[0] < a)
        return false;
    for (int i = 0; i < n; i++)
    {
        if (IsMatch(i switch
        {
            0 => a + t[k],
            1 => a * t[k],
            2 => Concat(a, t[k]),
            _ => throw new NotImplementedException()
        }, t, k + 1, n))
            return true;
    }
    return false;
}

long Concat(long x, long y)
{
    for (var z = y; z > 0; z /= 10)
        x *= 10;
    return x + y;
}
