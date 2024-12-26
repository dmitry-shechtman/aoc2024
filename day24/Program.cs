using System.Text.RegularExpressions;

const int COUNT = 64;
const int Z00 = COUNT * 2;

Regex regex = new(@"^((?<k>[xy]\d\d): (?<v>0|1)\n)+(\n(?<a>[a-w]{3}|[xy]\d\d) (?<op>AND|OR|XOR) (?<b>[a-w]{3}|[xy]\d\d) -> (?<c>(z\d\d|[a-w]{3})))+$");

var input = File.ReadAllText("input.txt").Trim();
var vals = regex.GetAllValues(input);

var gates = BuildCircuit(out var values);

Console.WriteLine(Part1());

long Part1() =>
    Enumerable.Range(Z00, COUNT)
        .Aggregate(0L, (a, i) => a | GetValue(i) << i);

Gate[] BuildCircuit(out int[] values)
{
    (var keys, values) = BuildIndex();

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
    foreach (var c in "xyz")
        for (int i = 0; i < COUNT; i++)
            keys.Add($"{c}{i:d02}");

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

enum Op { NIL, ONE, AND, OR, XOR }

record struct Gate(Op Op, int A, int B);
