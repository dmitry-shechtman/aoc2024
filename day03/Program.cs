var input = File.ReadAllText("input.txt");
Console.WriteLine(Part1(input));
Console.WriteLine(Part2(input));

int Part1(string input) =>
    EnumValues(input, @"mul\((\d+),(\d+)\)")
        .Sum(Parse);

int Part2(string input) =>
    EnumValues(input, @"mul\((\d+),(\d+)\)|do\(\)|don't\(\)")
        .Aggregate((true, v: 0), Parse2).v;

IEnumerable<string[]> EnumValues(string s, string p) =>
    Regex.Matches(s, p)
        .Cast<Match>()
        .Select(GetValues);

string[] GetValues(Match match) =>
    match.Groups
        .Cast<Group>()
        .Select(g => g.Value)
        .ToArray();

int Parse(string[] ss) =>
    int.Parse(ss[1]) * int.Parse(ss[2]);

(bool, int) Parse2((bool b, int v) t, string[] ss) => ss[0] switch
{
    "do()" => (true, t.v),
    "don't()" => (false, t.v),
    _ => t.b ? (t.b, t.v + Parse(ss)) : t
};
