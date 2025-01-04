using aoc;
using System.Text.RegularExpressions;

const int COUNT = 64;
const int Z00 = COUNT * 2;

Regex regex = new(@"^((?<k>[xy]\d\d): (?<v>0|1)\n)+(\n(?<a>[a-w]{3}|[xy]\d\d) (?<op>AND|OR|XOR) (?<b>[a-w]{3}|[xy]\d\d) -> (?<c>(z\d\d|[a-w]{3})))+$");

var input = File.ReadAllText("input.txt").Trim();
var vals = regex.GetAllValues(input);

var tuples = vals["c"]
    .Select((c, i) => (c, v: new Tuple(Enum.Parse<Op>(vals["op"][i]), vals["a"][i], vals["b"][i])))
    .ToDictionary(t => t.c, t => t.v);

var zCount = vals["c"].Count(k => k[0] == 'z');

Dictionary<string, Node> nodes;
Dictionary<Node, string> index;
var inputs = BuildInputs();
var acts = BuildActual();

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

Input[] BuildInputs()
{
    var inputs = new Input[Z00];
    for (int i = 0; i < inputs.Length; i++)
        inputs[i] = new(i);
    return inputs;
}

UInt128 BuildData()
{
    UInt128 data = 0;
    for (int i = 0; i < vals["k"].Length; i++)
    {
        var key = vals["k"][i];
        var value = UInt128.Parse(vals["v"][i]);
        var index = int.Parse(key[1..]) | (key[0] & 0x03) << 6;
        data |= value << index;
    }
    return data;
}

// Build adder circuit
Node[] BuildExpected()
{
    var exps = new Node[zCount];
    Node? x, y, carry = null;
    for (int i = 0; i < zCount - 1; i++)
    {
        (x, y) = (inputs[i], inputs[i + COUNT]);
        exps[i] = (x ^ y ^ carry)!;
        carry = x & y | (carry & (x ^ y));
    }
    exps[^1] = carry!;
    return exps;
}

// Build actual circuit
Gate[] BuildActual()
{
    nodes = new();
    index = new();
    var acts = new Gate[zCount];
    for (int i = 0; i < zCount; i++)
    {
        nodes.Add($"x{i:d02}", inputs[i]);
        nodes.Add($"y{i:d02}", inputs[i + COUNT]);
        acts[i] = CreateGate($"z{i:d02}", tuples);
    }
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
    for (int i = 0; i < zCount; i++)
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
abstract record Node
{
    public abstract long GetValue(UInt128 data);

    public static Node? operator &(Node? left, Node? right) =>
        Create(Op.AND, left, right);

    public static Node? operator |(Node? left, Node? right) =>
        Create(Op.OR, left, right);

    public static Node? operator ^(Node? left, Node? right) =>
        Create(Op.XOR, left, right);

    private static Node? Create(Op op, Node? left, Node? right) =>
        left is null
            ? null
            : right is null
                ? left
                : Gate.Create(op, left, right);
}

// Circuit input
sealed record Input(int Index) : Node
{
    public override long GetValue(UInt128 data) =>
        (long)((data & UInt128.One << Index) >> Index);
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

    public override long GetValue(UInt128 data) => Op switch
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
