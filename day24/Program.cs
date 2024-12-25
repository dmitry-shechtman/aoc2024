using System.Text.RegularExpressions;

Regex regex = new(@"^((?<k>[xy]\d\d): (?<v>0|1)\n)+(\n(?<a>[a-w]{3}|[xy]\d\d) (?<op>AND|OR|XOR) (?<b>[a-w]{3}|[xy]\d\d) -> (?<c>(z\d\d|[a-w]{3})))+$");

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
    ("AND", var a, var b) => GetValue(a) & GetValue(b),
    ("OR",  var a, var b) => GetValue(a) | GetValue(b),
    ("XOR", var a, var b) => GetValue(a) ^ GetValue(b),
    (var op, _, _) => long.Parse(op)
};
