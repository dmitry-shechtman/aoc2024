using System.Text.RegularExpressions;

Regex regex = new(@"^(Register \w: (?<reg>\d+)\n)+\nProgram: ((?<prg>[0-7]),)+(?<prg>[0-7])$");

var input = File.ReadAllText("input.txt").Trim();
var groups = regex.Match(input).Groups;
var reg = groups[^2].GetValuesInvariant(long.Parse);
var prg = groups[^1].GetValuesInvariant(int.Parse);
var val = new[] { 0L, 1, 2, 3, 0, 0, 0 };
var output = new int[prg.Length];

Console.WriteLine(Part1());
Console.WriteLine(Part2());

string Part1() =>
    string.Join(',', Run(prg.Length));

long Part2()
{
    reg[0] = 0;
    for (int i = 1; i <= prg.Length; i++)
        for (reg[0] <<= 3; ; ++reg[0])
            if (prg.AsSpan().EndsWith(Run(i)))
                break;
    return reg[0];
}

int[] Run(int max)
{
    const int A = 4, B = 5, C = 6;
    int ip = 0, length = 0, op, x, y;
    reg.CopyTo(val, A);
    while (ip < prg.Length && length < max)
    {
        (op, x) = (prg[ip++], prg[ip++]);
        y = (int)val[x];
        _ = op switch
        {
            1 => val[B]  ^= x,
            2 => val[B]   = y & 7,
            4 => val[B]  ^= val[C],
            3 => ip = val[A] != 0 ? x : ip,
            5 => output[length++] = y & 7,
            _ => val[op > 0 ? op - 1 : A] = val[A] >> y,
        };
    }
    return output[..length];
}
