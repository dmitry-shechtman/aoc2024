using System.Text.RegularExpressions;

using ANF = System.Collections.Generic.SortedSet<aoc.BitSet>;

const int COUNT = 64;
const int Z00 = COUNT * 2;

Regex regex = new(@"^((?<k>[xy]\d\d): (?<v>0|1)\n)+(\n(?<a>[a-w]{3}|[xy]\d\d) (?<op>AND|OR|XOR) (?<b>[a-w]{3}|[xy]\d\d) -> (?<c>(z\d\d|[a-w]{3})))+$");

var input = File.ReadAllText("input.txt").Trim();
var vals = regex.GetAllValues(input);

var zCount = 0;
var gates = BuildCircuit(out var keys, out var values);

ANF[] anfs, exps;
SortedSet<string> swap = new();

Console.WriteLine(Part1());
Console.WriteLine(Part2());

long Part1() =>
    Enumerable.Range(Z00, zCount)
        .Aggregate(0L, (a, i) => a | GetValue(i) << i);

string Part2()
{
    BuildExpected();
    do
        BuildActual();
    while (TrySwapAny() == true);
    return string.Join(',', swap);
}

Gate[] BuildCircuit(out string[] keys, out int[] values)
{
    (keys, values) = BuildIndex();

    var gates = new Gate[keys.Length];

    for (int i = 0; i < vals["c"].Length; i++)
        gates[Get("c", i)] = new(ParseOp(vals["op"][i]), Get("a", i), Get("b", i));

    for (int i = 0; i < vals["k"].Length; i++)
        gates[Get("k", i)] = new((Op)(vals["v"][i][0] - '0'), 0, 0);

    return gates;
}

(string[] keys, int[] values) BuildIndex()
{
    List<string> keys = new();
    foreach (var c in "xy")
        for (int i = 0; i < COUNT; i++)
            keys.Add($"{c}{i:d02}");

    foreach (var key in vals["c"])
        if (key[0] != 'z')
            keys.Add(key);
        else
            keys.Insert(Z00 + zCount, $"z{zCount++:d02}");

    values = new int[0x8000];
    for (int i = 0; i < keys.Count; i++)
        values[GetIndex(keys[i])] = i;

    return (keys.ToArray(), values);
}

int Get(string key, int i) =>
    values[GetIndex(vals[key][i])];

int GetIndex(string key) =>
    (key[0] & 0x1F) << 10 | (key[1] & 0x1F) << 5 | key[2] & 0x1F;

long GetValue(int index) => gates[index] switch
{
    (Op.AND, var a, var b) => GetValue(a) & GetValue(b),
    (Op.OR,  var a, var b) => GetValue(a) | GetValue(b),
    (Op.XOR, var a, var b) => GetValue(a) ^ GetValue(b),
    (var op, _, _) => (long)op
};

Op ParseOp(string op) => op switch
{
    "AND" => Op.AND,
    "OR"  => Op.OR,
    "XOR" => Op.XOR,
    _ => throw new NotImplementedException()
};

// Build ANFs for adder outputs 
void BuildExpected()
{
    exps = new ANF[COUNT];

    for (var (i, m) = (0, 1L); i < zCount; i++, m <<= 1)
    {
        ANF exp = new();
        if (i < zCount - 1) // Skip last bit
        {
            exp.Add(new(m, 0L)); // x_i maps to 2^i
            exp.Add(new(0L, m)); // y_i maps to 2^i+64
        }
        if (i > 0) // Skip bit 0
            exp.Add(new(m >> 1, m >> 1)); // Carry
        exps[i] = exp;
    }
}

// Build ANFs for actual circuit
void BuildActual()
{
    anfs = new ANF[gates.Length];

    // Initialize input variables
    for (var (i, m) = (0, 1L); i < COUNT; i++, m <<= 1)
    {
        anfs[i]         = new() { new(m, 0L) };
        anfs[i + COUNT] = new() { new(0L, m) };
    }

    // Add gates to the array
    for (int i = Z00; i < gates.Length; i++)
        anfs[i] = GetANF(i);
}

// Retrieve an ANF or create one if not found
ANF GetANF(int i) =>
    anfs[i] ?? (anfs[i] = DoGetANF(i));

// Recursively build an ANF
ANF DoGetANF(int i)
{
    var (Op, A, B) = gates[i];
    var a = GetANF(A);
    var b = GetANF(B);
    return Op switch
    {
        Op.AND => Multiply(a, b),
        Op.OR  => Add(Add(a, b), Multiply(a, b)),
        Op.XOR => Add(a, b),
        _ => throw new NotImplementedException(),
    };
}

// Multiply two ANFs (AND operation)
ANF Multiply(ANF anf1, ANF anf2)
{
    ANF result = new();
    foreach (var term1 in anf1)
    {
        foreach (var term2 in anf2)
        {
            var term = term1.Clone().Xor(term2);
            if (!result.Remove(term))
                result.Add(term);
        }
    }
    return result;
}

// Add two ANFs (XOR operation)
ANF Add(ANF anf1, ANF anf2)
{
    ANF result = new(anf1);
    foreach (var term in anf2)
        if (!result.Remove(term))
            result.Add(term);
    return result;
}

// Look for an invalid output; try swapping if found
bool? TrySwapAny()
{
    for (int i = 0; i < zCount; i++)
    {
        var exp = exps[i];
        if (!exp.SequenceEqual(anfs[i + Z00]))
            return TrySwap(exp, i + Z00);
    }
    return false;
}

// Recursively try swapping
bool? TrySwap(ANF anf, int i)
{
    if (DoTrySwap(anf, i))
        return true;
    var (op, a, b) = gates[i];
    return op == Op.XOR
        ? TrySwap(Add(anf, anfs[a]), b) == true ||
          TrySwap(Add(anf, anfs[b]), a) == true
        : null;
}

// Look for an equivalent ANF; swap if found
bool DoTrySwap(ANF anf, int i)
{
    if (!TryFind(anf, out var j))
        return false;
    (gates[i], gates[j]) = (gates[j], gates[i]);
    swap.Add(keys[i]);
    swap.Add(keys[j]);
    return true;
}

// Look for an equivalent ANF
bool TryFind(ANF anf, out int i)
{
    for (i = Z00; i < gates.Length; i++)
        if (anf.SequenceEqual(anfs[i]))
            return true;
    return false;
}

enum Op { NIL, ONE, AND, OR, XOR }

record struct Gate(Op Op, int A, int B);
