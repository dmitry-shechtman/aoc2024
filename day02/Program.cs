var input = File.ReadLines("input.txt")
    .Select(s => s.Split(' ').Select(int.Parse));

Console.WriteLine(input.Count(IsSafe1));
Console.WriteLine(input.Count(IsSafe2));

bool IsSafe1(IEnumerable<int> a) =>
    IsSafe(a.Zip(a.Skip(1), (x, y) => x - y));

bool IsSafe2(IEnumerable<int> a) =>
    Enumerable.Range(0, a.Count())
        .Any(i => IsSafe1(a.Take(i).Concat(a.Skip(i + 1))));

bool IsSafe(IEnumerable<int> b) =>
    b.Select(Math.Sign).Distinct().Count() == 1 &&
    b.All(x => Math.Abs(x) is >= 1 and <= 3);
