using aoc;
using System.Text.RegularExpressions;

Regex regex = new(@"^p=(?<m11>\d+),(?<m12>\d+) v=(?<m21>-?\d+),(?<m22>-?\d+)$");

const int W = 101, H = 103, RUN = 10;
Vector size = new(W, H);
Vector half = size / 2;

var robots = File.ReadAllLines("input.txt")
    .Select(s => Matrix.Parse(regex, s))
    .ToArray();

Console.WriteLine(Part1());
Console.WriteLine(Part2());

int Part1() => robots
    .Select(Step100)
    .Select(GetQuadrant)
    .Sum()
    .Product();

Vector Step100(Matrix m)
{
    var (p, v) = m;
    var (x, y) = (p + v * 100) % size;
    return new(x < 0 ? x + W : x, y < 0 ? y + H : y);
}

Vector4D GetQuadrant(Vector p) => p switch
{
    _ when p.x < half.x && p.y < half.y => (1, 0, 0, 0),
    _ when p.x > half.x && p.y < half.y => (0, 1, 0, 0),
    _ when p.x < half.x && p.y > half.y => (0, 0, 1, 0),
    _ when p.x > half.x && p.y > half.y => (0, 0, 0, 1),
    _ => default
};

int Part2()
{
    Span<Vector4D> qq = stackalloc Vector4D[robots.Length];
    int[,] counts = new int[W, H + 1];
    int step, total, run, x, y, z, w, i;
    for (i = 0; i < robots.Length; ++i)
    {
        qq[i] = (robots[i].m11, robots[i].m12, robots[i].m21, robots[i].m22);
        ++counts[qq[i].x, qq[i].y];
    }

    for (step = 0, total = 0; total < RUN; ++step)
    {
        for (i = 0; i < robots.Length; ++i)
        {
            (x, y, z, w) = qq[i];
            --counts[x, y];
            (x, y) = ((x + z) % W, (y + w) % H);
            (x, y) = (x < 0 ? x + W : x, y < 0 ? y + H : y);
            qq[i] = (x, y, z, w);
            ++counts[x, y];
        }

        for ((total, run) = (0, 1), i = 0; i < qq.Length; ++i)
            (total, run) = (counts[qq[i].x, qq[i].y + 1] > 0)
                ? (total, ++run)
                : (total > run ? total : run, 1);
    }

    return step;
}

string GetString(int[,] counts)
{
    var chars = new char[(W + 1) * H];
    for (int y = 0, i = 0; y < H; y++, chars[i++] = '\n')
        for (int x = 0; x < W; x++)
            chars[i++] = ".#"[counts[x, y]];
    return new string(chars);
}
