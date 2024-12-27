using aoc;
using Matrix = aoc.DoubleMatrix;

Matrix Shift = Matrix.FromColumns(default, default, (10000000000000, 10000000000000));

var input = File.ReadAllText("input.txt").Trim();
var machines = Matrix.ParseColumnsAll(input, 3);

Console.WriteLine(Part1());
Console.WriteLine(Part2());

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
