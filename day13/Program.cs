using Matrix = aoc.DoubleMatrix;
using System.Text.RegularExpressions;

Regex Regex = new(@"^Button A: X\+(\d+), Y\+(\d+)\nButton B: X\+(\d+), Y\+(\d+)\nPrize: X=(\d+), Y=(\d+)$");

Matrix Shift = Matrix.FromColumns(default, default, (10000000000000, 10000000000000));

var input = File.ReadAllText("input.txt").Trim();

var machines = input.Split("\n\n")
    .Select(Regex.GetLongs)
    .Select(FromLongs)
    .ToArray();

Console.WriteLine(Part1());
Console.WriteLine(Part2());

Matrix FromLongs(long[] v) =>
    Matrix.FromColumns((v[0], v[1]), (v[2], v[3]), (v[4], v[5]));

long Part1() => machines
    .Sum(SolveOne);

long Part2() => machines
    .Select(m => m + Shift)
    .Sum(SolveOne);

long SolveOne(Matrix m)
{
    long a, b;
    return m.Solve(out var x) &&
        (a = (long)Math.Round(x.X)) >= 0 &&
        (b = (long)Math.Round(x.Y)) >= 0 &&
        m.C1 * a + m.C2 * b == m.C3
            ? a * 3 + b
            : 0;
}
