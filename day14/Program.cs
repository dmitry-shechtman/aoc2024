using aoc;
using System.Text.RegularExpressions;

Regex regex = new(@"^p=(\d+),(\d+) v=(-?\d+),(-?\d+)$");

const int W = 101, H = 103, RUN = 10;
Vector size = new(W, H);
Vector half = size / 2;

var input = File.ReadAllText("input.txt");
var robots = Matrix.ParseRowsAll(input);

Console.WriteLine(Part1());
Console.WriteLine(await Part2Async());

int Part1() => robots
    .Select(m => Step(m, 100))
    .Select(GetQuadrant)
    .Sum()
    .Product();

Vector4D Step(Matrix m, int i, int n = 1)
{
    var (p, v) = m;
    var (x, y) = (p + v * i) % size;
    return new(x < 0 ? x + W : x, y < 0 ? y + H : y, v.x * n, v.y * n);
}

Vector4D GetQuadrant(Vector4D p) => p switch
{
    _ when p.x < half.x && p.y < half.y => (1, 0, 0, 0),
    _ when p.x > half.x && p.y < half.y => (0, 1, 0, 0),
    _ when p.x < half.x && p.y > half.y => (0, 0, 1, 0),
    _ when p.x > half.x && p.y > half.y => (0, 0, 0, 1),
    _ => default
};

async Task<int> Part2Async()
{
    var n = Environment.ProcessorCount;
    return await await Task.WhenAny(Enumerable.Range(0, n)
        .Select(i => Task.Run(() => Part2(i, n))));
}

int Part2(int step, int n)
{
    Span<Vector4D> qq = stackalloc Vector4D[robots.Length];
    int[,] counts = new int[W, H + 1];
    int total, run, x, y, z, w, i;
    for (i = 0; i < robots.Length; ++i)
    {
        qq[i] = Step(robots[i], step, n);
        ++counts[qq[i].x, qq[i].y];
    }

    for (total = 0; total < RUN; step += n)
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
