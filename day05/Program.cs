using aoc;

var input = File.ReadAllText("input.txt")
    .Trim().Split("\n\n");

var rr = Split(input[0], '|')
    .Select(t => (t[0], t[1]))
    .ToHashSet();

var uu = Split(input[1], ',');

Console.WriteLine(Solve(u => !Sort(u)));
Console.WriteLine(Solve(Sort));

IEnumerable<int[]> Split(string s, char c) =>
    s.Split('\n').Select(t =>
        t.Split(c).Select(int.Parse).ToArray());

int Solve(Func<int[], bool> p) =>
    uu.Where(p)
        .Sum(u => u[u.Length / 2]);

bool Sort(int[] u)
{
    if (u.All((x, i) => u[(i + 1)..].All(y => rr.Contains((x, y)))))
        return false;
    u.Sort((x, y) => rr.Contains((x, y)) ? -1 : 1);
    return true;
}
