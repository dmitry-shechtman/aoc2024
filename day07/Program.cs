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
            2 => t[k] switch
            {
                < 10 => a * 10 + t[k],
                < 100 => a * 100 + t[k],
                < 1000 => a * 1000 + t[k],
                < 10000 => a * 10000 + t[k],
                < 100000 => a * 100000 + t[k],
                < 1000000 => a * 1000000 + t[k],
                _ => throw new InvalidOperationException()
            },
            _ => throw new NotImplementedException()
        }, t, k + 1, n))
            return true;
    }
    return false;
}
