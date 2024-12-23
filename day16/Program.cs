using aoc;
using aoc.Grids;

const int Penalty = 1000;

var input = File.ReadAllText("input.txt").AsSpan().Trim();
var points = new Vector[2];
var walls = Grid.Parse(input, "SE", points);
var (start, end) = (points[0], points[1]);

Dictionary<Vector, int> dists =
    walls.ToDictionary(p => p, _ => 0);

Dictionary<Vector, PathValue> paths =
    walls.ToDictionary(p => p, _ => new PathValue());

Console.WriteLine(Part1());
Console.WriteLine(Part2());

int Part1()
{
    Vector pos, vec, vecA, vecB;
    int dist, dist1, dist2;


    PriorityQueue<Matrix, int> queue = new();
    queue.Enqueue(new(start, Vector.East), 0);

    while (queue.TryDequeue(out var item, out dist))
    {
        (pos, vec) = item;
        vecA = (vec.y, vec.x);
        vecB = -vecA;


        for (dist1 = dist + Penalty + 1;
            pos != end && (!dists.TryGetValue(pos, out dist2) || dist2 > 0);
            pos += vec, ++dist, ++dist1)
        {
            if (TryAdd(pos + vecA, dist1))
                queue.Enqueue(new(pos + vecA, vecA), dist1);

            if (TryAdd(pos + vecB, dist1))
                queue.Enqueue(new(pos + vecB, vecB), dist1);

        }


        TryAdd(pos, dist);
    }

    return dists[end];
}

bool TryAdd(Vector pos, int dist)
{
    if (dists.TryGetValue(pos, out var dist2) && dist2 <= dist)
        return false;
    dists[pos] = dist;
    return true;
}

int Part2()
{
    Vector pos, vec, vecA, vecB;
    int dist, dist2;
    PathValue value;
    List<Vector> path = new();
    Queue<QueueItem> queue = new();
    queue.Enqueue(new(start, Vector.East, 0));

    while (queue.TryDequeue(out var item))
    {
        (pos, vec, dist) = item;
        vecA = (vec.y, vec.x);
        vecB = -vecA;
        path.Clear();

        for (dist2 = dist + Penalty;
            pos != end && (!paths.TryGetValue(pos, out value) || value.Dist > 0);
            pos += vec, ++dist, ++dist2)
        {
            if (TryAdd2(pos, dist2, path, value))
            {
                queue.Enqueue(new(pos, vecA, dist2));
                queue.Enqueue(new(pos, vecB, dist2));
            }
            path.Add(pos);
        }

        if (!paths.TryGetValue(pos, out value))
            paths[pos] = new(dist, new(path));
    }

    return CountPathItems();
}

bool TryAdd2(Vector pos, int dist, List<Vector> path, PathValue value)
{
    if (value.Dist > 0 && value.Dist <= dist)
    {
        if (value.Dist < dist)
            return false;
        for (int i = 1; i < path.Count; i++)
            value.Path.Add(path[i]);
        return true;
    }
    paths[pos] = new(dist, new(path));
    return true;
}

int CountPathItems()
{
    HashSet<Vector> set = new() { end };
    Queue<Vector> queue = new(set);
    while (queue.TryDequeue(out var item))
        if (paths.TryGetValue(item, out var path2))
            foreach (var pos2 in path2.Path)
                if (set.Add(pos2))
                    queue.Enqueue(pos2);
    return set.Count;
}

record struct QueueItem(Vector Pos, Vector Vec, int Dist);
record struct PathValue(int Dist, HashSet<Vector> Path);
