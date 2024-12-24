using System.Text.RegularExpressions;

Regex regex = new(@"^((?<k>[xy]\d\d): (?<v>0|1)\n)+(\n(?<a>[a-w][a-w\d]{2}|[xy]\d\d) (?<op>AND|OR|XOR) (?<b>[a-w][a-w\d]{2}|[xy]\d\d) -> (?<c>(z\d\d|[a-w][a-w\d]{2})))+$");

var input = File.ReadAllText("input.txt").Trim();
var vals = regex.GetAllValues(input);

var gates = vals["c"].Select((k, i) =>
        (k, v: new Gate(ParseOp(vals["op"][i]), vals["a"][i], vals["b"][i])))
    .Concat(vals["k"].Select((k, i) =>
        (k, v: new Gate((Op)(vals["v"][i][0] - '0'), "", ""))))
    .ToDictionary(t => t.k, t => t.v);

Console.WriteLine(Part1());

long Part1() =>
    Enumerable.Range(0, 64).Reverse().Select(i => $"z{i:d02}")
        .Where(gates.ContainsKey)
        .Aggregate(0L, (a, k) => a << 1 | GetValue(k));

long GetValue(string key) => gates[key] switch
{
    (Op.NIL, _, _) => 0,
    (Op.ONE, _, _) => 1,
    (Op.AND, var a, var b) => GetValue(a) & GetValue(b),
    (Op.OR,  var a, var b) => GetValue(a) | GetValue(b),
    (Op.XOR, var a, var b) => GetValue(a) ^ GetValue(b),
    _ => throw new()
};

Op ParseOp(string op) => op switch
{
    "AND" => Op.AND,
    "OR"  => Op.OR,
    "XOR" => Op.XOR,
    _ => throw new NotImplementedException()
};

enum Op { NIL, ONE, AND, OR, XOR }

record struct Gate(Op Op, string A, string B);
