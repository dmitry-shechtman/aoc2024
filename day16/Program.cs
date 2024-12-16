using aoc;
using aoc.Grids;

const int Penalty = 1000;

var input = File.ReadAllText("input.txt").Trim();
var multi = MultiGrid.Parse(input, "#SE");
var (walls, start, end) = (multi[0], multi[1].Single(), multi[2].Single());

Dictionary<Vector, int> costs = new();
Dictionary<Vector, PathValue> paths = new();
Queue<QueueItem> queue = new();

Console.WriteLine(Part1());
Console.WriteLine(Part2());

int Part1()
{
    Vector pos, vec, vecA, vecB;
    int cost, cost1;
    queue.Enqueue(new(start, Vector.East, 0));


    while (queue.TryDequeue(out var item))
    {
        vec = item.Vec;
        vecA = (vec.y, vec.x);
        vecB = -vecA;


        for (pos = item.Pos, cost = item.Cost, cost1 = cost + Penalty + 1;
            !walls.Contains(pos) && pos != end;
            pos += vec, ++cost, ++cost1)
        {
            if (TryAdd(pos + vecA, cost1))
                queue.Enqueue(new(pos + vecA, vecA, cost1));
            if (TryAdd(pos + vecB, cost1))
                queue.Enqueue(new(pos + vecB, vecB, cost1));

        }

        TryAdd(pos, cost);
    }

    return costs[end];
}

bool TryAdd(Vector pos, int cost)
{
    if (costs.TryGetValue(pos, out var cost2) && cost2 <= cost)
        return false;
    costs[pos] = cost;
    return true;
}

int Part2()
{
    queue.Enqueue(new(start, Vector.East, 0));
    Vector pos, vec, vecA, vecB;
    int cost, cost2;
    List<Vector> path = new();

    while (queue.TryDequeue(out var item))
    {
        vec = item.Vec;
        vecA = (vec.y, vec.x);
        vecB = -vecA;
        path.Clear();

        for (pos = item.Pos, cost = item.Cost, cost2 = cost + Penalty;
            !walls.Contains(pos) && pos != end;
            pos += vec, ++cost, ++cost2)
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

bool TryAdd2(Vector pos, int cost, List<Vector> path)
{
    if (paths.TryGetValue(pos, out var value) && value.Cost <= cost)
    {
        if (value.Cost < cost)
            return false;
        for (int i = 1; i < path.Count; i++)
            value.Path.Add(path[i]);
        return true;
    }
    paths[pos] = new(cost, new(path));
    return true;
}

int CountPathItems()
{
    HashSet<Vector> set = new() { end };
    Queue<Vector> queue2 = new(set);
    while (queue2.TryDequeue(out var item))
        if (paths.TryGetValue(item, out var path2))
            foreach (var pos2 in path2.Path)
                if (set.Add(pos2))
                    queue2.Enqueue(pos2);
    return set.Count;
}

record struct QueueItem(Vector Pos, Vector Vec = default, int Cost = 0);
record struct PathValue(int Cost, HashSet<Vector> Path);
