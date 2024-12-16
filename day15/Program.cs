const int NONE  = 0;
const int LEFT  = 1;
const int RIGHT = 2;
const int BOX   = 3;
const int WALL  = 4;
const int ROBOT = 5;

var cc = ".[]O#@";

var s = File.ReadAllText("input.txt")
    .AsSpan()
    .Trim();

var index = s.IndexOf("\n\n");
var field = s[..index];
var width = field.IndexOf('\n');
var height = (field.Length + 1) / (width + 1);

var grid0 = new int[width * height];
for (int i = 0, j = 0; i < field.Length; i++)
    if (field[i] != '\n')
        grid0[j++] = cc.IndexOf(field[i]);

int[] headings = new[] { -width, 1, width, -1 };
List<int> path = new();
for (int i = index + 2; i < s.Length; i++)
    if (s[i] != '\n')
        path.Add(headings["^>v<".IndexOf(s[i])]);

int[] grid = Array.Empty<int>();
int[] stack = new int[field.Length];
int count;

Console.WriteLine(Solve(Init1, out _));
Console.WriteLine(Solve(Init2, out var pos));
Console.WriteLine(GetString(pos));

int Solve(Func<int> init, out int pos)
{
    pos = init();
    foreach (var vec in path)
    {
        count = 0;
        if (!TryAdd(pos + vec, vec))
            continue;
        for (int i = 0; i < count; i++)
            grid[stack[i] & 0xFFFF] = NONE;
        for (int i = 0; i < count; i++)
            grid[(stack[i] & 0xFFFF) + vec] = stack[i] >> 16;
        pos += vec;
    }
    return GetScore();
}

int Init1()
{
    grid = new int[width * height];
    var pos = 0;
    for (int i = 0; i < grid.Length; i++)
        if (grid0[i] == ROBOT)
            pos = i;
        else
            grid[i] = grid0[i];
    return pos;
}

int Init2()
{
    width *= 2;
    grid = new int[width * height];
    var pos = 0;
    for (int i = 0; i < grid.Length; i++)
        (grid[i], pos) = grid0[i >> 1] switch
        {
            BOX => ((i & 1) + 1, pos),
            WALL => (WALL, pos),
            ROBOT when (i & 1) == 0 => (NONE, i),
            _ => (NONE, pos)
        };
    for (int i = 0; i < path.Count; i++)
        path[i] = Math.Abs(path[i]) > 1 ? path[i] << 1 : path[i];
    return pos;
}

bool TryAdd(int pos, int vec)
{
    var value = grid[pos];
    return value switch
    {
        NONE => true,
        BOX => TryAddBox(pos, vec, value),
        LEFT => TryAddBox2(pos, vec, value),
        RIGHT => TryAddBox2(pos, vec, value),
        WALL => false,
        _ => throw new()
    };
}

bool TryAddBox(int pos, int vec, int value)
{
    if (!TryAdd(pos + vec, vec))
        return false;
    stack[count++] = pos | value << 16;
    return true;
}

bool TryAddBox2(int pos, int vec, int value)
{
    if (!TryAddBox(pos, vec, value))
        return false;
    if (Math.Abs(vec) > 1)
    {
        pos += 3 - (value << 1);
        if (!TryAdd(pos + vec, vec))
            return false;
        stack[count++] = pos | (value ^ BOX) << 16;
    }
    return true;
}

int GetScore()
{
    var total = 0;
    for (int y = 0, i = 0, score = 0; y < height; ++y, score += (100 - width))
        for (int x = 0; x < width; ++x, ++i, ++score)
            if ((grid[i] & LEFT) != 0)
                total += score;
    return total;
}

string GetString(int pos)
{
    grid[pos] = ROBOT;
    char[] chars = new char[(width + 1) * height];
    for (int y = 0, i = 0, j = 0; y < height; y++, chars[j++] = '\n')
        for (int x = 0; x < width; x++, i++)
            chars[j++] = cc[grid[i]];
    return new string(chars);
}
