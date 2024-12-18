using aoc;
using aoc.Grids;

var input = File.ReadAllText("input.txt").Trim();
var points = input.Split('\n')
    .Select(Vector.Parse)
    .ToArray();

VectorRange range = points.Range();
Size size = new(range);

HashSet<Vector> balls = new();
PriorityQueue<Vector, int> queue = new();
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
    queue.Enqueue(default, 1);
    while (queue.TryDequeue(out var pos0, out dist))
        foreach (var vec in headings)
            if (size.Contains(pos = pos0 + vec) && balls.Add(pos))
                if (pos == range.Max)
                    return true;
                else
                    queue.Enqueue(pos, dist + 1);
    dist = 0;
    return false;
}
