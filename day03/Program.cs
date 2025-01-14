﻿var input = File.ReadAllText("input.txt");
Console.WriteLine(Part1());
Console.WriteLine(Part2());

int Part1() =>
    Solve(@"mul\((\d+),(\d+)\)");

int Part2() =>
    Solve(@"mul\((\d+),(\d+)\)|do\(\)|don't\(\)");

int Solve(string p) =>
    new Regex(p)
        .EnumerateValuesMany(input)
        .Aggregate((true, v: 0), Parse2).v;

int Parse(string[] ss) =>
    int.Parse(ss[1]) * int.Parse(ss[2]);

(bool, int) Parse2((bool b, int v) t, string[] ss) => ss[0] switch
{
    "do()" => (true, t.v),
    "don't()" => (false, t.v),
    _ => t.b ? (t.b, t.v + Parse(ss)) : t
};
