using aoc;
using aoc.Grids;

const int NONE  = 0;
const int ROBOT = 0;
const int LEFT  = 1;
const int RIGHT = 2;
const int BOX   = 3;
const int WALL  = 4;

var cc = "@[]O#";

Vector Score = (1, 100);

var s = File.ReadAllText("input.txt")
    .AsSpan()
    .Trim();

var index = s.IndexOf("\n\n");
var multi = MultiGrid.Builder.Parse(s[..index], cc, out var range);
var path = Grid.Vector.ParseAll(s[index..], '\n');

int[] grid;
Stack<(Vector, int)> stack = new();
Vector pos;

Console.WriteLine(Part1());
Console.WriteLine(Part2());
Console.WriteLine(GetString(pos));

int Part1() => Solve(
    Matrix.One,
    Matrix.One,
    BOX, BOX);

int Part2() => Solve(
    (2, 0, 0, 1, 0, 0),
    (2, 0, 0, 1, 1, 0),
    LEFT, RIGHT);

int Solve(Matrix even, Matrix odd, int left, int right)
{
    Init(even, odd, left, right);
    foreach (var vec in path)
    {
        stack.Clear();
        if (!TryAdd(pos + vec, vec))
            continue;
        foreach (var (box, _) in stack)
            range.SetValue(grid, box, NONE);
        foreach (var (box, value) in stack)
            range.SetValue(grid, box + vec, value);
        pos += vec;
    }
    return GetScore();
}

void Init(Matrix even, Matrix odd, int left, int right)
{
    pos = multi[ROBOT].Single() * even;
    range = (range.Min * even, range.Max * odd);
    grid = new int[range.Length];
    foreach (var box in multi[WALL])
        range.SetValue(grid, box * even, WALL);
    foreach (var box in multi[WALL])
        range.SetValue(grid, box * odd, WALL);
    foreach (var box in multi[BOX])
        range.SetValue(grid, box * even, left);
    foreach (var box in multi[BOX])
        range.SetValue(grid, box * odd, right);
}

bool TryAdd(Vector pos, Vector vec)
{
    var value = range.GetValue(grid, pos);
    return value == NONE
        || (value != WALL
        && TryAdd(pos + vec, vec)
        && TryAddBox(pos, vec, value));
}

bool TryAddBox(Vector pos, Vector vec, int value)
{
    stack.Push((pos, value));
    if (vec.x == 0 && value < BOX)
    {
        pos += (value == LEFT) ? (1, 0) : (-1, 0);
        if (!TryAdd(pos + vec, vec))
            return false;
        stack.Push((pos, value ^ BOX));
    }
    return true;
}

int GetScore()
{
    var total = 0;
    for (int y = 0, i = 0; y < range.Height; y++)
        for (int x = 0; x < range.Width; x++, i++)
            if (grid[i] == LEFT || grid[i] == BOX)
                total += Score.Dot((x, y));
    return total;
}

string GetString(Vector pos)
{
    range.SetValue(grid, pos, cc.Length);
    return MultiGrid.Builder.FromArray(grid, range)
        .ToString("[]O#@");
}
