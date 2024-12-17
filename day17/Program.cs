using System.Text.RegularExpressions;

Regex regex = new(@"^(Register \w: (?<reg>\d+)\n)+\nProgram: ((?<prg>[0-7]),)+(?<prg>[0-7])$");

var input = File.ReadAllText("input.txt").Trim();
var match = regex.Match(input);
var reg = GetInts("reg");
var prg = GetInts("prg");
var output = new int[prg.Length];

Console.WriteLine(Part1());
Console.WriteLine(Part2());

int[] GetInts(string name) =>
    match.Groups[name].Captures
        .Select(c => int.Parse(c.Value))
        .ToArray();

string Part1() =>
    string.Join(',', Run(reg[0], reg[1], reg[2]).ToArray());

long Part2()
{
    long a = 0L, b = reg[1], c = reg[2];
    for (int i = 0; i < prg.Length; i++)
        for (a <<= 3; ; ++a)
            if (Run(a, b, c)[..(i + 1)].SequenceEqual(prg[^(i + 1)..]))
                break;
    return a;
}

ReadOnlySpan<int> Run(long a, long b, long c)
{
    int ip = 0, length = 0, op, x, y;
    var val = new[] { 0, 1, 2, 3, a, b, c, 0 };
    while (ip < prg.Length)
    {
        (a, b, c) = (val[4], val[5], val[6]);
        (op, x)   = (prg[ip], prg[ip + 1]);
        (ip, y)   = (op != 3 || a == 0 ? ip + 2 : x, (int)val[x]);
        (val[4], val[5], val[6]) = op switch
        {
            0 => (a >> y, b,          c),
            1 => (a,      b ^ x,      c),
            2 => (a,      val[x] & 7, c),
            4 => (a,      b ^ c,      c),
            6 => (a,      a >> y,     c),
            7 => (a,      b,          a >> y),
            _ => (a,      b,          c)
        };
        if (op == 5)
            output[length++] = y & 7;
    }
    return output[..length];
}
