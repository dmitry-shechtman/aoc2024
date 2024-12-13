using aoc;
using System.Text.RegularExpressions;

Regex Regex = new(@"^Button A: X\+(\d+), Y\+(\d+)\nButton B: X\+(\d+), Y\+(\d+)\nPrize: X=(\d+), Y=(\d+)$");

var input = File.ReadAllText("input.txt").Trim();

var machines = input.Split("\n\n")
    .Select(Regex.GetLongs)
    .Select(FromLongs)
    .ToArray();

Console.WriteLine(Part1());
Console.WriteLine(Part2());

(DoubleMatrix A, DoubleVector b) FromLongs(long[] v) =>
    ((v[0], v[2], v[1], v[3]), (v[4], v[5]));

long Part1() => machines
    .Sum(SolveOne);

long Part2() => machines
    .Select(m => (m.A, m.b + (10000000000000, 10000000000000)))
    .Sum(SolveOne);

long SolveOne((DoubleMatrix A, DoubleVector b) m)
{
    long a, b;
    return m.A.Solve(m.b, out var x) &&
        m.A.C1 * (a = (long)Math.Round(x.X)) +
        m.A.C2 * (b = (long)Math.Round(x.Y)) == m.b
            ? a * 3 + b
            : 0;
}
