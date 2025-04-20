using aoc;
using aoc.Grids;

var (filename, min) = args.Length switch
{
    0 => ("input.txt", 100),
    1 => (args[0], 100),
    _ => (args[0], int.Parse(args[1])),
};

var input = File.ReadAllText("input.txt").AsSpan().Trim();
var points = new Vector[2];
var walls = Grid.Builder.Parse(input, "SE", points);
var (start, end) = (points[0], points[1]);

Dictionary<Vector, int> dists = new();

var path = FindShortestPath();

Console.WriteLine(Part1());

int Part1() =>
    path.AsSpan()[..^(min + 2)].Sum((p, i) =>
        path.AsSpan()[(i + min + 2)..].Count(q => (q - p).Abs() <= 2));

Vector[] FindShortestPath()
{
    Vector pos;
    PriorityQueue<Vector[], int> queue = new();
    queue.Enqueue(new[] { start }, 1);
    while (queue.TryDequeue(out var path, out var dist))
        foreach (var vec in Grid.Headings)
            if (path[^1] == end)
                return path;
            else if (TryAdd(pos = path[^1] + vec, dist))
                queue.Enqueue(path.Append(pos).ToArray(), dist + 1);
    throw new InvalidOperationException();
}

bool TryAdd(Vector pos, int dist)
{
    if (walls.Contains(pos) ||
        dists.GetValueOrDefault(pos, int.MaxValue) < dist)
            return false;
    dists[pos] = dist;
    return true;
}
