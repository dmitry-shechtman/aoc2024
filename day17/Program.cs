using System.Text.RegularExpressions;

Regex regex = new(@"^(Register \w: (?<reg>\d+)\n)+\nProgram: ((?<prg>[0-7]),)+(?<prg>[0-7])$");

var input = File.ReadAllText("input.txt").Trim();
var values = regex.GetAllValuesInvariant<int>(input, ^2..);
var reg = values["reg"];
var prg = values["prg"];

Console.WriteLine(Part1());
Console.WriteLine(Part2());

string Part1() =>
    string.Join(',', Run(reg[0], reg[1], reg[2]).ToArray());

long Part2()
{
    long a = 0L, b = reg[1], c = reg[2];
    for (int i = 1; i <= prg.Length; i++)
        for (a <<= 3; ; ++a)
            if (Run(a, b, c)[..i].SequenceEqual(prg.AsSpan()[^i..]))
                break;
    return a;
}

ReadOnlySpan<int> Run(long a, long b, long c)
{
    const int A = 4, B = 5, C = 6;
    var output = new int[prg.Length];
    int ip = 0, length = 0, op, x, y;
    var val = new[] { 0, 1, 2, 3, a, b, c };
    while (ip < prg.Length)
    {
        (op, x) = (prg[ip++], prg[ip++]);
        y = (int)val[x];
        _ = op switch
        {
            0 => val[A] >>= y,
            1 => val[B]  ^= x,
            2 => val[B]   = y & 7,
            4 => val[B]  ^= val[C],
            6 => val[B]   = val[A] >> y,
            7 => val[C]   = val[A] >> y,
            3 => ip = val[A] != 0 ? x : ip,
            5 => output[length++] = y & 7,
            _ => throw new()
        };
    }
    return output.AsSpan()[..length];
}
