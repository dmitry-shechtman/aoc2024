using aoc;
using System.Collections.Concurrent;

const int MAGIC = 256;
const string KEYS = "^>v<A0123456789";

var input = File.ReadAllText("input.txt").Trim()
    .Split('\n');

string[,][] paths = new string[MAGIC, MAGIC][];

ConcurrentDictionary<int, long>[,] mins =
    new ConcurrentDictionary<int, long>[MAGIC, MAGIC];

PriorityQueue<Vector, string> queue = new();
Init();

Console.WriteLine(Solve(2));
Console.WriteLine(Solve(25));

long Solve(int depth) =>
    input.Sum(s =>
        int.Parse(s[..^1]) * GetMin(s, depth));

long GetMin(string source, int depth) =>
    $"A{source}".Zip(source, (c, d) =>
        GetMinPair(c, d, depth)).Sum();

long GetMinPair(int c, int d, int depth) =>
    mins[c, d].GetOrAdd(depth, _ =>
        paths[c, d].Min(s => GetMin(s, depth - 1)));

int FindMin(string source, int depth) =>
    FindPaths($"A{source}").Select(s => depth > 0
        ? FindMin(s, depth - 1)
        : s.Length).Min();

IEnumerable<string> FindPaths(string source) =>
    source.Length > 1
        ? paths[source[0], source[1]].SelectMany(s =>
            FindPaths(source[1..]).Select(t => $"{s}{t}"))
        : new[] { string.Empty };

void Init()
{
    Set(0,
        (1, 0), (2, 1), (1, 1), (0, 1), (2, 0), (0, 0));
    Set(4,
        (2, 3), (1, 3), (0, 2), (1, 2), (2, 2), (0, 1),
        (1, 1), (2, 1), (0, 0), (1, 0), (2, 0), (0, 3));
}

void Set(int start, params Vector[] pad)
{
    for (int i = 0, a = i + start; i < pad.Length - 1; ++i, ++a)
        for (int j = 0, b = j + start; j < pad.Length - 1; ++j, ++b)
            mins[KEYS[a], KEYS[b]] = new(new[] {
                KeyValuePair.Create(0, (long)(paths[KEYS[a], KEYS[b]] =
                    GetPaths(pad[i], pad[j], pad[^1]).ToArray())[0].Length) });
}

IEnumerable<string> GetPaths(Vector start, Vector end, Vector wall)
{
    queue.Enqueue(start, string.Empty);
    while (queue.TryDequeue(out var pos, out var path))
        if (pos != wall && !TryAdd(pos, end, path))
            yield return $"{path}A";
}

bool TryAdd(Vector pos, Vector end, string path)
{
    if (pos == end)
        return false;
    if (pos.x < end.x)
        queue.Enqueue(pos + (1, 0), $"{path}>");
    else if (pos.x > end.x)
        queue.Enqueue(pos - (1, 0), $"{path}<");
    if (pos.y > end.y)
        queue.Enqueue(pos - (0, 1), $"{path}^");
    else if (pos.y < end.y)
        queue.Enqueue(pos + (0, 1), $"{path}v");
    return true;
}
