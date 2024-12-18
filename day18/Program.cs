using aoc;
using aoc.Grids;

var input = File.ReadAllText("input.txt").Trim();
var points = input.Split('\n')
    .Select(Vector.Parse)
    .ToArray();

VectorRange range = points.Range();
Size size = new(range);

HashSet<Vector> balls = new();
Queue<QueueItem> queue = new();
Vector[] headings = Grid.Headings;

Console.WriteLine(Part1());
Console.WriteLine(Part2());

int Part1() =>
    TryFindShortestPath(1024, out var value) ? value : 0;

Vector Part2() =>
    FindBlock(1024, points.Length);

Vector FindBlock(int start, int end)
{
    var pivot = (start + end) / 2;
    var found = TryFindShortestPath(pivot, out _);
    return end - start == 1
        ? points[found ? pivot : pivot - 1]
        : FindBlock(found ? pivot : start, found ? end : pivot);
}

bool TryFindShortestPath(int index, out int dist)
{
    Vector pos;
    balls.Clear();
    points[..index].All(balls.Add);
    queue.Clear();
    queue.Enqueue(default);
    while (queue.TryDequeue(out var item) && (dist = ++item.Dist) > 0)
        foreach (var vec in headings)
            if (vec != item.Vec && size.Contains(pos = item.Pos + vec))
                if (balls.Add(pos))
                    if (pos == range.Max)
                        return true;
                    else
                        queue.Enqueue(new(pos, -vec, dist));
    dist = 0;
    return false;
}

record struct QueueItem(Vector Pos, Vector Vec, int Dist);
