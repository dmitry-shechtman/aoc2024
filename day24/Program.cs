using aoc;
using System.Text.RegularExpressions;

Regex regex = new(@"^((?<k>[xy]\d\d): (?<v>0|1)\n)+(\n(?<a>[a-w]{3}|[xy]\d\d) (?<op>AND|OR|XOR) (?<b>[a-w]{3}|[xy]\d\d) -> (?<c>(z\d\d|[a-w]{3})))+$");

var input = File.ReadAllText("input.txt").Trim();
var vals = regex.GetAllValues(input, ^6..);

var cnts = new int[3];
var kCount = BuildCount("k");
var cCount = BuildCount("c");
var nodes  = BuildNodes(out var index);
var tuples = BuildTuples(out var keys, out var outputs);
var acts   = BuildActual();

Console.WriteLine(Part1());
Console.WriteLine(Part2());

long Part1()
{
    var data = BuildData();
    return acts.Aggregate(0L, (a, v, i) => a | v.GetValue(data) << i);
}

string Part2()
{
    var exps = BuildExpected();
    SortedSet<string> swap = new();
    while (TrySwapAny(exps, acts, swap))
        acts = BuildActual();
    return string.Join(',', swap);
}

int BuildCount(string k)
{
    foreach (var key in vals[k])
        if ((key[0] & 0x1C) == 0x18)
            ++cnts[key[0] & 0x03];
    return vals[k].Length;
}

Node[] BuildNodes(out Dictionary<Node, int> index)
{
    var nodes = new Node[kCount + cCount];
    for (int i = 0; i < kCount; i++)
        nodes[i] = new Input(i);
    index = new(cCount);
    return nodes;
}

Tuple[] BuildTuples(out string[] keys, out int[] outputs)
{
    keys = new string[kCount + cCount];
    Dictionary<string, int> values = new(kCount + cCount);
    foreach (var k in "xy")
        for (int i = 0; i < cnts[k & 0x03]; i++)
            values.Add($"{k}{i:d02}", i << 1 | k & 0x01);

    for (int i = 0; i < cCount; i++)
        values.Add(keys[i + kCount] = vals["c"][i], i + kCount);

    var tuples = new Tuple[kCount + cCount];
    outputs = new int[cnts[^1]];
    for (int i = 0; i < cCount; i++)
    {
        tuples[i + kCount] = new(ParseOp(vals["op"][i]), values[vals["a"][i]], values[vals["b"][i]]);
        if (keys[i + kCount][0] == 'z')
            outputs[ParseKey(keys[i + kCount])] = i + kCount;
    }

    return tuples;
}

UInt128 BuildData()
{
    UInt128 data = 0;
    for (int i = 0; i < vals["k"].Length; i++)
    {
        var key = vals["k"][i];
        var value = UInt128.Parse(vals["v"][i]);
        var index = ParseKey(key) << 1 | key[0] & 0x01;
        data |= value << index;
    }
    return data;
}

Op ParseOp(string op) => op switch
{
    "AND" => Op.AND,
    "OR"  => Op.OR,
    "XOR" => Op.XOR,
    _ => throw new NotImplementedException()
};

int ParseKey(string key) =>
    (key[1] & 0x0F) * 10 + (key[2] & 0x0F);

// Build adder circuit
Node[] BuildExpected()
{
    var exps = new Node[cnts[^1]];
    Node carry = Node.Zero, x, y, xor;
    for (int i = 0; i < exps.Length; i++)
    {
        (x, y) = (nodes[i << 1], nodes[i << 1 | 1]);
        xor = i < exps.Length - 1 ? x ^ y : Node.Zero;
        exps[i] = xor ^ carry;
        carry = x & y | xor & carry;
    }
    return exps;
}

// Build actual circuit
Node[] BuildActual()
{
    nodes.Clear(kCount, cCount);
    index.Clear();
    var acts = new Node[cnts[^1]];
    for (int i = 0; i < acts.Length; i++)
        acts[i] = GetNode(outputs[i]);
    for (int i = kCount; i < nodes.Length; i++)
        index.Add(nodes[i], i);
    return acts;
}

Node GetNode(int i) =>
    nodes[i] ??= CreateGate(tuples[i]);

Gate CreateGate(Tuple t) =>
    Gate.Create(t.Op, GetNode(t.A), GetNode(t.B));

// Look for an invalid output; try swapping if found
bool TrySwapAny(Node[] exps, Node[] acts, SortedSet<string> swap)
{
    for (int i = 0; i < exps.Length; i++)
        if (!exps[i].Equals(acts[i]))
            return TrySwap(exps[i], acts[i], swap);
    return false;
}

// Recursively try swapping
bool TrySwap(Node exp, Node act, SortedSet<string> swap)
{
    if (DoTrySwap(exp, act, swap))
        return true;
    if (!(exp is Gate gExp && act is Gate gAct && gExp.Op == gAct.Op))
        throw new();
    return gExp.A.Equals(gAct.A) && TrySwap(gExp.B, gAct.B, swap)
        || gExp.B.Equals(gAct.B) && TrySwap(gExp.A, gAct.A, swap);
}

// Look for an equivalent gate; swap if found
bool DoTrySwap(Node exp, Node act, SortedSet<string> swap)
{
    if (!index.TryGetValue(exp, out var i))
        return false;
    var j = index[act];
    (tuples[i], tuples[j]) = (tuples[j], tuples[i]);
    swap.Add(keys[i]);
    swap.Add(keys[j]);
    return true;
}

// Circuit node
record Node
{
    public static Node Zero { get; } = new();

    public override int GetHashCode() => -1;

    public virtual long GetValue(UInt128 data) => 0;

    public static Node operator &(Node left, Node right) =>
        !ReferenceEquals(Zero, left) && !ReferenceEquals(Zero, right)
            ? Gate.Create(Op.AND, left, right)
            : Zero;

    public static Node operator |(Node left, Node right) =>
        Create(Op.OR, left, right);

    public static Node operator ^(Node left, Node right) =>
        Create(Op.XOR, left, right);

    private static Node Create(Op op, Node left, Node right) =>
        !ReferenceEquals(Zero, left)
            ? !ReferenceEquals(Zero, right)
                ? Gate.Create(op, left, right)
                : left
            : right;
}

// Circuit input
sealed record Input(int Index) : Node
{
    public override int GetHashCode() => Index;

    public override long GetValue(UInt128 data) =>
        (long)((data & UInt128.One << Index) >> Index);
}

// Circuit gate
sealed record Gate(Op Op, Node A, Node B) : Node
{
    public static Gate Create(Op op, Node left, Node right)
    {
        var (i, j) = (left.GetHashCode(), right.GetHashCode());
        var (a, b, h) = i < j
            ? (left, right, j)
            : (right, left, i);
        return new(op, a, b)
        {
            HashCode = (h + 0x100) | (int)op << 24
        };
    }

    public override int GetHashCode() => HashCode;

    private int HashCode { get; init; }

    public override long GetValue(UInt128 data) => Op switch
    {
        Op.AND => A.GetValue(data) & B.GetValue(data),
        Op.OR  => A.GetValue(data) | B.GetValue(data),
        Op.XOR => A.GetValue(data) ^ B.GetValue(data),
        _ => throw new NotImplementedException(),
    };
}

// Gate tuple
record struct Tuple(Op Op, int A, int B);

// Operator
enum Op { AND, OR, XOR }
