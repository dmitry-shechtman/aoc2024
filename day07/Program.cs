var Pow10 = new[] { 10, 100, 1000, 10000 };

var vv = File.ReadAllLines("input.txt")
    .Select(ParseOne);

Console.WriteLine(Solve(1));
Console.WriteLine(Solve(2));

(long v, int l)[] ParseOne(string s)
{
    var t = new (long v, int l)[16];
    long z = 0L, l = 0L;
    int i = 1, j = 0, k = 0;
    for (; s[j] != ':'; ++j)
        z = z * 10 + s[j] - '0';
    for (j += 2, k = j; k < s.Length; ++k)
    {
        if (s[k] == ' ')
        {
            t[i++] = (l, k - j - 1);
            j = k + 1;
            l = 0L;
        }
        else
        {
            l = l * 10 + s[k] - '0';
        }
    }
    t[i++] = (l, k - j - 1);
    t[0] = (z, i);
    return t;
}

long Solve(int p) =>
    vv.AsParallel().Sum(t => IsMatch(t[1].v, t, 2, p + 1) ? t[0].v : 0);

bool IsMatch(long a, ReadOnlySpan<(long v, int l)> t, int k, int n)
{
    if (k == t[0].l)
        return t[0].v == a;
    if (t[0].v < a)
        return false;
    for (int i = 0; i < n; ++i)
    {
        if (IsMatch(i switch
        {
            0 => a + t[k].v,
            1 => a * t[k].v,
            2 => a * Pow10[t[k].l] + t[k].v,
            _ => throw new NotImplementedException()
        }, t, k + 1, n))
        {
            return true;
        }
    }
    return false;
}
