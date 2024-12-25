var s = File.OpenText("input.txt");
var v = new List<int[]>[] { new(), new() };

for (int k, x, y; s.Peek() >= 0; s.Read())
    for (y = 0, k = s.Peek() & 1, v[k].Add(new int[5]); y < 7; ++y, s.Read())
        for (x = 0; x < 5; ++x)
            v[k][^1][x] += s.Read() & 1;

Console.WriteLine(Part1());

int Part1() =>
    v[0].Sum(a => v[1].Count(b => a.Zip(b, (a, b) => a + b < 8).All(_ => _)));
