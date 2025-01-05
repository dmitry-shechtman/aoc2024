using aoc;
using System.Numerics;
using System.Text.RegularExpressions;

Regex regex = new(@"^((?<k>[xy]\d\d): (?<v>0|1)\n)+(\n(?<a>[a-w]{3}|[xy]\d\d) (?<op>AND|OR|XOR) (?<b>[a-w]{3}|[xy]\d\d) -> (?<c>(z\d\d|[a-w]{3})))+$");

var input = File.ReadAllText("input.txt").Trim();
var vals = regex.GetAllValues(input);

var tuples = vals["c"]
    .Select((c, i) => (c, v: new Tuple(Enum.Parse<Op>(vals["op"][i]), vals["a"][i], vals["b"][i])))
    .ToDictionary(t => t.c, t => t.v);

Dictionary<string, Node> nodes;
Dictionary<Node, string> index;
var cnts = BuildCounts();
var acts = BuildActual();

Console.WriteLine(Part1());
Console.WriteLine(Part2());

BigInteger Part1()
{
    var data = BuildData();
    return acts.Aggregate(BigInteger.Zero,
        (a, v, i) => a | v.GetValue(data) << i);
}

string Part2()
{
    var exps = BuildExpected();
    SortedSet<string> swap = new();
    while (TrySwapAny(exps, acts, swap))
        acts = BuildActual();
    return string.Join(',', swap);
}

int[] BuildCounts()
{
    var cnts = new int[3];
    foreach (var k in "abc")
        foreach (var key in vals[$"{k}"])
            if (int.TryParse(key[1..], out var v))
                cnts[key[0] & 0x03] = Math.Max(cnts[key[0] & 0x03], v + 1);
    return cnts;
}

BigInteger BuildData()
{
    BigInteger data = 0;
    for (int i = 0; i < vals["k"].Length; i++)
    {
        var key = vals["k"][i];
        var value = BigInteger.Parse(vals["v"][i]);
        var index = int.Parse(key[1..]) << 1 | key[0] & 0x01;
        data |= value << index;
    }
    return data;
}

// Build adder circuit
Node[] BuildExpected()
{
    var exps = new Node[cnts[^1]];
    Node carry = Node.Zero, xor;
    for (int i = 0; i < exps.Length; i++)
    {
        Input x = new(i << 1), y = new(i << 1 | 1);
        xor = i < exps.Length - 1 ? x ^ y : Node.Zero;
        exps[i] = xor ^ carry;
        carry = x & y | xor & carry;
    }
    return exps;
}

// Build actual circuit
Gate[] BuildActual()
{
    nodes = new();
    index = new();
    for (int k = 0; k < 2; k++)
        for (int i = 0; i < cnts[k]; i++)
            nodes.Add($"{"xy"[k]}{i:d02}", new Input(i << 1 | k));
    var acts = new Gate[cnts[^1]];
    for (int i = 0; i < acts.Length; i++)
        acts[i] = CreateGate($"z{i:d02}", tuples);
    return acts;
}

Node GetNode(string key)
{
    if (!nodes.TryGetValue(key, out var value))
        value = CreateGate(key, tuples);
    return value;
}

Gate CreateGate(string key, Dictionary<string, Tuple> tuples)
{
    var value = DoCreateGate(tuples[key]);
    nodes.Add(key, value);
    index.Add(value, key);
    return value;
}

Gate DoCreateGate(Tuple t) =>
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
    if (!index.TryGetValue(exp, out var i) || !index.TryGetValue(act, out var j))
        return false;
    (tuples[i], tuples[j]) = (tuples[j], tuples[i]);
    swap.Add(i);
    swap.Add(j);
    return true;
}

// Circuit node
record Node
{
    public static Node Zero { get; } = new();

    public virtual BigInteger GetValue(BigInteger data) => 0;

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
    public override BigInteger GetValue(BigInteger data) =>
        (data & BigInteger.One << Index) >> Index;
}

// Circuit gate
sealed record Gate(Op Op, Node A, Node B) : Node
{
    public static Gate Create(Op op, Node left, Node right)
    {
        var (a, b) = left.GetHashCode() < right.GetHashCode()
            ? (left, right)
            : (right, left);
        return new(op, a, b);
    }

    public override BigInteger GetValue(BigInteger data) => Op switch
    {
        Op.AND => A.GetValue(data) & B.GetValue(data),
        Op.OR  => A.GetValue(data) | B.GetValue(data),
        Op.XOR => A.GetValue(data) ^ B.GetValue(data),
        _ => throw new NotImplementedException(),
    };
}

// Gate tuple
record struct Tuple(Op Op, string A, string B);

// Operator
enum Op { AND, OR, XOR }
