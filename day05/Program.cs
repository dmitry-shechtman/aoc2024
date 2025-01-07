var input = File.ReadAllText("input.txt")
    .Trim().Split("\n\n");

var rr = Split(input[0], '|')
    .Select(t => (t[0], t[1]))
    .ToHashSet();

var pp = new int[2];
foreach (var u in Split(input[1], ','))
{
    var i = u.Zip(u[1..], (x, y) =>
        rr.Contains((x, y))).All(b => b) ? 0 : 1;
    u.Sort((x, y) => rr.Contains((x, y)) ? -1 : 1);
    pp[i] += u[u.Length / 2];
}

Console.WriteLine(pp[0]);
Console.WriteLine(pp[1]);

IEnumerable<int[]> Split(string s, char c) =>
    s.Split('\n').Select(t =>
        t.Split(c).Select(int.Parse).ToArray());
