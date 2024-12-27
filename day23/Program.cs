using aoc;

var input = File.ReadAllLines("input.txt");

int[] keys = new int[1024];

var graph = BuildGraph(out int min, out int max);

Console.WriteLine(Part1());
Console.WriteLine(Part2());

BitSet[] BuildGraph(out int min, out int max)
{
    var values = BuildIndex(out min, out max);

    var graph = new BitSet[values.Count];
    for (int i = 0; i < graph.Length; i++)
        graph[i] = new(graph.Length);

    foreach (string s in input)
    {
        var x = values[s[..2]];
        var y = values[s[3..]];
        graph[x][y] = graph[y][x] = true;
    }

    return graph;
}

Dictionary<string, int> BuildIndex(out int min, out int max)
{
    BitSet set = new(1024);
    foreach (string s in input)
        set[GetIndex(s)] = true;

    Dictionary<string, int> values = new(1024);

    (min, max) = (0, 0);
    for (int i = set.FirstSet(), j = 0; i >= 0; i = set.NextSet(i))
    {
        if ((i & 0x3E0) == 0x280)
            (min, max) = (min == 0 ? j : min, j);
        var key = $"{(char)(i >> 5 | 0x60)}{(char)(i & 0x1F | 0x60)}";
        (keys[j], values[key]) = (i, j++);
    }

    return values;
}

int Part1()
{
    int count = 0;
    BitSet setA, setAB;
    for (int a = 0; a < graph.Length; a++)
    {
        setA = graph[a];
        for (int b = setA.NextSet(a); b >= 0; b = setA.NextSet(b))
        {
            setAB = setA.Clone().And(graph[b]);
            for (int c = setAB.NextSet(b); c >= 0; c = setAB.NextSet(c))
            {
                count += IsMatch(a) || IsMatch(b) || IsMatch(c) ? 1 : 0;
            }
        }
    }
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

    Span<char> chars = stackalloc char[clique.CountSet() * 3 - 1];
    int j = 0;
    foreach (var i in clique)
    {
        if (j > 0)
            chars[j++] = ',';
        chars[j++] = (char)(keys[i] >> 5 & 0x1F | 0x60);
        chars[j++] = (char)(keys[i]      & 0x1F | 0x60);
    }

    return new(chars);
}

int GetIndex(ReadOnlySpan<char> key) =>
    (key[0] & 0x1F) << 5 | key[1] & 0x1F;

bool IsMatch(int index) =>
    index >= min && index <= max;

BitSet BronKerbosch(BitSet r, BitSet p, BitSet x, BitSet c)
{
    if (!p.AnySet() && !x.AnySet())
        return r.CountSet() > c.CountSet() ? r : c;
    int pivot = p.Clone().Or(x).FirstSet();
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
