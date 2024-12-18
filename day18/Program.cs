using aoc;
using aoc.Grids;

var input = File.ReadAllText("input.txt").Trim();
var points = input.Split('\n')
    .Select(Vector.Parse)
    .ToArray();

VectorRange range = points.Range();
Size size = new(range);

HashSet<Vector> walls = new(points[..1024]);
Dictionary<Vector, int> costs = new();
Queue<QueueItem> queue = new();
Vector[] headings = Grid.Headings;

Console.WriteLine(Part1());
Console.WriteLine(Part2());

int Part1() =>
    TryFindShortestPath(out var value) ? value : 0;

Vector Part2() =>
    FindBlock(walls.Count, points.Length);

Vector FindBlock(int start, int end)
{
    var pivot = (start + end) / 2;
    walls.Clear();
    points[..pivot].All(walls.Add);
    var found = TryFindShortestPath(out _);
    return end - start == 1
        ? points[found ? pivot : pivot - 1]
        : FindBlock(found ? pivot : start, found ? end : pivot);
}

bool TryFindShortestPath(out int cost)
{
    Vector pos;
    costs.Clear();
    queue.Clear();
    queue.Enqueue(default);
    while (queue.TryDequeue(out var item))
        foreach (var vec in headings)
            if (vec != item.Vec && size.Contains(pos = item.Pos + vec) &&
                !walls.Contains(pos) && TryAdd(pos, item.Cost + 1))
                    queue.Enqueue(new(pos, -vec, item.Cost + 1));
    return costs.TryGetValue(range.Max, out cost);
}

bool TryAdd(Vector pos, int cost)
{
    if (costs.TryGetValue(pos, out var cost2) && cost2 <= cost)
        return false;
    costs[pos] = cost;
    return true;
}

record struct QueueItem(Vector Pos, Vector Vec, int Cost);
