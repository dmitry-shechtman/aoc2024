var s = File.ReadAllText("input.txt").Trim();
var v = new List<int[]>[] { new(), new() };

for (int i = 0, k, x, y; i < s.Length; ++i)
    for (y = 0, k = s[i] & 1, v[k].Add(new int[5]); y < 7; ++y, ++i)
        for (x = 0; x < 5; ++x)
            v[k][^1][x] += s[i++] & 1;

Console.WriteLine(Part1());

int Part1() =>
    v[0].Sum(a => v[1].Count(b => a.Zip(b, (a, b) => a + b <= 7).All(_ => _)));
