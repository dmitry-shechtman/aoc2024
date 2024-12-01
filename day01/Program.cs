using aoc;

var input = File.ReadAllLines("input.txt").Select(s => s.Split("   "));
var a = input.Select(s => int.Parse(s[0])).Order().ToArray();
var b = input.Select(s => int.Parse(s[1])).Order().ToArray();

var d = a.Zip(b).Sum(t => Math.Abs(t.First - t.Second));
Console.WriteLine(d);

var s = a.Sum(x => x * b.Count(y => x == y));
Console.WriteLine(s);
