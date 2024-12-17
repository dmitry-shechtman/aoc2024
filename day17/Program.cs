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
    int ip = 0, length = 0, result;
    while (ip < prg.Length)
    {
        (a, b, c, ip, result) = (prg[ip++] << 3 | prg[ip++]) switch
        {
            // 0 adv
            0x00 => (a, b, c, ip, -1),
            0x01 => (a >> 1, b, c, ip, -1),
            0x02 => (a >> 2, b, c, ip, -1),
            0x03 => (a >> 3, b, c, ip, -1),
            0x04 => (a >> (int)a, b, c, ip, -1),
            0x05 => (a >> (int)b, b, c, ip, -1),
            0x06 => (a >> (int)c, b, c, ip, -1),

            // 1 bxl
            0x08 => (a, b, c, ip, -1),
            0x09 => (a, b ^ 1, c, ip, -1),
            0x0A => (a, b ^ 2, c, ip, -1),
            0x0B => (a, b ^ 3, c, ip, -1),
            0x0C => (a, b ^ 4, c, ip, -1),
            0x0D => (a, b ^ 5, c, ip, -1),
            0x0E => (a, b ^ 6, c, ip, -1),
            0x0F => (a, b ^ 7, c, ip, -1),

            // 2 bst
            0x10 => (a, 0, c, ip, -1),
            0x11 => (a, 1, c, ip, -1),
            0x12 => (a, 2, c, ip, -1),
            0x13 => (a, 3, c, ip, -1),
            0x14 => (a, a & 7, c, ip, -1),
            0x15 => (a, b & 7, c, ip, -1),
            0x16 => (a, c & 7, c, ip, -1),

            // 3 jnz
            0x18 => (a, b, c, a == 0 ? ip : 0, -1),
            0x19 => (a, b, c, a == 0 ? ip : 1, -1),
            0x1A => (a, b, c, a == 0 ? ip : 2, -1),
            0x1B => (a, b, c, a == 0 ? ip : 3, -1),
            0x1C => (a, b, c, a == 0 ? ip : 4, -1),
            0x1D => (a, b, c, a == 0 ? ip : 5, -1),
            0x1E => (a, b, c, a == 0 ? ip : 6, -1),
            0x1F => (a, b, c, a == 0 ? ip : 7, -1),

            // 4 bxc
            0x20 => (a, b ^ c, c, ip, -1),
            0x21 => (a, b ^ c, c, ip, -1),
            0x22 => (a, b ^ c, c, ip, -1),
            0x23 => (a, b ^ c, c, ip, -1),
            0x24 => (a, b ^ c, c, ip, -1),
            0x25 => (a, b ^ c, c, ip, -1),
            0x26 => (a, b ^ c, c, ip, -1),
            0x27 => (a, b ^ c, c, ip, -1),

            // 5 out
            0x28 => (a, b, c, ip, 0),
            0x29 => (a, b, c, ip, 1),
            0x2A => (a, b, c, ip, 2),
            0x2B => (a, b, c, ip, 3),
            0x2C => (a, b, c, ip, (int)(a & 7)),
            0x2D => (a, b, c, ip, (int)(b & 7)),
            0x2E => (a, b, c, ip, (int)(c & 7)),

            // 6 bdv
            0x30 => (a, a, c, ip, -1),
            0x31 => (a, a >> 1, c, ip, -1),
            0x32 => (a, a >> 2, c, ip, -1),
            0x33 => (a, a >> 3, c, ip, -1),
            0x34 => (a, a >> (int)a, c, ip, -1),
            0x35 => (a, a >> (int)b, c, ip, -1),
            0x36 => (a, a >> (int)c, c, ip, -1),

            // 7 cdv
            0x38 => (a, b, a, ip, -1),
            0x39 => (a, b, a >> 1, ip, -1),
            0x3A => (a, b, a >> 2, ip, -1),
            0x3B => (a, b, a >> 3, ip, -1),
            0x3C => (a, b, a >> (int)a, ip, -1),
            0x3D => (a, b, a >> (int)b, ip, -1),
            0x3E => (a, b, a >> (int)c, ip, -1),
            _ => throw new NotImplementedException()
        };
        if (result >= 0)
            output[length++] = result;
    }
    return output[..length];
}
