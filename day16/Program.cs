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
var width1 = size.width + 1;

var dists = new int[size.Length];
dists.Fill(int.MaxValue);
var paths = new PathValue[size.Length];
paths.Fill(new(int.MaxValue, new()));
Queue<QueueItem> queue = new();

Console.WriteLine(Part1());
Console.WriteLine(Part2());

int Part1()
{
    int vec, vecA, vecB;
    int pos, dist, dist1;
    queue.Enqueue(new(start, 1, 0));


    while (queue.TryDequeue(out var item))
    {
        (pos, vec, dist) = item;
        vecA = vec < 0 ? width1 + vec : width1 - vec;
        vecB = -vecA;

        for (dist1 = dist + Penalty + 1;
            !walls[pos] && pos != end;
            pos += vec, ++dist, ++dist1)
        {
            if (TryAdd(pos + vecA, dist1))
                queue.Enqueue(new(pos + vecA, vecA, dist1));

            if (TryAdd(pos + vecB, dist1))
                queue.Enqueue(new(pos + vecB, vecB, dist1));

        }

        TryAdd(pos, dist);
    }

    return dists[end];
}

bool TryAdd(int pos, int dist)
{
    if (dists[pos] <= dist)
        return false;
    dists[pos] = dist;
    return true;
}

int Part2()
{
    int vec, vecA, vecB;
    int pos, dist, dist2;
    queue.Enqueue(new(start, 1, 0));
    List<int> path = new();

    while (queue.TryDequeue(out var item))
    {
        (pos, vec, dist) = item;
        vecA = vec < 0 ? width1 + vec : width1 - vec;
        vecB = -vecA;
        path.Clear();
        for (dist2 = dist + Penalty;
            !walls[pos] && pos != end;
            pos += vec, ++dist, ++dist2)
        {
            if (TryAdd2(pos, dist2, path))
            {
                queue.Enqueue(new(pos, vecA, dist2));
                queue.Enqueue(new(pos, vecB, dist2));
            }
            path.Add(pos);
        }

        TryAdd2(pos, dist, path);
    }

    return CountPathItems();
}

bool TryAdd2(int pos, int dist, List<int> path)
{
    var (dist2, path2) = paths[pos];
    if (dist2 <= dist)
    {
        if (dist2 < dist)
            return false;
        for (int i = 1; i < path.Count; i++)
            path2.Add(path[i]);
        return true;
    }
    paths[pos] = new(dist, new(path));
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

record struct QueueItem(int Pos, int Vec, int Dist);
record struct PathValue(int Dist, HashSet<int> Path);
