var s = File.ReadAllText("input.txt");
var v = new List<long>[] { new(), new() };

for (int i = 0, j, k; i < s.Length; ++i)
    for (j = 0, k = s[i] & 1, v[k].Add(0); j < 42; ++j)
        v[k][^1] |= (s[i++] & 1L) << j;

Console.WriteLine(Part1());

int Part1() =>
    v[0].Sum(a => v[1].Count(b => (a & b) == 0));
