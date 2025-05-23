﻿var input = File.ReadAllText("input.txt").Trim()
    .SelectMany((c, i) =>
        Enumerable.Repeat(i % 2 == 0 ? i / 2 : -1, c - '0'))
    .ToArray();

var check = input
    .Select((v, i) => v < 0 ? 0L : v * i)
    .Sum();

Console.WriteLine(Part1());
Console.WriteLine(Part2());

long Part1()
{
    var (d, z) = (input.ToArray(), check);
    for (int i = d.Length - 1, j = 0; i > j; --i)
    {
        if (d[i] < 0)
            continue;
        for (; d[j] >= 0; ++j) ;
        TrySwap(d, ref z, ref i, j);
    }
    return z;
}

long Part2()
{
    var (d, z) = (input.ToArray(), check);
    var c = new int[10];
    Array.Clear(c);
    int i, j, m, n;
    for (i = d.Length - 1; i > c[1]; --i)
    {
        if (d[i] < 0)
            continue;
        for (n = 1; i >= 0 && d[--i] == d[i + n]; ++n) ;
        for (++i, j = c[n], m = 0; j <= i && m < n; ++j)
            m = d[j] >= 0 ? 0 : ++m;
        c[n] = j;
        TrySwap(d, ref z, ref i, --j, n);
    }
    return z;
}

void TrySwap(int[] d, ref long z, ref int i, int j, int n = 1)
{
    if (i <= j)
        return;
    z += (long)(j - i - n + 1) * d[i] * n;
    for (; n > 0; ++i, --j, --n)
        (d[i], d[j]) = (-1, d[i]);
}
