var vv = File.ReadAllLines("input.txt")
    .Select(ParseOne);

Console.WriteLine(Solve(1));
Console.WriteLine(Solve(2));

List<long> ParseOne(string t)
{
    var l = 0L;
    var i = 0;
    for (; t[i] != ':'; i++)
        l = l * 10 + t[i] - '0';
    var v = new List<long>() { l };
    for ((i, l) = (i + 2, 0); i < t.Length; ++i)
        if (t[i] == ' ')
        {
            v.Add(l);
            l = 0L;
        }
        else
            l = l * 10 + t[i] - '0';
    v.Add(l);
    return v;
}

long Solve(int p) =>
    vv.AsParallel().Sum(t => IsMatch(t[1], t, 2, p + 1) ? t[0] : 0);

bool IsMatch(long a, List<long> t, int k, int n)
{
    if (k == t.Count)
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
