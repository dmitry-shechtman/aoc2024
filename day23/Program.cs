var input = File.ReadAllLines("input.txt");

Dictionary<int, HashSet<int>> graph = new(1024);

BuildGraph();
Console.WriteLine(Part1());
Console.WriteLine(Part2());

void BuildGraph()
{
    foreach (var s in input)
    {
        var a = (s[0] & 0x1F) << 5 | s[1] & 0x1F;
        var b = (s[3] & 0x1F) << 5 | s[4] & 0x1F;
        graph.GetOrAdd(a, _ => new()).Add(b);
        graph.GetOrAdd(b, _ => new()).Add(a);
    }
}

int Part1() => graph.SelectMany(t =>
    t.Value.SelectMany(b =>
        graph[b].Intersect(t.Value).Select(c =>
            new[] { t.Key, b, c })
                .Where(abc => abc.Any(v => (v >> 5 & 0x1F) == 0x14))
                .Select(abc => abc.Order().Aggregate((a, v) => a << 10 | v))))
    .Distinct().Count();

string Part2()
{
    var clique = BronKerbosch(new(), new(graph.Keys), new(), new());
    return string.Join(',', clique.Order().Select(v =>
        $"{(char)(v >> 5 | 0x60)}{(char)(v & 0x1F | 0x60)}"));
}

List<int> BronKerbosch(List<int> r, HashSet<int> p, HashSet<int> x, List<int> c)
{
    List<int> s;
    HashSet<int> n, q, y;
    if (!p.Any() && !x.Any())
        return r.Count > c.Count ? r : c;
    var pivot = p.Concat(x).First();
    foreach (var node in p.Except(graph[pivot]))
    {
        s = new(r) { node };
        n = graph[node];
        q = new(p.Intersect(n));
        p.Remove(node);
        y = new(x.Intersect(n));
        x.Add(node);
        c = BronKerbosch(s, q, y, c);
    }
    return c;
}
