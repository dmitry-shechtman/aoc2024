using aoc;
using aoc.Grids;

const int Penalty = 1000;

var input = File.ReadAllText("input.txt").Trim();
var multi = MultiGrid.Parse(input, "#SE", out var size);

var start = size.GetIndex(multi[1].Single());
var end   = size.GetIndex(multi[2].Single());
var walls = new bool[size.Length];
foreach (var pos in multi[0])
    size.SetValue(walls, pos, true);

var costs = new int[size.Length];
costs.Fill(int.MaxValue);
var paths = new PathValue[size.Length];
paths.Fill(new(int.MaxValue, new()));
Queue<QueueItem> queue = new();

Console.WriteLine(Part1());
Console.WriteLine(Part2());

int Part1()
{
    Vector vec, vecA, vecB;
    int pos, offset, offsetA, offsetB, cost, cost1;
    queue.Enqueue(new(start, Vector.East, 0));


    while (queue.TryDequeue(out var item))
    {
        (pos, vec, cost) = item;
        vecA = (vec.y, vec.x);
        vecB = -vecA;
        offset = size.GetIndex(vec);
        offsetA = size.GetIndex(vecA);
        offsetB = size.GetIndex(vecB);

        for (cost1 = cost + Penalty + 1;
            !walls[pos] && pos != end;
            pos += offset, ++cost, ++cost1)
        {
            if (TryAdd(pos + offsetA, cost1))
                queue.Enqueue(new(pos + offsetA, vecA, cost1));
            if (TryAdd(pos + offsetB, cost1))
                queue.Enqueue(new(pos + offsetB, vecB, cost1));

        }

        TryAdd(pos, cost);
    }

    return costs[end];
}

bool TryAdd(int pos, int cost)
{
    if (costs[pos] <= cost)
        return false;
    costs[pos] = cost;
    return true;
}

int Part2()
{
    Vector vec, vecA, vecB;
    int pos, offset, cost, cost2;
    queue.Enqueue(new(start, Vector.East, 0));
    List<int> path = new();

    while (queue.TryDequeue(out var item))
    {
        (pos, vec, cost) = item;
        vecA = (vec.y, vec.x);
        vecB = -vecA;
        offset = size.GetIndex(vec);

        path.Clear();

        for (cost2 = cost + Penalty;
            !walls[pos] && pos != end;
            pos += offset, ++cost, ++cost2)
        {
            if (TryAdd2(pos, cost2, path))
                queue.Enqueue(new(pos, vecA, cost2));
            if (TryAdd2(pos, cost2, path))
                queue.Enqueue(new(pos, vecB, cost2));
            path.Add(pos);
        }

        TryAdd2(pos, cost, path);
    }

    return CountPathItems();
}

bool TryAdd2(int pos, int cost, List<int> path)
{
    var (cost2, path2) = paths[pos];
    if (cost2 <= cost)
    {
        if (cost2 < cost)
            return false;
        for (int i = 1; i < path.Count; i++)
            path2.Add(path[i]);
        return true;
    }
    paths[pos] = new(cost, new(path));
    return true;
}

int CountPathItems()
{
    HashSet<int> set = new() { end };
    Queue<int> queue2 = new(set);
    while (queue2.TryDequeue(out var item))
        foreach (var pos2 in paths[item].Path)
            if (set.Add(pos2))
                queue2.Enqueue(pos2);
    return set.Count;
}

record struct QueueItem(int Pos, Vector Vec, int Cost);
record struct PathValue(int Cost, HashSet<int> Path);
