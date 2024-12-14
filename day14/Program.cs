using aoc;
using System.Text.RegularExpressions;

Regex regex = new(@"^p=(\d+),(\d+) v=(-?\d+),(-?\d+)$");

const int W = 101, H = 103;
Vector size = new(W, H);
Vector half = size / 2;

var robots = File.ReadAllLines("input.txt")
    .Select(regex.GetInts)
    .Select(v => new Matrix(v[0], v[1], v[2], v[3]))
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
    int step, total, x, y, i;
    for (i = 0; i < robots.Length; i++)
    {
        qq[i] = (robots[i].m11, robots[i].m12, robots[i].m21, robots[i].m22);
        ++counts[qq[i].x, qq[i].y];
    }

    for (step = 0, total = 0; total < robots.Length / 2; ++step)
    {
        for (i = 0; i < robots.Length; i++)
        {
            --counts[qq[i].x, qq[i].y];
            (x, y) = ((qq[i].x + qq[i].z) % W, (qq[i].y + qq[i].w) % H);
            (x, y) = (x < 0 ? x + W : x, y < 0 ? y + H : y);
            qq[i] = (x, y, qq[i].z, qq[i].w);
            ++counts[qq[i].x, qq[i].y];
        }

        for (total = 0, i = 0; i < qq.Length; i++)
            if (counts[qq[i].x, qq[i].y + 1] > 0)
                ++total;
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
