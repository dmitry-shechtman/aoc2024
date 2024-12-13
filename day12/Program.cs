using aoc;
using aoc.Grids;

var s = File.ReadAllText("input.txt");
var m = MultiGrid.Parse(s);

Console.WriteLine(Solve1());
Console.WriteLine(Solve2());

int Solve1() => m[..^1].AsParallel()
    .Sum(g => new PerimeterWorker(g).Count());

int Solve2() => m[..^1].AsParallel()
    .Sum(g => new SideWorker(g).Count());

abstract record Worker(Grid Grid)
{
    readonly HashSet<Vector> visited = new();
    readonly Queue<Vector> queue = new();
    readonly List<Vector3D> border = new();

    public int Count() =>
        Grid.Where(visited.Add).Sum(Count);

    int Count(Vector p)
    {
        Vector r;
        var cnt = visited.Count;
        queue.Clear();
        queue.Enqueue(p);
        border.Clear();
        while (queue.TryDequeue(out var q))
            for (int i = 0; i < Grid.Headings.Length; i++)
                if (!Grid.Contains(r = q + Grid.Headings[i]))
                    border.Add((q, i));
                else if (visited.Add(r))
                    queue.Enqueue(r);
        return (visited.Count - cnt + 1) * Count(border);
    }

    protected abstract int Count(List<Vector3D> border);
}

record PerimeterWorker(Grid Grid) : Worker(Grid)
{
    protected override int Count(List<Vector3D> border) =>
        border.Count;
}

record SideWorker(Grid Grid) : Worker(Grid)
{
    protected override int Count(List<Vector3D> border) => border
        .GroupBy(p => (p.Z % 2 == 0 ? p.Y : p.X) << 2 | p.Z)
        .Select(g => g.Select(p => p.Z % 2 == 0 ? p.X : p.Y).Order())
        .Sum(c => c.Zip(c.Skip(1), (x, y) => y > x + 1).Count(v => v) + 1);
}
