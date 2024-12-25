using System.Text.RegularExpressions;

Regex regex = new(@"^((?<k>[xy]\d\d): (?<v>0|1)\n)+(\n(?<a>[a-w][a-w\d]{2}|[xy]\d\d) (?<op>AND|OR|XOR) (?<b>[a-w][a-w\d]{2}|[xy]\d\d) -> (?<c>(z\d\d|[a-w][a-w\d]{2})))+$");

var input = File.ReadAllText("input.txt").Trim();
var vals = regex.GetAllValues(input);

var gates = vals["c"].Select((k, i) =>
        (k, v: (vals["op"][i], vals["a"][i], vals["b"][i])))
    .Concat(vals["k"].Select((k, i) =>
        (k, v: (vals["v"][i], "", ""))))
    .ToDictionary(t => t.k, t => t.v);

Console.WriteLine(Part1());

long Part1() =>
    gates.Keys.Where(t => t[0] == 'z')
        .OrderDescending()
        .Aggregate(0L, (a, k) => a << 1 | GetValue(k));

long GetValue(string key) => gates[key] switch
{
    ("0", _, _) => 0,
    ("1", _, _) => 1,
    ("AND", var a, var b) => GetValue(a) & GetValue(b),
    ("OR",  var a, var b) => GetValue(a) | GetValue(b),
    ("XOR", var a, var b) => GetValue(a) ^ GetValue(b),
    _ => throw new()
};
