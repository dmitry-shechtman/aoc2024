﻿using System.Text.RegularExpressions;

using ANF = System.Collections.Generic.SortedSet<System.UInt128>;

const int COUNT = 64;
const int Z00 = COUNT * 2;

Regex regex = new(@"^((?<k>[xy]\d\d): (?<v>0|1)\n)+(\n(?<a>[a-w]{3}|[xy]\d\d) (?<op>AND|OR|XOR) (?<b>[a-w]{3}|[xy]\d\d) -> (?<c>(z\d\d|[a-w]{3})))+$");

var input = File.ReadAllText("input.txt").Trim();
var vals = regex.GetAllValues(input);

var gates = BuildCircuit(out var keys, out var values);

ANF[] anfs, exps;
SortedSet<string> swap = new();

Console.WriteLine(Part1());
Console.WriteLine(Part2());

long Part1() =>
    Enumerable.Range(Z00, COUNT)
        .Aggregate(0L, (a, i) => a | GetValue(i) << i);

string Part2()
{
    BuildExpected();
    do
        BuildActual();
    while (TrySwapAny());
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
    for (int i = 0; i < COUNT; i++)
        foreach (var c in "xy")
            keys.Add($"{c}{i:d02}");

    for (int i = 0; i < COUNT; i++)
        keys.Add($"z{i:d02}");

    foreach (var key in vals["c"])
        if (key[0] != 'z')
            keys.Add(key);

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

    for (var (i, m) = (Z00, UInt128.One); gates[i] != default; i++, m <<= 2)
    {
        ANF exp = new() { m, m << 1 };
        if (gates[i + 1] == default) // Last bit
            exp.Clear(); // No x_i or y_i
        if (i > Z00) // Skip bit 0
            exp.Add(m >> 1 | m >> 2); // Carry
        exps[i - Z00] = exp;
    }
}

// Build ANFs for actual circuit
void BuildActual()
{
    anfs = new ANF[gates.Length];

    // Initialize input variables
    for (var (i, m) = (0, UInt128.One); i < Z00; i++, m <<= 1)
        anfs[i] = new() { m }; // x_i maps to 2^2i

    // Add gates to the array
    for (int i = Z00; i < gates.Length; i++)
        anfs[i] = gates[i] == default ? new() : GetANF(i);
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
ANF Multiply(ANF left, ANF right)
{
    ANF result = new();
    foreach (var key1 in left)
        foreach (var key2 in right)
            if (!result.Remove(key1 ^ key2))
                result.Add(key1 ^ key2);
    return result;
}

// Add two ANFs (XOR operation)
ANF Add(ANF left, ANF right)
{
    ANF result = new(left);
    foreach (var key in right)
        if (!result.Remove(key))
            result.Add(key);
    return result;
}

// Look for an invalid output; try swapping if found
bool TrySwapAny()
{
    for (int i = Z00; anfs[i].Count > 0; i++)
    {
        var exp = exps[i - Z00];
        if (!exp.SequenceEqual(anfs[i]))
            return TrySwap(exp, i);
    }
    return false;
}

// Recursively try swapping
bool TrySwap(ANF anf, int i)
{
    if (DoTrySwap(anf, i))
        return true;
    var (op, a, b) = gates[i];
    return op == Op.XOR &&
        (TrySwap(Add(anf, anfs[a]), b) ||
        TrySwap(Add(anf, anfs[b]), a));
}

// Search an equivalent ANF; swap if found
bool DoTrySwap(ANF anf, int i)
{
    if (!TryFind(anf, out var j))
        return false;
    (gates[i], gates[j]) = (gates[j], gates[i]);
    swap.Add(keys[i]);
    swap.Add(keys[j]);
    return true;
}

// Search an equivalent ANF
bool TryFind(ANF anf, out int i)
{
    for (i = Z00; i < gates.Length; i++)
        if (anf.SequenceEqual(anfs[i]))
            return true;
    return false;
}

enum Op { NIL, ONE, AND, OR, XOR }

record struct Gate(Op Op, int A, int B);
