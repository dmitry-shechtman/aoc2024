using aoc;

const int MAGIC = 256, MAXDEPTH = 25;
const int N = 0, A = 4;
const string KEYS = "^>v<A0123456789";

var input = File.ReadAllText("input.txt").Trim()
    .Split('\n');

int[] index = new int[256];
for (int i = 0; i < KEYS.Length; i++)
    index[KEYS[i]] = i;

string[] paths = new string[MAGIC];
long[][] mins = new long[MAGIC][];

PriorityQueue<Vector, string> queue = new();
Init();

Console.WriteLine(Solve(2));
Console.WriteLine(Solve(25));

long Solve(int depth) =>
    input.Sum(s =>
        int.Parse(s[..^1]) * GetMin(s, depth));

long GetMin(string source, int depth)
{
    long sum = 0L;
    for (int i = 0, prev = A, curr; i < source.Length; i++, prev = curr)
        sum += GetMinPair(prev << 4 | (curr = index[source[i]]), depth);
    return sum;
}

long GetMinPair(int index, int depth) =>
    mins[index][depth] > 0
        ? mins[index][depth]
        : mins[index][depth] = GetMin(paths[index], depth - 1);

void Init()
{
    Set(N,
        (1, 0), (2, 1), (1, 1), (0, 1), (2, 0), (0, 0));
    Set(A,
        (2, 3), (1, 3), (0, 2), (1, 2), (2, 2), (0, 1),
        (1, 1), (2, 1), (0, 0), (1, 0), (2, 0), (0, 3));
}

void Set(int start, params Vector[] pad)
{
    for (int i = 0, a = i + start; i < pad.Length - 1; ++i, ++a)
    {
        for (int j = 0, b = j + start; j < pad.Length - 1; ++j, ++b)
        {
            mins[a << 4 | b] = new long[MAXDEPTH + 1];
            mins[a << 4 | b][0] = (paths[a << 4 | b] =
                GetPath(pad[i], pad[j], pad[^1])).Length;
        }
    }
}

string GetPath(Vector start, Vector end, Vector wall)
{
    var min = (rank: int.MaxValue, path: string.Empty);
    int rank;
    queue.Enqueue(start, string.Empty);
    while (queue.TryDequeue(out var pos, out var path))
        if (pos != wall && !TryAdd(pos, end, path))
            if ((rank = Rank(path)) < min.rank)
                min = (rank, path);
    return $"{min.path}A";
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

int Rank(string path)
{
    if (path.Length < 2)
        return 0;
    var (i, turned) = (0, false);
    while (i < path.Length - 1)
        if (path[i] != path[++i])
            if (turned)
                return int.MaxValue;
            else
                turned = true;
    return Math.Max(1, index[path[i]]);
}
