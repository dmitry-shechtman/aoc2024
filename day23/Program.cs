using aoc;

var input = File.ReadAllLines("input.txt");

var graph = BuildGraph(out var keys);

Console.WriteLine(Part1());
Console.WriteLine(Part2());

BitSet[] BuildGraph(out int[] keys)
{
    (keys, var values) = BuildIndex();

    var graph = new BitSet[keys.Length];
    for (int i = 0; i < graph.Length; i++)
        graph[i] = new(graph.Length);

    for (int i = 0; i < input.Length; i++)
    {
        var x = values[GetIndex(input[i])];
        var y = values[GetIndex(input[i].AsSpan()[3..])];
        graph[x][y] = graph[y][x] = true;
    }

    return graph;
}

(int[] keys, int[] values) BuildIndex()
{
    HashSet<int> keys = new(1024);
    int[] values = new int[0x400];

    for (int i = 0; i < input.Length; i++)
    {
        var a = GetIndex(input[i]);
        if (keys.Add(a))
            values[a] = keys.Count - 1;
    }

    return (keys.ToArray(), values);
}

int Part1()
{
    int count = 0;
    for (int a = 0; a < graph.Length; a++)
        for (int b = a + 1; b < graph.Length; b++)
            if (graph[a][b])
                for (int c = b + 1; c < graph.Length; c++)
                    count += graph[a][c] && graph[b][c] &&
                        (IsMatch(a) || IsMatch(b) || IsMatch(c)) ? 1 : 0;
    return count;
}

string Part2()
{
    var count = graph.Length;
    BitSet r = new(count);
    BitSet p = new(count, true);
    BitSet x = new(count);
    BitSet c = new(count);
    var clique = BronKerbosch(r, p, x, c);

    var names = new int[clique.CountSet()];
    int k = 0;
    foreach (var i in clique)
        names[k++] = keys[i];

    names.Sort();

    Span<char> chars = stackalloc char[names.Length * 3 - 1];
    for (int i = 0, j = 0; i < names.Length; i++)
    {
        if (j > 0)
            chars[j++] = ',';
        chars[j++] = (char)(names[i] >> 5 & 0x1F | 0x60);
        chars[j++] = (char)(names[i]      & 0x1F | 0x60);
    }

    return new(chars);
}

int GetIndex(ReadOnlySpan<char> key) =>
    (key[0] & 0x1F) << 5 | key[1] & 0x1F;

bool IsMatch(int index) =>
    (keys[index] & 0x3E0) == 0x280;

BitSet BronKerbosch(BitSet r, BitSet p, BitSet x, BitSet c)
{
    if (p.CountSet() == 0 && x.CountSet() == 0)
        return r.CountSet() > c.CountSet() ? r : c;
    int pivot = p.Clone().Or(x).FirstSet(out _);
    BitSet n, s, q, y;
    foreach (int i in p.Clone().AndNot(graph[pivot]))
    {
        n = graph[i];
        s = r.Clone().ThenSet(i, true);
        q = p.Clone().And(n);
        y = x.Clone().And(n);
        c = BronKerbosch(s, q, y, c);
        p[i] = false;
        x[i] = true;
    }
    return c;
}
