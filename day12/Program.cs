using aoc;
using aoc.Grids;

var s = File.ReadAllText("input.txt");
var t = s.Where(char.IsLetter);
var m = MultiGrid.Parse(s, t.ToArray(), out var z);

Console.WriteLine(Solve1());
Console.WriteLine(Solve2());

int Solve1() => m[..^1].AsParallel()
    .Sum(g => new PerimeterWorker(g, z).Count());

int Solve2() => m[..^1].AsParallel()
    .Sum(g => new SideWorker(g, z).Count());

abstract record Worker(Grid Grid, Size Size)
{
    readonly HashSet<Vector> visited = new();
    readonly Queue<Vector> queue = new();
    readonly List<Dictionary<int, bool[]>> heads = Grid.Headings
        .Select(_ => new Dictionary<int, bool[]>())
        .ToList();

    public int Count() =>
        Grid.Where(visited.Add).Sum(Count);

    int Count(Vector p)
    {
        Vector r;
        var cnt = visited.Count;
        queue.Clear();
        queue.Enqueue(p);
        heads.ForEach(p => p.Clear());
        while (queue.TryDequeue(out var q))
            for (int i = 0; i < heads.Count; i++)
                if (!Grid.Contains(r = q + Grid.Headings[i]))
                    Set(q, i);
                else if (visited.Add(r))
                    queue.Enqueue(r);
        return (visited.Count - cnt + 1) *
            heads.SelectMany(v => v.Values).Sum(Count);
    }

    void Set(Vector q, int i)
    {
        var key   = i % 2 == 0 ? q.Y : q.X;
        var value = i % 2 == 0 ? q.X : q.Y;
        if (!heads[i].TryGetValue(key, out var coords))
            heads[i].Add(key, coords = new bool[i % 2 == 0 ? Size.Height : Size.Width]);
        coords[value] = true;
    }

    protected abstract int Count(bool[] coords);
}

record PerimeterWorker(Grid Grid, Size Size) : Worker(Grid, Size)
{
    protected override int Count(bool[] coords) =>
        coords.Count(v => v);
}

record SideWorker(Grid Grid, Size Size) : Worker(Grid, Size)
{
    protected override int Count(bool[] coords) =>
        (coords.Zip(coords[1..], (a, b) => a ^ b).Count(v => v) + 1) / 2;
}
