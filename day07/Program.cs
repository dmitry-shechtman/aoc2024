var vv = File.ReadAllLines("input.txt")
    .Select(s => s.Split(": "))
    .Select(ParseOne);

Console.WriteLine(Solve(Enum1));
Console.WriteLine(Solve(Enum2));

(long z, long[] v) ParseOne(string[] tt) =>
    (long.Parse(tt[0]), tt[1].Split(' ').Select(long.Parse).ToArray());

long Solve(Func<long, long, IEnumerable<long>> f) =>
    vv.AsParallel().Sum(t => EnumAll(t, f, t.v.Length - 1).Contains(t.z) ? t.z : 0);

IEnumerable<long> EnumAll((long z, long[] v) t, Func<long, long, IEnumerable<long>> f, int i) =>
    i == 0
        ? EnumOne(t.v[i])
        : EnumAll(t, f, i - 1).SelectMany(x => f(x, t.v[i]));

IEnumerable<long> EnumOne(long x)
{
    yield return x;
}

IEnumerable<long> Enum1(long x, long y)
{
    yield return x + y;
    yield return x * y;
}

IEnumerable<long> Enum2(long x, long y) =>
    Enum1(x, y).Append(Concat(x, y));

long Concat(long x, long y)
{
    for (var z = y; z > 0; z /= 10)
        x *= 10;
    return x + y;
}
