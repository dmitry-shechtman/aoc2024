using aoc;
using aoc.Grids;

const int NONE  = 0;
const int LEFT  = 1;
const int RIGHT = 2;
const int BOX   = 3;
const int WALL  = 4;

Matrix Even  = (2, 0, 0, 1, 0, 0);
Matrix Odd   = (2, 0, 0, 1, 1, 0);
Vector Score = (1, 100);

var tt = File.ReadAllText("input.txt")
    .Trim().Split("\n\n");

var multi = MultiGrid.Parse(tt[0], "#O", out var size);
var start = Vector.FindChar(tt[0], '@');
var path = Grid.ParseVectors(tt[1].Replace("\n", ""));

int[] grid = Array.Empty<int>();
Stack<(Vector, int)> stack = new();
Queue<Vector> queue = new();
Vector pos = default;

Console.WriteLine(Solve(Init1));
Console.WriteLine(Solve(Init2));
Console.WriteLine(GetString(pos));

int Solve(Action init)
{
    init();
    foreach (var vec in path)
    {
        if (!CanPush(pos, vec))
            continue;
        foreach (var (box, _) in stack)
            SetValue(box, NONE);
        foreach (var (box, value) in stack)
            SetValue(box + vec, value);
        pos += vec;
    }
    return GetScore();
}

void Init1()
{
    pos = start;
    grid = new int[size.Length];
    foreach (var box in multi[0])
        SetValue(box, WALL);
    foreach (var box in multi[1])
        SetValue(box, BOX);
}

void Init2()
{
    pos = start * Even;
    size = new((Vector)size * Even);
    grid = new int[size.Length];
    foreach (var box in multi[0])
        SetValue(box * Even, WALL);
    foreach (var box in multi[0])
        SetValue(box * Odd, WALL);
    foreach (var box in multi[1])
        SetValue(box * Even, LEFT);
    foreach (var box in multi[1])
        SetValue(box * Odd, RIGHT);
}

bool CanPush(Vector pos, Vector vec)
{
    stack.Clear();
    queue.Clear();
    queue.Enqueue(pos + vec);
    while (queue.TryDequeue(out pos))
    {
        var value = GetValue(pos);
        if (value == NONE)
            continue;
        if (value == WALL)
            return false;
        AddBox(pos, vec, value);
    }
    return true;
}

void AddBox(Vector pos, Vector vec, int value)
{
    stack.Push((pos, value));
    queue.Enqueue(pos + vec);
    if (vec.x == 0 && value < BOX)
    {
        pos += (value == LEFT) ? (1, 0) : (-1, 0);
        stack.Push((pos, value ^ BOX));
        queue.Enqueue(pos + vec);
    }
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

string GetString(Vector pos)
{
    char[] chars = new char[(size.Width + 1) * size.Height];
    for (int y = 0, i = 0, j = 0; y < size.Height; y++, chars[j++] = '\n')
        for (int x = 0; x < size.Width; x++, i++)
            chars[j++] = pos == (x, y) ? '@' : ".[]O#"[grid[i]];
    return new string(chars);
}
