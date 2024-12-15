using aoc;
using aoc.Grids;

const int NONE  = 0;
const int LEFT  = 1;
const int RIGHT = 2;
const int BOX   = 3;
const int WALL  = 4;
const int ROBOT = 5;

var cc = ".[]O#@";

Matrix Even  = (2, 0, 0, 1, 0, 0);
Matrix Odd   = (2, 0, 0, 1, 1, 0);
Vector Score = (1, 100);

var s = File.ReadAllText("input.txt")
    .AsSpan()
    .Trim();

var index = s.IndexOf("\n\n");
var multi = MultiGrid.Parse(s[..index], cc, out var size);
var path = Grid.ParseVectors(s[index..], '\n');

int[] grid = Array.Empty<int>();
Stack<(Vector, int)> stack = new();
Queue<Vector> queue = new();

Console.WriteLine(Solve(Init1));
Console.WriteLine(Solve(Init2));
Console.WriteLine(GetString());

int Solve(Func<Vector> init)
{
    var pos = init();
    SetValue(pos, ROBOT);
    foreach (var vec in path)
    {
        stack.Clear();
        if (!TryAdd(pos, vec))
            continue;
        foreach (var (box, _) in stack)
            SetValue(box, NONE);
        foreach (var (box, value) in stack)
            SetValue(box + vec, value);
        pos += vec;
    }
    return GetScore();
}

Vector Init1()
{
    grid = new int[size.Length];
    foreach (var box in multi[WALL])
        SetValue(box, WALL);
    foreach (var box in multi[BOX])
        SetValue(box, BOX);
    return multi[ROBOT].Single();
}

Vector Init2()
{
    size = new((Vector)size * Even);
    grid = new int[size.Length];
    foreach (var box in multi[WALL])
        SetValue(box * Even, WALL);
    foreach (var box in multi[WALL])
        SetValue(box * Odd, WALL);
    foreach (var box in multi[BOX])
        SetValue(box * Even, LEFT);
    foreach (var box in multi[BOX])
        SetValue(box * Odd, RIGHT);
    return multi[ROBOT].Single() * Even;
}

bool TryAdd(Vector pos, Vector vec)
{
    var value = GetValue(pos);
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
    for (int y = 0, i = 0; y < size.Height; y++)
        for (int x = 0; x < size.Width; x++, i++)
            if (grid[i] == LEFT || grid[i] == BOX)
                total += Score.Dot((x, y));
    return total;
}

int GetValue(Vector pos) =>
    grid[pos.y * size.Width + pos.x];

void SetValue(Vector pos, int value) =>
    grid[pos.y * size.Width + pos.x] = value;

string GetString()
{
    char[] chars = new char[(size.Width + 1) * size.Height];
    for (int y = 0, i = 0, j = 0; y < size.Height; y++, chars[j++] = '\n')
        for (int x = 0; x < size.Width; x++, i++)
            chars[j++] = cc[grid[i]];
    return new string(chars);
}
